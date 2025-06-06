namespace ex1;

public delegate void TaskAddedHandler(TaskItem task);
public delegate void TaskStatusChangedHandler(TaskItem task, string oldStatus, string newStatus);

class Program
{
    static void Main(string[] args)
    {
        TaskManager manager = new TaskManager();
        manager.TaskAdded += (task) =>
        {
            Console.WriteLine($"Задача '{task.Title}' добавлена.");
        };
        manager.TaskStatusChanged += (task, oldStatus, newStatus) =>
        {
            if (newStatus == "сделано")
            {
                Console.Beep();
            }
        };
        
        while (true)
        {
            Console.WriteLine("\n=== Меню ===");
            Console.WriteLine("1. Добавить задачу");
            Console.WriteLine("2. Показать все задачи");
            Console.WriteLine("3. Изменить задачу");
            Console.WriteLine("4. Удалить задачу");
            Console.WriteLine("5. Выход");
            Console.Write("Выберите действие: ");

            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    AddTask(manager);
                    break;
                case "2":
                    ShowTasks(manager);
                    break;
                case "3":
                    EditTask(manager);
                    break;
                case "4":
                    DeleteTask(manager);
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    break;
            }
        }
    }
    
    static void AddTask(TaskManager manager)
    {
        Console.Write("Введите название задачи: ");
        string title = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Название не может быть пустым.");
            return;
        }
        int priority;
        while (true)
        {
            Console.Write("Введите приоритет задачи (целое число): ");
            if (int.TryParse(Console.ReadLine(), out priority))
                break;
            Console.WriteLine("Некорректный ввод. Пожалуйста, введите целое число.");
        }
        DateTime deadline;
        while (true)
        {
            Console.Write("Введите дату дедлайна (гггг-мм-дд): ");
            if (DateTime.TryParse(Console.ReadLine(), out deadline))
                break;
            Console.WriteLine("Некорректный формат даты. Попробуйте снова.");
        }

        TaskItem newTask = new TaskItem(title, priority, deadline);
        manager.AddTask(newTask);
    }
    
    static void ShowTasks(TaskManager manager)
    {
        List<TaskItem> tasks = manager.GetTasks();
        if (tasks.Count == 0)
        {
            Console.WriteLine("Нет задач.");
            return;
        }

        Console.WriteLine("Сортировать по: 1 - приоритет, 2 - название, 3 - дате создания");
        string sortChoice = Console.ReadLine();
        IEnumerable<TaskItem> sortedTasks;
        switch (sortChoice)
        {
            case "1":
                sortedTasks = tasks.OrderBy(t => t.Priority);
                break;
            case "2":
                sortedTasks = tasks.OrderBy(t => t.Title);
                break;
            case "3":
                sortedTasks = tasks.OrderBy(t => t.CreationDate);
                break;
            default:
                Console.WriteLine("Неверный выбор, по умолчанию сортировка по дате создания.");
                sortedTasks = tasks.OrderBy(t => t.CreationDate);
                break;
        }

        Console.WriteLine("\nЗадачи:");
        ConsoleColor origColor = Console.ForegroundColor;
        foreach (var task in sortedTasks)
        {
            if (DateTime.Now > task.Deadline)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = origColor;

            Console.WriteLine($"Название: {task.Title}, Описание: {task.Description}, " +
                              $"Приоритет: {task.Priority}, Дедлайн: {task.Deadline.ToShortDateString()}, " +
                              $"Статус: {task.Status}, Создано: {task.CreationDate}");
        }
        Console.ForegroundColor = origColor;
    }
    
    static void EditTask(TaskManager manager)
    {
        List<TaskItem> tasks = manager.GetTasks();
        if (tasks.Count == 0)
        {
            Console.WriteLine("Нет задач для редактирования.");
            return;
        }

        Console.WriteLine("Выберите задачу для изменения (по номеру):");
        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i].Title} (статус: {tasks[i].Status})");
        }
        Console.Write("Номер: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > tasks.Count)
        {
            Console.WriteLine("Некорректный номер.");
            return;
        }
        index--;

        TaskItem taskToEdit = tasks[index];

        Console.Write("Введите новое описание (оставьте пустым, чтобы не менять): ");
        string newDesc = Console.ReadLine();

        string newStatus = null;
        Console.WriteLine($"Текущий статус: {taskToEdit.Status}");
        switch (taskToEdit.Status)
        {
            case "новая":
                Console.WriteLine("Новый статус: 1 - в работе, 2 - сделано");
                string s1 = Console.ReadLine();
                if (s1 == "1") newStatus = "в работе";
                else if (s1 == "2") newStatus = "сделано";
                else newStatus = taskToEdit.Status;
                break;
            case "в работе":
                Console.WriteLine("Новый статус: 1 - сделано");
                string s2 = Console.ReadLine();
                if (s2 == "1") newStatus = "сделано";
                else newStatus = taskToEdit.Status;
                break;
            case "сделано":
                Console.WriteLine("Статус нельзя изменить.");
                newStatus = taskToEdit.Status;
                break;
        }

        manager.UpdateTask(index, newDesc, newStatus);
        Console.WriteLine("Задача обновлена.");
    }
    
    static void DeleteTask(TaskManager manager)
    {
        List<TaskItem> tasks = manager.GetTasks();
        if (tasks.Count == 0)
        {
            Console.WriteLine("Нет задач для удаления.");
            return;
        }

        Console.WriteLine("Выберите задачу для удаления (по номеру, только статус \"новая\"):");
        for (int i = 0; i < tasks.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i].Title} (статус: {tasks[i].Status})");
        }
        Console.Write("Номер: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > tasks.Count)
        {
            Console.WriteLine("Некорректный номер.");
            return;
        }
        index--;

        if (!manager.RemoveTask(index))
        {
            Console.WriteLine("Удалять можно только задачи со статусом \"новая\".");
        }
        else
        {
            Console.WriteLine("Задача удалена.");
        }
    }
}
