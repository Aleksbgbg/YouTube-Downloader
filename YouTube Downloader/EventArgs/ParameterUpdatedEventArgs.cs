namespace YouTube.Downloader.EventArgs
{
    using System;

    internal class ParameterUpdatedEventArgs : EventArgs
    {
        internal ParameterUpdatedEventArgs(object newValue)
        {
            NewValue = newValue;
        }

        internal object NewValue { get; }
    }
}