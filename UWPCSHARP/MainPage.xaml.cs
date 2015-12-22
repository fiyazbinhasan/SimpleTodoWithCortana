using System;
using Windows.ApplicationModel.Store;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using UWPCSHARP.Model;
using static Windows.UI.Xaml.Window;

namespace UWPCSHARP
{
    public sealed partial class MainPage : Page
    {
        private string _ratingUri = @"";

        public MainPage()
        {
            InitializeComponent();
            Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
        }

        private async void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            var vm = DataContext as ViewModel.ViewModel;
            if (args.VirtualKey == VirtualKey.Enter && args.KeyStatus.IsKeyReleased)
            {
                if (vm != null)
                {
                    vm.Todos.Add(new Todo() { Title = TitleTextBox.Text, IsDone = false });
                    await Utility.Utility.WriteTodosToLocalFolderAsync(vm.Todos);
                    vm.Title = string.Empty;
                }
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var vm = DataContext as ViewModel.ViewModel;
            if (vm != null)
            {
                try
                {
                    var todos = await Utility.Utility.ReadTodosFromLocalFolderAsync();
                    foreach (var todo in todos)
                    {
                        vm.Todos.Add(todo);
                    }
                }
                catch (Exception)
                {
                    MessageDialog dialog =
                        new MessageDialog(
                            "Welcome to Simple Todo! The simplest yet elegant todo manager for you. If you like the app kindly support the developer by giving it a 5 star.");
                    await dialog.ShowAsync();
                }
            }
        }

        private async void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModel.ViewModel;
            if (vm != null)
            {
                vm.Todos.Add(new Todo {Title = vm.Title, IsDone = false});
                await Utility.Utility.WriteTodosToLocalFolderAsync(vm.Todos);
                vm.Title = string.Empty;
            }
        }

        private async void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModel.ViewModel;
            var todo = (Todo) (((Button) sender).Tag);
            if (todo != null)
            {
                vm?.Todos.Remove(todo);
                await Utility.Utility.WriteTodosToLocalFolderAsync(vm?.Todos);
            }
        }

        private async void RatingButton_Click(object sender, RoutedEventArgs e)
        {
            string appId = CurrentApp.AppId.ToString();
            _ratingUri = "ms-windows-store:reviewapp?appid=" + appId;
            var uri = new Uri(_ratingUri);
            await Launcher.LaunchUriAsync(uri);
        }
    }
}
