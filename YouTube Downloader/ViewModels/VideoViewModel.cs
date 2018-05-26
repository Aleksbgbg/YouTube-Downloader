namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class VideoViewModel : ViewModelBase, IVideoViewModel
    {
        private YouTubeVideo _video;
        public YouTubeVideo Video
        {
            get => _video;

            set
            {
                if (_video == value) return;

                _video = value;
                NotifyOfPropertyChange(() => Video);
            }
        }

        public override object GetView(object context = default)
        {
            return null;
        }
    }
}