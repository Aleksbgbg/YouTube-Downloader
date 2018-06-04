namespace YouTube.Downloader.Models
{
    using System;

    internal class ActionButton
    {
        internal ActionButton(string imageName, string text, Action action)
        {
            ImageName = imageName;
            Text = text;
            Action = action;
        }

        public string ImageName { get; }

        public string Text { get; }

        public Action Action { get; }
    }
}