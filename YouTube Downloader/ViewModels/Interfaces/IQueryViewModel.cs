namespace YouTube.Downloader.ViewModels.Interfaces
{
    internal interface IQueryViewModel : IViewModelBase
    {
        IMatchedVideosViewModel MatchedVideosViewModel { get; }

        bool IsLoading { get; }
    }
}