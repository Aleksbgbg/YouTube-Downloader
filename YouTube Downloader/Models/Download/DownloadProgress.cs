namespace YouTube.Downloader.Models.Download
{
    using Caliburn.Micro;

    internal class DownloadProgress : PropertyChangedBase
    {
        private long _sizeDownloaded;
        public long SizeDownloaded
        {
            get => _sizeDownloaded;

            private set
            {
                if (_sizeDownloaded == value) return;

                _sizeDownloaded = value;
                NotifyOfPropertyChange(() => SizeDownloaded);
            }
        }

        private long _totalDownloadSize;
        public long TotalDownloadSize
        {
            get => _totalDownloadSize;

            set
            {
                if (_totalDownloadSize == value) return;

                _totalDownloadSize = value;
                NotifyOfPropertyChange(() => TotalDownloadSize);
            }
        }

        private double _progressPercentage;
        public double ProgressPercentage
        {
            get => _progressPercentage;

            set
            {
                if (_progressPercentage == value) return;

                _progressPercentage = value;
                NotifyOfPropertyChange(() => ProgressPercentage);

                SizeDownloaded = (long)(TotalDownloadSize * (ProgressPercentage / 100));
            }
        }

        private long _downloadSpeed;
        public long DownloadSpeed
        {
            get => _downloadSpeed;

            set
            {
                if (_downloadSpeed == value) return;

                _downloadSpeed = value;
                NotifyOfPropertyChange(() => DownloadSpeed);
            }
        }

        private int _stage;
        public int Stage
        {
            get => _stage;

            set
            {
                if (_stage == value) return;

                _stage = value;
                NotifyOfPropertyChange(() => Stage);
            }
        }
    }
}