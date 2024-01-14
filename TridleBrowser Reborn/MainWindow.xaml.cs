using CefSharp.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using CefSharp;

namespace TridleBrowser_Reborn
{
    public partial class MainWindow
    {
        private readonly string _cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BrowserCookies");

        public MainWindow()
        {
            var settings = new CefSettings
            {
                CachePath = _cachePath
            };

            Cef.Initialize(settings);

            Directory.CreateDirectory(_cachePath);
            InitializeComponent();
            SetWindowProperties();
            
            Webbrowser.Address = Properties.Settings.Default.homepageUrl;
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        }
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.F11)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.SingleBorderWindow;
                }
                else
                {
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.None;
                }
            }
        }

        private void SetWindowProperties()
        {
            Height = Properties.Settings.Default.savedHeight;
            Width = Properties.Settings.Default.savedWidth;
            WindowState = Properties.Settings.Default.savedIsMaximized ? WindowState.Maximized : WindowState.Normal;
        }

        private void Img_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == Img_Back && Webbrowser.CanGoBack) Webbrowser.Back();
            else if (sender == Img_Forward && Webbrowser.CanGoForward) Webbrowser.Forward();
            else if (sender == Img_Reload) Webbrowser.Reload();
            else if (sender == Img_Settings) new SettingsWindow().Show();
        }

        private void Webbrowser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Webbrowser.IsLoaded)
            {
                string url = Webbrowser.Address;
                txt_Url.Document.Blocks.Clear();

                if (url.StartsWith("http://www."))
                {
                    url = url.Substring(11);
                }
                else if (url.StartsWith("https://www."))
                {
                    url = url.Substring(12);
                }

                int dotComIndex = url.IndexOf(".com");
                if (dotComIndex != -1)
                {
                    dotComIndex += 4; // Include ".com" in the first part
                    string firstPart = url.Substring(0, dotComIndex);
                    string secondPart = url.Substring(dotComIndex);

                    var firstPartRun = new Run(firstPart)
                    {
                        Foreground = Brushes.White
                    };

                    var secondPartRun = new Run(secondPart)
                    {
                        Foreground = Brushes.Gray
                    };

                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(firstPartRun);
                    paragraph.Inlines.Add(secondPartRun);

                    txt_Url.Document.Blocks.Add(paragraph);
                }
                else
                {
                    var run = new Run(url)
                    {
                        Foreground = Brushes.White
                    };

                    var paragraph = new Paragraph();
                    paragraph.Inlines.Add(run);

                    txt_Url.Document.Blocks.Add(paragraph);
                }
            }
        }

        private void Txt_Url_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                string url = new TextRange(txt_Url.Document.ContentStart, txt_Url.Document.ContentEnd).Text.Trim();
                txt_Url.Document.Blocks.Clear();
                string formattedUrl = FormatUrl(url);
                Console.WriteLine($"Loading URL: {formattedUrl}");
                Webbrowser.Load(formattedUrl);
            }
        }

        private string FormatUrl(string text)
        {
            if (!text.Contains("."))
            {
                switch (Properties.Settings.Default.defaultSearchEngine)
                {
                    case "Google":
                        text = "https://www.google.com/search?q=" + Uri.EscapeDataString(text);
                        break;
                    case "Bing":
                        text = "https://www.bing.com/search?q=" + Uri.EscapeDataString(text);
                        break;
                    case "DuckDuckGo":
                        text = "https://duckduckgo.com/?q=" + Uri.EscapeDataString(text);
                        break;
                    default:
                        throw new Exception($"Unexpected search engine: {Properties.Settings.Default.defaultSearchEngine}");
                }
            }
            else
            {
                if (!text.StartsWith("http://") && !text.StartsWith("https://"))
                    text = text.StartsWith("www.") ? "http://" + text : "http://www." + text;
            }

            return text;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.savedHeight = Height;
            Properties.Settings.Default.savedWidth = Width;
            Properties.Settings.Default.savedIsMaximized = WindowState == WindowState.Maximized;
            Properties.Settings.Default.Save();
        }
        
        private void Txt_Url_GotFocus(object sender, RoutedEventArgs e)
        {
            var url = Webbrowser.Address;
            txt_Url.Document.Blocks.Clear();

            string protocolPart = string.Empty;
            string restOfUrl = url;

            if (url.StartsWith("http://"))
            {
                protocolPart = url.Substring(0, 7);
                restOfUrl = url.Substring(7);
            }
            else if (url.StartsWith("https://"))
            {
                protocolPart = url.Substring(0, 8);
                restOfUrl = url.Substring(8);
            }

            int dotComIndex = restOfUrl.IndexOf(".com");
            if (dotComIndex != -1)
            {
                dotComIndex += 4; // Include ".com" in the first part
                string firstPart = restOfUrl.Substring(0, dotComIndex);
                string secondPart = restOfUrl.Substring(dotComIndex);

                var protocolPartRun = new Run(protocolPart)
                {
                    Foreground = Brushes.Gray
                };

                var firstPartRun = new Run(firstPart)
                {
                    Foreground = Brushes.White
                };

                var secondPartRun = new Run(secondPart)
                {
                    Foreground = Brushes.Gray
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(protocolPartRun);
                paragraph.Inlines.Add(firstPartRun);
                paragraph.Inlines.Add(secondPartRun);

                txt_Url.Document.Blocks.Add(paragraph);
            }
            else
            {
                var protocolPartRun = new Run(protocolPart)
                {
                    Foreground = Brushes.Gray
                };

                var restOfUrlRun = new Run(restOfUrl)
                {
                    Foreground = Brushes.White
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(protocolPartRun);
                paragraph.Inlines.Add(restOfUrlRun);

                txt_Url.Document.Blocks.Add(paragraph);
            }
        }

        private void Txt_Url_OnLostFocus(object sender, RoutedEventArgs e)
        {
            var url = new TextRange(txt_Url.Document.ContentStart, txt_Url.Document.ContentEnd).Text.Trim();
            txt_Url.Document.Blocks.Clear();

            if (url.StartsWith("http://www."))
            {
                url = url.Substring(11);
            }
            else if (url.StartsWith("https://www."))
            {
                url = url.Substring(12);
            }

            int dotComIndex = url.IndexOf(".com");
            if (dotComIndex != -1)
            {
                dotComIndex += 4; // Include ".com" in the first part
                string firstPart = url.Substring(0, dotComIndex);
                string secondPart = url.Substring(dotComIndex);

                var firstPartRun = new Run(firstPart)
                {
                    Foreground = Brushes.White
                };

                var secondPartRun = new Run(secondPart)
                {
                    Foreground = Brushes.Gray
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(firstPartRun);
                paragraph.Inlines.Add(secondPartRun);

                txt_Url.Document.Blocks.Add(paragraph);
            }
            else
            {
                var run = new Run(url)
                {
                    Foreground = Brushes.White
                };

                var paragraph = new Paragraph();
                paragraph.Inlines.Add(run);

                txt_Url.Document.Blocks.Add(paragraph);
            }
        }
    }
}