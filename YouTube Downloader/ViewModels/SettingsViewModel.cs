namespace YouTube.Downloader.ViewModels
{
    using System;
    using System.Collections.Generic;

    using YouTube.Downloader.Models;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly HashSet<string> _changedValues = new HashSet<string>();

        private readonly ISettingsService _settingsService;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            Settings = settingsService.Settings;

            Settings.PropertyChanged += (sender, e) =>
            {
                _changedValues.Add(e.PropertyName);
                NotifyOfPropertyChange(() => CanApply);
            };
        }

        public Settings Settings { get; }

        public event EventHandler Closed;

        public void Ok()
        {
            if (CanApply)
            {
                Apply();
            }

            Deactivate();
        }

        public void Cancel()
        {
            Deactivate();
        }

        public bool CanApply => _changedValues.Count > 0;

        public void Apply()
        {
            _settingsService.Save();
            _changedValues.Clear();
        }

        private void Deactivate()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }
    }
}