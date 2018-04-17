namespace YouTube.Downloader
{
    using System.Threading;
    using System.Windows;

    public partial class App
    {
        private readonly Mutex _mutex;

        public App()
        {
            if (Mutex.TryOpenExisting("YouTube Downloader", out Mutex _))
            {
                MessageBox.Show("Another instance of YouTube Downloader is already open.", "Instance Already Open", MessageBoxButton.OK, MessageBoxImage.Warning);
                Current.Shutdown();
                return;
            }

            _mutex = new Mutex(true, "YouTube Downloader");

            Dispatcher.UnhandledException += (sender, e) =>
            {
                e.Handled = true;
                MessageBox.Show($"Operation unsuccessful.\n\n{e.Exception.Message}", "An Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            };
        }
    }
}