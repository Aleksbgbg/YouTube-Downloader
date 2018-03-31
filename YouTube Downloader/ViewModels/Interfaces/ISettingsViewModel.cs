namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System;

    using YouTube.Downloader.Models;

    internal interface ISettingsViewModel : IViewModelBase
    {
        Settings Settings { get; }

        event EventHandler Closed;
    }
}