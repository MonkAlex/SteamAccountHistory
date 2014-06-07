using System;
using System.Windows;

namespace SteamAccountHistory
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            this.ListBox.ItemsSource = Library.VisibleGames;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            Library.AllApps = SteamDB.GetApps(LoadAccount.GetPackages());
            Refresh();
        }

        private void Ignore_Click(object sender, RoutedEventArgs e)
        {
            Library.ChangeStatus(SteamApp.AppStatus.Ignored, this.ListBox.SelectedItems);
            Refresh();
        }

        private void Active_Click(object sender, RoutedEventArgs e)
        {
            Library.ChangeStatus(SteamApp.AppStatus.Active, this.ListBox.SelectedItems);
            Refresh();
        }

        private void Complete_Click(object sender, RoutedEventArgs e)
        {
            Library.ChangeStatus(SteamApp.AppStatus.Completed, this.ListBox.SelectedItems);
            Refresh();
        }

        private void Planned_Click(object sender, RoutedEventArgs e)
        {
            Library.ChangeStatus(SteamApp.AppStatus.Planned, this.ListBox.SelectedItems);
            Refresh();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Library.Save();
        }
    }
}
