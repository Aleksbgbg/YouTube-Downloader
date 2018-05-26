namespace YouTube.Downloader.ViewModels.Interfaces
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    internal interface IRequeryViewModel : IViewModelBase
    {
        IVideoViewModel NewVideo { get; }

        IObservableCollection<IMatchedVideoViewModel> Results { get; }

        string Query { get; set; }

        IEnumerable<IResult> Search();

        void Initialise(IVideoViewModel requeryTarget);
    }
}