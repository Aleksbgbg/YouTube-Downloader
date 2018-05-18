namespace YouTube.Downloader.Services
{
    using System.Collections.Generic;
    using System.Windows;

    using Caliburn.Micro;

    using YouTube.Downloader.Services.Interfaces;

    internal class DialogService : IDialogService
    {
        private static readonly Dictionary<string, object> DefaultSettings = new Dictionary<string, object>
        {
            ["WindowStartupLocation"] = WindowStartupLocation.CenterScreen
        };

        private readonly IWindowManager _windowManager;

        public DialogService(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public void ShowDialog<TViewModel>()
        {
            TViewModel viewModel = IoC.Get<TViewModel>();

            _windowManager.ShowDialog(viewModel, default, DefaultSettings);
        }
    }
}