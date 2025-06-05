using Azunt.NoteManagement;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Azunt.Web.Components.Pages.Notes.Components;

public partial class ModalForm : ComponentBase
{
    private IBrowserFile selectedFile = default!;

    #region Properties

    /// <summary>
    /// 모달 다이얼로그 표시 여부
    /// </summary>
    public bool IsShow { get; set; } = false;

    #endregion

    #region Public Methods

    public void Show() => IsShow = true;

    public void Hide()
    {
        IsShow = false;
        StateHasChanged();
    }

    #endregion

    #region Parameters
    [Parameter] public string UserName { get; set; } = "";
    [Parameter] public RenderFragment EditorFormTitle { get; set; } = null!;
    [Parameter] public Note ModelSender { get; set; } = null!;

    public Note ModelEdit { get; set; } = null!;

    [Parameter] public Action CreateCallback { get; set; } = null!;
    [Parameter] public EventCallback<bool> EditCallback { get; set; }
    [Parameter] public int ParentId { get; set; } = 0;
    [Parameter] public string ParentKey { get; set; } = "";
    [Parameter] public string Category { get; set; } = "";
    #endregion

    #region Injectors

    [Inject]
    public INoteRepository RepositoryReference { get; set; } = null!;

    [Inject]
    private INoteStorageService NoteStorage { get; set; } = null!;

    #endregion

    #region Lifecycle

    protected override void OnParametersSet()
    {
        if (ModelSender != null)
        {
            ModelEdit = new Note
            {
                Id = ModelSender.Id,
                Name = ModelSender.Name,
                Active = ModelSender.Active,
                Created = ModelSender.Created,
                CreatedBy = ModelSender.CreatedBy,
                FileName = ModelSender.FileName
            };
        }
        else
        {
            ModelEdit = new Note();
        }
    }

    #endregion

    #region Event Handlers

    protected async Task HandleNoteChange(InputFileChangeEventArgs e)
    {
        selectedFile = e.File;

        using var stream = selectedFile.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
        var noteUrl = await NoteStorage.UploadAsync(stream, selectedFile.Name);

        // 파일명 저장
        ModelEdit.FileName = Path.GetFileName(noteUrl);

        // 만약 Name이 비어 있으면 FileName을 자동 설정
        if (string.IsNullOrWhiteSpace(ModelEdit.Name))
        {
            ModelEdit.Name = ModelEdit.FileName;
        }
    }

    protected async Task HandleValidSubmit()
    {
        ModelSender.Active = true;
        ModelSender.Name = ModelEdit.Name;
        ModelSender.CreatedBy = UserName ?? "Anonymous";

        // 기존 파일 삭제 조건: 수정 중이며 파일명이 바뀐 경우
        bool isNoteReplaced = ModelSender.Id > 0 &&
                              !string.IsNullOrWhiteSpace(ModelSender.FileName) &&
                              ModelSender.FileName != ModelEdit.FileName;

        if (isNoteReplaced)
        {
            // 기존 파일 삭제
            await NoteStorage.DeleteAsync(ModelSender.FileName!);
        }

        ModelSender.FileName = ModelEdit.FileName;
        ModelSender.ParentKey = ParentKey;
        ModelSender.ParentId = ParentId;
        ModelSender.Category = Category;

        if (ModelSender.Id == 0)
        {
            ModelSender.Created = DateTime.UtcNow;
            await RepositoryReference.AddAsync(ModelSender);
            CreateCallback?.Invoke();
        }
        else
        {
            await RepositoryReference.UpdateAsync(ModelSender);
            await EditCallback.InvokeAsync(true);
        }

        Hide();
    }

    #endregion
}