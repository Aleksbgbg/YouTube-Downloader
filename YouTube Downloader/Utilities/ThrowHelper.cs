namespace YouTube.Downloader.Utilities
{
    using System;
    using System.Runtime.CompilerServices;

    internal static class ThrowHelper
    {
        internal static NotSupportedException NotSupported([CallerMemberName] string method = default)
        {
            return new NotSupportedException($"{method} is not supported.");
        }
    }
}