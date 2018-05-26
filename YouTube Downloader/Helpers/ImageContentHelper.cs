namespace YouTube.Downloader.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    internal static class ImageContentHelper
    {
        private static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached("Image",
                                                                                                       typeof(ImageSource),
                                                                                                       typeof(ImageContentHelper),
                                                                                                       new FrameworkPropertyMetadata(ImageProperty_Changed));

        private static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached("Content",
                                                                                                         typeof(string),
                                                                                                         typeof(ImageContentHelper),
                                                                                                         new FrameworkPropertyMetadata(ContentProperty_Changed));

        internal static ImageSource GetImage(DependencyObject dependencyObject)
        {
            return (ImageSource)dependencyObject.GetValue(ImageProperty);
        }

        internal static void SetImage(DependencyObject dependencyObject, ImageSource value)
        {
            dependencyObject.SetValue(ImageProperty, value);
        }

        internal static string GetContent(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(ContentProperty);
        }

        internal static void SetContent(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(ContentProperty, value);
        }

        private static void ImageProperty_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            switch (dependencyObject)
            {
                case Button button:
                    {
                        StackPanel stackPanel = GetStackPanel(button);

                        if (!(stackPanel.Children.Count == 2 && stackPanel.Children[0] is Image buttonImage && stackPanel.Children[1] is TextBlock))
                        {
                            throw new InvalidOperationException("Cannot attach image content to button when button content is not in the correct format.");
                        }

                        buttonImage.Source = (ImageSource)e.NewValue;
                    }
                    break;

                case MenuItem menuItem:
                    {
                        if (!(menuItem.Icon is Image))
                        {
                            menuItem.Icon = new Image();
                        }

                        Image image = (Image)menuItem.Icon;

                        image.Source = (ImageSource)e.NewValue;
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dependencyObject), dependencyObject, "Image-attached object must have an image attachment implementation.");
            }
        }

        private static void ContentProperty_Changed(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            switch (dependencyObject)
            {
                case Button button:
                    {
                        StackPanel stackPanel = GetStackPanel(button);

                        if (!(stackPanel.Children.Count == 2 && stackPanel.Children[0] is Image && stackPanel.Children[1] is TextBlock buttonTextBlock))
                        {
                            throw new InvalidOperationException("Cannot attach string content to button when button content is not in the correct format.");
                        }

                        buttonTextBlock.Text = (string)e.NewValue;
                    }
                    break;

                case MenuItem menuItem:
                    menuItem.Header = (string)e.NewValue;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dependencyObject), dependencyObject, "Image-attached object must have an image attachment implementation.");
            }
        }

        private static StackPanel GetStackPanel(Button button)
        {
            if (button.Content is StackPanel panel)
            {
                return panel;
            }

            StackPanel buttonStackPanel = new StackPanel
            {
                    Orientation = Orientation.Horizontal
            };

            buttonStackPanel.Children.Add(new Image
            {
                    Height = 20
            });
            buttonStackPanel.Children.Add(new TextBlock
            {
                    Margin = new Thickness(5, 0, 5, 0)
            });

            button.Content = buttonStackPanel;

            return buttonStackPanel;
        }
    }
}