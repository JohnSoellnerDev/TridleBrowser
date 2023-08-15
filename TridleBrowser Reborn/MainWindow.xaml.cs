using CefSharp.Wpf;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.ComponentModel;

namespace TridleBrowser_Reborn
{
    public partial class MainWindow : Window
    {
        private string cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BrowserCookies");

        public MainWindow()
        {

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            var settings = new CefSettings
            {
                CachePath = cachePath
            };

            InitializeComponent();

            Height = Properties.Settings.Default.savedHeight;
            Width = Properties.Settings.Default.savedWidth;
            if (Properties.Settings.Default.savedIsMaximized)
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void img_Back_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Webbrowser.CanGoBack)
            {
                Webbrowser.Back();
            }
        }

        private void img_Forward_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Webbrowser.CanGoForward)
            {
                Webbrowser.Forward();
            }
        }
        private void img_Reload_MouseDown(object sender, MouseButtonEventArgs e)
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

        private void txt_Url_KeyDown(object sender, KeyEventArgs e)
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
                        text = "http://" + text;
                    }
                }

                if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
                {
                    Webbrowser.Load(text);
                }
                else
                {
                    string googleUrl = "https://www.google.com/search?q=" + Uri.EscapeDataString(text);
                    Webbrowser.Load(googleUrl);
                }
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
