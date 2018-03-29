namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class YouTubeVideoViewModel : ViewModelBase, IYouTubeVideoViewModel
    {
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;

            set
            {
                if (_isSelected == value) return;

                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public YouTubeVideo Video { get; private set; }

        public void Initialise(YouTubeVideo video)
        {
            Video = video;
        }
    }
}