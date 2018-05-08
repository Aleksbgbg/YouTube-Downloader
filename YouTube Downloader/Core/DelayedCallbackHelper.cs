namespace YouTube.Downloader.Core
{
    using System.Timers;

    using Caliburn.Micro;

    using Action = System.Action;

    internal static class DelayedCallbackHelper
    {
        internal static void SetTimeout(double delay, Action callback)
        {
            Timer timer = new Timer(delay)
            {
                AutoReset = false
            };

            void TimerElapsed(object sender, ElapsedEventArgs e)
            {
                timer.Stop();
                timer.Elapsed -= TimerElapsed;

                callback.BeginOnUIThread();
            }

            timer.Elapsed += TimerElapsed;

            timer.Start();
        }
    }
}