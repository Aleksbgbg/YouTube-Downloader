namespace YouTube.Downloader.ViewModels.Interfaces
{
    using YouTube.Downloader.Models;

    internal interface IActionButtonViewModel : IViewModelBase
    {
        ActionButton ActionButton { get; }

        void Initialise(ActionButton actionButton);
    }
}