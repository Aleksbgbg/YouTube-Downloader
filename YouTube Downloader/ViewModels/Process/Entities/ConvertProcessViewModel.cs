namespace YouTube.Downloader.ViewModels.Process.Entities
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Utilities.Downloading;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process.Entities;

    internal class ConvertProcessViewModel : ActiveProcessViewModel, IConvertProcessViewModel
    {
        public ConvertProgress ConvertProgress { get; private set; }

        public void Initialise(IVideoViewModel videoViewModel, ConvertProcess process, ConvertProgress convertProgress)
        {
            Initialise(videoViewModel, process);
            ConvertProgress = convertProgress;
        }
    }
}