using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TodoWidget.Models;
using TodoWidget.Services;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using WinRT.Interop;
using System.IO;
using System.Diagnostics;

namespace TodoWidget
{
    public sealed partial class MainWindow : Window
    {
        private readonly TodoService _todoService;
        private ObservableCollection<TodoItem> _tasks;
        private ObservableCollection<TodoList> _lists;
        private TodoList _selectedList;

        public MainWindow()
        {
            this.InitializeComponent();
            _todoService = new TodoService();
            _tasks = new ObservableCollection<TodoItem>();
            _lists = new ObservableCollection<TodoList>();
            
            LoadLists();
            LoadTasks();

            // Set up window properties for widget-like behavior
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 400, Height = 600 });
            
            // Set the window to be always on top and ensure it's visible
            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter.IsAlwaysOnTop = true;
            presenter.IsResizable = true;
            presenter.IsMinimizable = true;
            presenter.IsMaximizable = false;
            
            // Make sure the window is visible
            this.Activate();
            appWindow.Show();
        }

        private void LoadLists()
        {
            _lists.Clear();
            foreach (var list in _todoService.GetTodoLists())
            {
                _lists.Add(list);
            }
            ListSelector.ItemsSource = _lists;
            ListSelector.DisplayMemberPath = "Name";
        }

        private void LoadTasks()
        {
            _tasks.Clear();
            var selectedIndex = TaskTabs.SelectedIndex;
            var tasks = selectedIndex switch
            {
                0 => _todoService.GetTodayItems(),
                1 => _todoService.GetUpcomingItems(),
                _ => _todoService.GetTodoItems(_selectedList?.Id)
            };

            foreach (var task in tasks.OrderBy(t => t.IsCompleted).ThenBy(t => t.DueDate))
            {
                _tasks.Add(task);
            }
            TasksList.ItemsSource = _tasks;
        }

        private async void AddTask_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewTaskTextBox.Text)) return;

            var task = new TodoItem
            {
                Title = NewTaskTextBox.Text,
                ListId = _selectedList?.Id,
                DueDate = DateTime.Today
            };

            await _todoService.AddTodoItem(task);
            NewTaskTextBox.Text = string.Empty;
            LoadTasks();
        }

        private void NewTaskTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                AddTask_Click(sender, null);
            }
        }

        private async void TaskCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TodoItem task)
            {
                task.IsCompleted = true;
                task.CompletedDate = DateTime.Now;
                await _todoService.UpdateTodoItem(task);
                LoadTasks();
            }
        }

        private async void TaskCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.DataContext is TodoItem task)
            {
                task.IsCompleted = false;
                task.CompletedDate = null;
                await _todoService.UpdateTodoItem(task);
                LoadTasks();
            }
        }

        private async void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is TodoItem task)
            {
                await _todoService.DeleteTodoItem(task.Id);
                LoadTasks();
            }
        }

        private void ListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedList = ListSelector.SelectedItem as TodoList;
            LoadTasks();
        }

        private void TaskTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadTasks();
        }
    }
} 