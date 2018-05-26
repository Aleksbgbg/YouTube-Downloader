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
            ["WindowStartupLocation"] = WindowStartupLocation.CenterScreen,
            ["ResizeMode"] = ResizeMode.NoResize
        };

        private readonly IWindowManager _windowManager;

        public DialogService(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public void ShowDialog<TViewModel>()
        {
            TViewModel viewModel = IoC.Get<TViewModel>();
            ShowDialog(viewModel);
        }

        public void ShowDialog<TViewModel>(TViewModel viewModel)
        {
            _windowManager.ShowDialog(viewModel, default, DefaultSettings);
        }
    }
}