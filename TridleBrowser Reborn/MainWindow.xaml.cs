using CefSharp.Wpf;
using CefSharp;
using System;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;

namespace TridleBrowser_Reborn
{
    public partial class MainWindow
    {
        private readonly string _cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BrowserCookies");

        public MainWindow()
        {

            if (!Directory.Exists(_cachePath))
            {
                Directory.CreateDirectory(_cachePath);
            }

            InitializeComponent();

            Height = Properties.Settings.Default.savedHeight;
            Width = Properties.Settings.Default.savedWidth;
            if (Properties.Settings.Default.savedIsMaximized)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Img_Back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Webbrowser.CanGoBack)
            {
                Webbrowser.Back();
            }
        }

        private void Img_Forward_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Webbrowser.CanGoForward)
            {
                Webbrowser.Forward();
            }
        }
        private void Img_Reload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Webbrowser.Reload();
        }

        private void Webbrowser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Webbrowser.IsLoaded)
            {
                txt_Url.Text = Webbrowser.Address;
            }
        }

        private void Txt_Url_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = txt_Url.Text;

                if (!text.StartsWith("http://") && !text.StartsWith("https://"))
                {
                    if (text.StartsWith("www."))
                    {
                        text = "http://" + text;
                    }
                    else
                    {
                        text = "http://www." + text;
                    }
                }

                string url = Uri.IsWellFormedUriString(text, UriKind.Absolute) ? text : "https://www.google.com/search?q=" + Uri.EscapeDataString(text);
                Webbrowser.Load(url);
            }
        }


        private void img_Settings_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.savedHeight = Height;
            Properties.Settings.Default.savedWidth = Width;
            Properties.Settings.Default.savedIsMaximized = WindowState == WindowState.Maximized;
            Properties.Settings.Default.Save();
        }
    }
}
