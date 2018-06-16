namespace YouTube.Downloader.Services
{
    using System;
    using System.Collections.Generic;

    using YouTube.Downloader.Models.Download;
    using YouTube.Downloader.Services.Interfaces;
    using YouTube.Downloader.Utilities.Processing;

    internal class ConversionService : IConversionService
    {
        private const int MaxConcurrentConversions = 3;

        private readonly Queue<ConvertProcess> _conversionQueue = new Queue<ConvertProcess>();

        private readonly List<ConvertProcess> _currentConversions = new List<ConvertProcess>();

        public void QueueConversion(IEnumerable<ConvertProcess> conversions)
        {
            foreach (ConvertProcess convertProcess in conversions)
            {
                _conversionQueue.Enqueue(convertProcess);
            }

            while (_currentConversions.Count < MaxConcurrentConversions && _conversionQueue.Count > 0)
            {
                ConvertNext();
            }
        }

        private void ConvertNext()
        {
            //while (true)
            //{

            if (_conversionQueue.Count == 0)
            {
                return;
            }

            ConvertProcess convertProcess = _conversionQueue.Dequeue();

            //if (downloadProcess.HasExited)
            //{
            //    continue;
            //}

            _currentConversions.Add(convertProcess);

            void ConversionFinished(object sender, EventArgs e)
            {
                convertProcess.Exited -= ConversionFinished;

                _currentConversions.Remove(convertProcess);

                ConvertNext();
            }

            convertProcess.Exited += ConversionFinished;

            convertProcess.Start();

            //    break;
            //}
        }
    }
}