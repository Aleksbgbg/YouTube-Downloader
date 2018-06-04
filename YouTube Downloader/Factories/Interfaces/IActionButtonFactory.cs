namespace YouTube.Downloader.Factories.Interfaces
{
    using System;

    using YouTube.Downloader.ViewModels.Interfaces;

    internal interface IActionButtonFactory
    {
        IActionButtonViewModel MakeActionButtonViewModel(string image, string name, Action action);
    }
}