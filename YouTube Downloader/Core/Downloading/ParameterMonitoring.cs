namespace YouTube.Downloader.Core.Downloading
{
    using System;
    using System.Text.RegularExpressions;

    using YouTube.Downloader.EventArgs;

    internal class ParameterMonitoring
    {
        private readonly Func<Match, object> _matchProcessor;

        internal ParameterMonitoring(string name, Regex regex, Func<Match, object> matchProcessor)
        {
            Name = name;
            Regex = regex;
            _matchProcessor = matchProcessor;
        }

        internal event EventHandler<ParameterUpdatedEventArgs> ValueUpdated;

        internal string Name { get; }

        internal Regex Regex { get; }

        private object _value;
        internal object Value
        {
            get => _value;

            private set
            {
                _value = value;
                ValueUpdated?.Invoke(this, new ParameterUpdatedEventArgs(_value));
            }
        }

        internal void Update(Match match)
        {
            Value = _matchProcessor(match);
        }
    }
}