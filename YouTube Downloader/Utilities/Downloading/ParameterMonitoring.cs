namespace YouTube.Downloader.Utilities.Downloading
{
    using System;
    using System.Text.RegularExpressions;

    using YouTube.Downloader.EventArgs;

    internal class ParameterMonitoring
    {
        private readonly Func<object, Match, object> _matchProcessor;

        internal ParameterMonitoring(string name, Regex regex, Func<object, Match, object> matchProcessor, object defaultValue = default)
        {
            Name = name;
            Regex = regex;
            _matchProcessor = matchProcessor;
            Value = defaultValue;
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
            Value = _matchProcessor(Value, match);
        }

        internal ParameterMonitoring GetCopy()
        {
            return new ParameterMonitoring(Name, Regex, _matchProcessor, Value);
        }
    }
}