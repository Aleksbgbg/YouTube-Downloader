#if DEBUG
namespace YouTube.Downloader.Debug
{
    using System;
    using System.Globalization;
    using System.IO;

    internal sealed class DebugLogger : IDisposable
    {
        private readonly StreamWriter _fileStreamWriter;

        static DebugLogger()
        {
            if (!Directory.Exists("Debug"))
            {
                Directory.CreateDirectory("Debug");
            }
        }

        internal DebugLogger()
        {
            _fileStreamWriter = new StreamWriter($"Debug/{DateTime.Now.ToString(CultureInfo.InvariantCulture).Replace('/', '-').Replace(':', '-')}.log")
            {
                AutoFlush = true
            };
        }

        internal void Log(string message)
        {
            Console.WriteLine(message);
            _fileStreamWriter.WriteLine(message);
        }

        internal void Exit()
        {
            _fileStreamWriter.Flush();
            _fileStreamWriter.Close();
        }

        ~DebugLogger()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fileStreamWriter.Dispose();
            }
        }
    }
}
#endif // DEBUG