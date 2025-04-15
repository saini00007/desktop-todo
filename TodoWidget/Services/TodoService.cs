using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TodoWidget.Models;

namespace TodoWidget.Services
{
    public class TodoService
    {
        private List<TodoItem> _todoItems;
        private List<TodoList> _todoLists;
        private readonly string _todoItemsFile = "todoItems.json";
        private readonly string _todoListsFile = "todoLists.json";
        private readonly string _dataFolder;

        public TodoService()
        {
            _dataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TodoWidget");
            Directory.CreateDirectory(_dataFolder);
            LoadData();
        }

        private void LoadData()
        {
            var itemsPath = Path.Combine(_dataFolder, _todoItemsFile);
            var listsPath = Path.Combine(_dataFolder, _todoListsFile);

            if (File.Exists(itemsPath))
            {
                var json = File.ReadAllText(itemsPath);
                _todoItems = JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
            }
            else
            {
                _todoItems = new List<TodoItem>();
            }

            if (File.Exists(listsPath))
            {
                var json = File.ReadAllText(listsPath);
                _todoLists = JsonSerializer.Deserialize<List<TodoList>>(json) ?? new List<TodoList>();
            }
            else
            {
                _todoLists = new List<TodoList>
                {
                    new TodoList
                    {
                        Name = "Default",
                        Description = "Default todo list",
                        CreatedDate = DateTime.Now,
                        Emoji = "📝"
                    }
                };
            }
        }

        private async Task SaveData()
        {
            var itemsPath = Path.Combine(_dataFolder, _todoItemsFile);
            var listsPath = Path.Combine(_dataFolder, _todoListsFile);

            var itemsJson = JsonSerializer.Serialize(_todoItems);
            var listsJson = JsonSerializer.Serialize(_todoLists);

            await File.WriteAllTextAsync(itemsPath, itemsJson);
            await File.WriteAllTextAsync(listsPath, listsJson);
        }

        public async Task<TodoItem> AddTodoItem(TodoItem item)
        {
            item.CreatedDate = DateTime.Now;
            _todoItems.Add(item);
            await SaveData();
            return item;
        }

        public async Task<bool> UpdateTodoItem(TodoItem item)
        {
            var existingItem = _todoItems.FirstOrDefault(t => t.Id == item.Id);
            if (existingItem == null) return false;

            var index = _todoItems.IndexOf(existingItem);
            _todoItems[index] = item;
            await SaveData();
            return true;
        }

        public async Task<bool> DeleteTodoItem(string id)
        {
            var item = _todoItems.FirstOrDefault(t => t.Id == id);
            if (item == null) return false;

            _todoItems.Remove(item);
            await SaveData();
            return true;
        }

        public async Task<TodoList> AddTodoList(TodoList list)
        {
            list.CreatedDate = DateTime.Now;
            _todoLists.Add(list);
            await SaveData();
            return list;
        }

        public IEnumerable<TodoItem> GetTodoItems(string listId = null)
        {
            return listId == null 
                ? _todoItems 
                : _todoItems.Where(t => t.ListId == listId);
        }

        public IEnumerable<TodoList> GetTodoLists()
        {
            return _todoLists;
        }

        public IEnumerable<TodoItem> GetTodoItemsByDate(DateTime date)
        {
            return _todoItems.Where(t => t.DueDate?.Date == date.Date);
        }

        public IEnumerable<TodoItem> GetTodayItems()
        {
            return GetTodoItemsByDate(DateTime.Today);
        }

        public IEnumerable<TodoItem> GetUpcomingItems()
        {
            return _todoItems.Where(t => t.DueDate > DateTime.Today);
        }
    }
} 