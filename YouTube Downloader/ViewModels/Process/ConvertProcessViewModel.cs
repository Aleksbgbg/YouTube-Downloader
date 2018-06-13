namespace YouTube.Downloader.ViewModels.Process
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces.Process;

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