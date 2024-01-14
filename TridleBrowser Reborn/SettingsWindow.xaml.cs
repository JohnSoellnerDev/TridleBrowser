using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TridleBrowser_Reborn
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            string defaultSearchEngine = Properties.Settings.Default.defaultSearchEngine;

            foreach (ComboBoxItem item in cmb_SearchEngine.Items)
            {
                if ((string)item.Content == defaultSearchEngine)
                {
                    cmb_SearchEngine.SelectedItem = item;
                    break;
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.homepageUrl = txt_HomepageUrl.Text;

            if (cmb_SearchEngine.SelectedItem is ComboBoxItem selectedItem)
            {
                Properties.Settings.Default.defaultSearchEngine = (string)selectedItem.Content;
            }
            Properties.Settings.Default.Save();

            Console.WriteLine("defaultSearchEngine: " + Properties.Settings.Default.defaultSearchEngine);
        }

        private async void btnCheckUpdates_Click(object sender, RoutedEventArgs e)
        {
            var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue("TridleBrowser"));
            var releases = await client.Repository.Release.GetAll("TridleGamesSV", "TridleBrowser");
            var latest = releases[0];

            var currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var latestVersion = new Version(latest.TagName.TrimStart('v'));

            if (latestVersion > currentVersion)
            {
                var result = MessageBox.Show($"Do you want to update to the latest version {latestVersion}?", "Update available", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var asset = latest.Assets.First(a => a.Name == "TridleBrowser.exe");
                    var assetUrl = asset.BrowserDownloadUrl;

                    using (var webClient = new System.Net.WebClient())
                    {
                        var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TridleBrowser.exe");
                        await webClient.DownloadFileTaskAsync(new Uri(assetUrl), path);
                    }

                    System.Diagnostics.Process.Start(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TridleBrowser.exe"));
                }
            }
            else
            {
                MessageBox.Show("Everything is up to date", "No updates available", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}