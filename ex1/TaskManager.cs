using System.Text.Json;
namespace ex1;

public class TaskManager
{
    private List<TaskItem> tasks;
    private const string FileName = "tasks.json";

    public event TaskAddedHandler TaskAdded;
    public event TaskStatusChangedHandler TaskStatusChanged;

    public TaskManager()
    {
        tasks = new List<TaskItem>();
        LoadTasks();
    }
    
    private void LoadTasks()
    {
        if (File.Exists(FileName))
        {
            try
            {
                string json = File.ReadAllText(FileName);
                tasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
            }
            catch
            {
                tasks = new List<TaskItem>();
            }
        }
    }
    
    private void SaveTasks()
    {
        string json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FileName, json);
    }
    
    public void AddTask(TaskItem task)
    {
        tasks.Add(task);
        TaskAdded?.Invoke(task);
        SaveTasks();
    }
    public List<TaskItem> GetTasks()
    {
        return tasks;
    }
    
    public void UpdateTask(int index, string newDescription, string newStatus)
    {
        if (index < 0 || index >= tasks.Count) return;
        TaskItem task = tasks[index];
        if (!string.IsNullOrEmpty(newDescription))
        {
            task.Description = newDescription;
        }
        if (!string.IsNullOrEmpty(newStatus) && newStatus != task.Status)
        {
            string oldStatus = task.Status;
            task.Status = newStatus;
            TaskStatusChanged?.Invoke(task, oldStatus, newStatus);
        }
        SaveTasks();
    }
    public bool RemoveTask(int index)
    {
        if (index < 0 || index >= tasks.Count) return false;
        if (tasks[index].Status != "новая")
            return false;
        tasks.RemoveAt(index);
        SaveTasks();
        return true;
    }
}