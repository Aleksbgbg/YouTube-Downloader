namespace YouTube.Downloader.ViewModels.Interfaces
{
    internal interface IQueryViewModel : IViewModelBase
    {
        IMatchedVideosViewModel MatchedVideosViewModel { get; }

        bool IsLoading { get; }

        bool QueryBoxIsExpanded { get; }

        string Query { get; set; }
    }
}