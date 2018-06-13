namespace YouTube.Downloader.ViewModels.Interfaces.Process
{
    using YouTube.Downloader.Core.Downloading;
    using YouTube.Downloader.Models;

    internal interface IConvertProcessViewModel : IActiveProcessViewModel
    {
        ConvertProgress ConvertProgress { get; }

        void Initialise(IVideoViewModel videoViewModel, ConvertProcess process, ConvertProgress convertProgress);
    }
}