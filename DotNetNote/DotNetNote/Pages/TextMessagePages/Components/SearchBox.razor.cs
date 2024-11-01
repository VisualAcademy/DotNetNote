using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Timers;

namespace VisualAcademy.Pages.TextMessages.Components
{
    public partial class SearchBox : ComponentBase, IDisposable
    {
        #region Fields
        private string searchQuery;
        private System.Timers.Timer debounceTimer;
        #endregion

        #region Parameters
        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> AdditionalAttributes { get; set; }

        // 자식 컴포넌트에서 발생한 정보를 부모 컴포넌트에게 전달
        [Parameter]
        public EventCallback<string> SearchQueryChanged { get; set; }

        [Parameter]
        public int Debounce { get; set; } = 300;
        #endregion

        #region Properties
        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                debounceTimer.Stop(); // 텍스트박스에 값을 입력하는 동안 타이머 중지
                debounceTimer.Start(); // 타이머 실행(300밀리초 후에 딱 한 번 실행)
            }
        }
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// 페이지 초기화 이벤트 처리기
        /// </summary>
        protected override void OnInitialized()
        {
            debounceTimer = new System.Timers.Timer();
            debounceTimer.Interval = Debounce;
            debounceTimer.AutoReset = false; // 딱 한번 실행 
            debounceTimer.Elapsed += SearchHandler;
        }
        #endregion

        #region Event Handlers
        protected void Search() => SearchQueryChanged.InvokeAsync(SearchQuery); // 부모의 메서드에 검색어 전달

        protected async void SearchHandler(object source, ElapsedEventArgs e) => await InvokeAsync(() => SearchQueryChanged.InvokeAsync(SearchQuery)); // 부모의 메서드에 검색어 전달
        #endregion

        #region Public Methods
        public void Dispose() => debounceTimer.Dispose();
        #endregion
    }
}