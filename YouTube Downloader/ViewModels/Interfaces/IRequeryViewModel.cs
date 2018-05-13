namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    internal interface IRequeryViewModel : IViewModelBase
    {
        IObservableCollection<IMatchedVideoViewModel> Results { get; }

        IEnumerable<IResult> Search(string query);
    }
}