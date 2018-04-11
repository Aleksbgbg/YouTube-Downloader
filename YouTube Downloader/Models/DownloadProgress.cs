namespace YouTube.Downloader.Models
{
    using Caliburn.Micro;

    internal class DownloadProgress : PropertyChangedBase
    {
        internal DownloadProgress(DownloadType downloadType)
        {
            TotalStages = downloadType == DownloadType.Audio ? 1 : 2;
        }

        private double _progressPercentage;
        public double ProgressPercentage
        {
            get => _progressPercentage;

            set
            {
                if (_progressPercentage == value) return;

                if (_progressPercentage > value)
                {
                    ++CurrentStage;
                }

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

        private int _currentStage = 1;
        public int CurrentStage
        {
            get => _currentStage;

            private set
            {
                if (_currentStage == value) return;

                _currentStage = value;
                NotifyOfPropertyChange(() => CurrentStage);
                NotifyOfPropertyChange(() => StageText);
            }
        }

        public int TotalStages { get; }

        public string StageText
        {
            get
            {
                if (TotalStages == 2)
                {
                    return CurrentStage == 1 ? "Downloading Video" : "Downloading Audio";
                }

                return "Downloading Audio";
            }
        }
    }
}