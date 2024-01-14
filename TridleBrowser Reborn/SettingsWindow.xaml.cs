using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

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
    }
}