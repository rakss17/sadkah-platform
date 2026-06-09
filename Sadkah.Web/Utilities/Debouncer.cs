namespace Sadkah.Web.Utilities
{
    /// <summary>
    /// Debounces an asynchronous action so it only runs after a quiet period.
    /// Each new call cancels the previously scheduled run.
    /// </summary>
    public sealed class Debouncer : IDisposable
    {
        private readonly int delayMilliseconds;
        private CancellationTokenSource? cts;

        public Debouncer(int delayMilliseconds)
        {
            this.delayMilliseconds = delayMilliseconds;
        }

        public void Debounce(Func<Task> action)
        {
            cts?.Cancel();
            cts?.Dispose();
            cts = new CancellationTokenSource();

            _ = RunAfterDelayAsync(action, cts.Token);
        }

        private async Task RunAfterDelayAsync(Func<Task> action, CancellationToken cancellationToken)
        {
            try
            {
                await Task.Delay(delayMilliseconds, cancellationToken);
                await action();
            }
            catch (TaskCanceledException)
            {
            }
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}
