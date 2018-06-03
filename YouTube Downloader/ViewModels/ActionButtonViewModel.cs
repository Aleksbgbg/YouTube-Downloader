namespace YouTube.Downloader.ViewModels
{
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    internal class ActionButtonViewModel : ViewModelBase, IActionButtonViewModel
    {
        public ActionButton ActionButton { get; private set; }

        public void Initialise(ActionButton actionButton)
        {
            ActionButton = actionButton;
        }

        public void Clicked()
        {
            ActionButton.Action();
        }
    }
}