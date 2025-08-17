using Microsoft.AspNetCore.Components;
using System.Timers;

namespace VisualAcademy.Pages.TextMessages.Components
{
    /// <summary>
    /// A debounced search box component. Public API remains stable:
    /// - AdditionalAttributes
    /// - SearchQueryChanged (EventCallback&lt;string&gt;)
    /// - Debounce (ms)
    /// You can optionally choose the debounce engine (Timer | Cts) via <see cref="Engine"/>.
    /// Default is Timer to keep full backward compatibility.
    /// </summary>
    public partial class SearchBox : ComponentBase, IDisposable
    {
        /// <summary>
        /// Debounce engine selector (Timer: legacy-compatible, Cts: Task-based modern flow).
        /// </summary>
        public enum DebounceEngine
        {
            Timer,
            Cts
        }

        #region Fields
        private string searchQuery = string.Empty;

        // Timer engine fields
        private System.Timers.Timer? timer;

        // CTS engine fields
        private CancellationTokenSource? cts;
        private readonly object gate = new();
        #endregion

        #region Parameters
        [Parameter(CaptureUnmatchedValues = true)]
        public IDictionary<string, object> AdditionalAttributes { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Pass information from the child component to the parent component when debounced.
        /// </summary>
        [Parameter]
        public EventCallback<string> SearchQueryChanged { get; set; }

        /// <summary>
        /// Debounce interval in milliseconds. If negative, it will be coerced to 0.
        /// </summary>
        [Parameter]
        public int Debounce { get; set; } = 300;

        /// <summary>
        /// Choose the debounce engine. Default is Timer for non-breaking behavior.
        /// </summary>
        [Parameter]
        public DebounceEngine Engine { get; set; } = DebounceEngine.Timer;
        #endregion

        #region Properties
        public string SearchQuery
        {
            get => searchQuery;
            set
            {
                searchQuery = value;
                if (Engine == DebounceEngine.Timer)
                {
                    timer?.Stop();
                    timer?.Start();
                }
                else
                {
                    RestartDebounceCts();
                }
            }
        }
        #endregion

        #region Lifecycle
        protected override void OnInitialized()
        {
            // Initialize the current engine
            InitializeEngine();
        }

        protected override void OnParametersSet()
        {
            // Sanitize Debounce
            if (Debounce < 0) Debounce = 0;

            // If Debounce changed at runtime, reflect it for Timer engine
            if (Engine == DebounceEngine.Timer && timer is not null)
            {
                timer.Interval = Debounce;
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Immediate search trigger (e.g., search button). Does not apply debounce.
        /// </summary>
        protected void Search()
            => SearchQueryChanged.InvokeAsync(SearchQuery);
        #endregion

        #region Timer Engine
        private void InitializeEngine()
        {
            // Dispose any previous engine artifacts to be safe when switching engines at runtime
            DisposeTimerInternal();
            DisposeCtsInternal();

            if (Engine == DebounceEngine.Timer)
            {
                timer = new System.Timers.Timer
                {
                    Interval = Debounce,
                    AutoReset = false // fire once per trigger
                };
                timer.Elapsed += OnTimerElapsed;
            }
            else
            {
                // CTS engine: nothing to initialize eagerly
            }
        }

        private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            // We cannot await here; marshal back to the renderer and ignore the returned Task.
            _ = OnSearchDebouncedAsync();
        }

        private void DisposeTimerInternal()
        {
            if (timer is not null)
            {
                try
                {
                    timer.Stop();
                    timer.Elapsed -= OnTimerElapsed;
                    timer.Dispose();
                }
                catch
                {
                    // Swallow on dispose path
                }
                finally
                {
                    timer = null;
                }
            }
        }
        #endregion

        #region CTS Engine
        private void RestartDebounceCts()
        {
            CancellationTokenSource? old;
            lock (gate)
            {
                old = cts;
                cts = new CancellationTokenSource();
            }

            try { old?.Cancel(); }
            catch { /* ignore */ }
            finally { old?.Dispose(); }

            _ = DebounceAsync(cts.Token);
        }

        private async Task DebounceAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay(Debounce, token);
                await OnSearchDebouncedAsync();
            }
            catch (OperationCanceledException)
            {
                // Expected when user keeps typing; ignore.
            }
        }

        private void DisposeCtsInternal()
        {
            lock (gate)
            {
                try { cts?.Cancel(); }
                catch { /* ignore */ }
                finally
                {
                    cts?.Dispose();
                    cts = null;
                }
            }
        }
        #endregion

        #region Common
        private Task OnSearchDebouncedAsync()
            => InvokeAsync(() => SearchQueryChanged.InvokeAsync(SearchQuery));
        #endregion

        #region IDisposable
        public void Dispose()
        {
            DisposeTimerInternal();
            DisposeCtsInternal();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}