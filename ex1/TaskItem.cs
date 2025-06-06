namespace ex1;

public class TaskItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public int Priority { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime CreationDate { get; set; }
    public string Status { get; set; }
    
    public TaskItem(string title, int priority, DateTime deadline)
    {
        Title = title;
        Description = "";
        Priority = priority;
        Deadline = deadline;
        CreationDate = DateTime.Now;
        Status = "новая";
    }
}