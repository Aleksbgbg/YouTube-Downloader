namespace YouTube.Downloader.Factories
{
    using Caliburn.Micro;

    using YouTube.Downloader.Factories.Interfaces;
    using YouTube.Downloader.Models;
    using YouTube.Downloader.ViewModels.Interfaces;

    using Action = System.Action;

    internal class ActionButtonFactory : IActionButtonFactory
    {
        public IActionButtonViewModel MakeActionButtonViewModel(string image, string name, Action action)
        {
            IActionButtonViewModel actionButtonViewModel = IoC.Get<IActionButtonViewModel>();
            actionButtonViewModel.Initialise(new ActionButton(image, name, action));

            return actionButtonViewModel;
        }
    }
}