namespace YouTube.Downloader.Models
{
    using Caliburn.Micro;

    internal class DownloadProgress : PropertyChangedBase
    {
        private double _progressPercentage;
        public double ProgressPercentage
        {
            get => _progressPercentage;

            set
            {
                if (_progressPercentage == value) return;

                _progressPercentage = value;
                NotifyOfPropertyChange(() => ProgressPercentage);

                SizeDownloaded = TotalDownloadSize * (ProgressPercentage / 100);
            }
        }

        private double _sizeDownloaded;
        public double SizeDownloaded
        {
            get => _sizeDownloaded;

            private set
            {
                if (_sizeDownloaded == value) return;

                _sizeDownloaded = value;
                NotifyOfPropertyChange(() => SizeDownloaded);
            }
        }

        private double _totalDownloadSize;
        public double TotalDownloadSize
        {
            get => _totalDownloadSize;

            set
            {
                if (_totalDownloadSize == value) return;

                _totalDownloadSize = value;
                NotifyOfPropertyChange(() => TotalDownloadSize);
            }
        }

        private double _downloadSpeed;
        public double DownloadSpeed
        {
            get => _downloadSpeed;

            set
            {
                if (_downloadSpeed == value) return;

                _downloadSpeed = value;
                NotifyOfPropertyChange(() => DownloadSpeed);
            }
        }

        private string _statusText;
        public string StatusText
        {
            get => _statusText;

            set
            {
                if (_statusText == value) return;

                _statusText = value;
                NotifyOfPropertyChange(() => StatusText);
            }
        }
    }
}