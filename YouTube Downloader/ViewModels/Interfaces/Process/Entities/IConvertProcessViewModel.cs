﻿namespace YouTube.Downloader.ViewModels.Interfaces.Process.Entities
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.Utilities.Processing;

    internal interface IConvertProcessViewModel : IActiveProcessViewModel
    {
        ConvertProgress ConvertProgress { get; }

        void Initialise(IVideoViewModel videoViewModel, ConvertProcess process, ConvertProgress convertProgress);
    }
}