using System;

namespace MAUIEssentials.AppCode.Helpers
{
    public class CustomTimer
	{
		private readonly TimeSpan timespan;
		private Action callback;

		private CancellationTokenSource cancellation;

        private IDispatcher? dispatcher;

		public TimeSpan Time { get; set; }
		public bool IsRunning { get; set; }

        public CustomTimer(TimeSpan timespan, Action callback)
        {
            this.timespan = timespan;
            this.callback = callback;
            cancellation = new CancellationTokenSource();
            dispatcher = Application.Current?.Dispatcher;
		}

		public void SetCallback(Action callback)
		{
			this.callback = callback;
		}

        public void Start()
        {   
            try
            {
                if (IsRunning)
                return; // Prevent multiple timers

                IsRunning = true;
                CancellationTokenSource cts = cancellation; // safe copy

                dispatcher?.StartTimer(timespan, () =>
                {
                    if (cts.IsCancellationRequested)
                    {
                        IsRunning = false;
                        return false; // Stop the timer
                    }

                    callback.Invoke();
                    return true; // Continue the timer
                });
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
		}

        public void Stop()
        {
            try
            {
                IsRunning = false;
                Time = TimeSpan.Zero;
                Interlocked.Exchange(ref cancellation, new CancellationTokenSource()).Cancel();
            }
            catch (Exception ex)
            {
                ex.LogException();
            }
		}
	}
}