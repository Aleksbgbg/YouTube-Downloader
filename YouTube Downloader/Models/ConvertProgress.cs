namespace YouTube.Downloader.Models
{
    using Caliburn.Micro;

    internal class ConvertProgress : PropertyChangedBase
    {
        internal ConvertProgress(long totalSize)
        {
            TotalSize = totalSize;
        }

        private long _convertedBytes;
        public long ConvertedBytes
        {
            get => _convertedBytes;

            set
            {
                if (_convertedBytes == value) return;

                _convertedBytes = value;
                NotifyOfPropertyChange(() => ConvertedBytes);

                Progress = (double)_convertedBytes / TotalSize;
            }
        }

        public long TotalSize { get; }

        private double _progress;
        public double Progress
        {
            get => _progress;

            set
            {
                if (_progress == value) return;

                _progress = value;
                NotifyOfPropertyChange(() => Progress);
            }
        }

        private long _bitrate;
        public long Bitrate
        {
            get => _bitrate;

            set
            {
                if (_bitrate == value) return;

                _bitrate = value;
                NotifyOfPropertyChange(() => Bitrate);
            }
        }
    }
}