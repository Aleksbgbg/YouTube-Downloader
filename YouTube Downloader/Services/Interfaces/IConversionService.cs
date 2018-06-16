namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    using YouTube.Downloader.Utilities.Processing;

    internal interface IConversionService
    {
        void QueueConversion(IEnumerable<ConvertProcess> conversions);
    }
}