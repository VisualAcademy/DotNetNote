using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace VisualAcademy.Pages.TextMessages.Components
{
    public partial class DeleteDialog
    {
        #region Parameters
        /// <summary>
        /// 부모에서 OnClickCallback 속성에 지정한 이벤트 처리기 실행
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClickCallback { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// 모달 다이얼로그를 표시할건지 여부 
        /// </summary>
        public bool IsShow { get; set; } = false;
        #endregion

        #region Public Methods
        /// <summary>
        /// 폼 보이기 
        /// </summary>
        public void Show() => IsShow = true;

        /// <summary>
        /// 폼 닫기
        /// </summary>
        public void Hide() => IsShow = false;
        #endregion
    }
}
