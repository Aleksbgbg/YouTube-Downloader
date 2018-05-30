namespace YouTube.Downloader.Services.Interfaces
{
    using System.Collections.Generic;

    using YouTube.Downloader.Core.Downloading;

    internal interface IConversionService
    {
        void QueueConversion(IEnumerable<ConvertProcess> conversions);
    }
}