using System;
using System.Collections.Generic;

namespace TodoWidget.Models
{
    public class TodoItem
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string FormattedDueDate => DueDate.HasValue ? DueDate.Value.ToString("MMM dd") : string.Empty;
        public string ListId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string Emoji { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int Priority { get; set; }
    }

    public class TodoList
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Emoji { get; set; }
    }
} 