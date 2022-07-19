#region Types of run Tasks
/************************************** 1 ****************************************/

Task task1 = new Task(() => Console.WriteLine("Task1 is executed"));
task1.Start();

Task task2 = Task.Factory.StartNew(() => Console.WriteLine("Task2 is executed"));

Task task3 = Task.Run(() => Console.WriteLine("Task3 is executed"));

task1.Wait();
task2.Wait();
task3.Wait();
Console.WriteLine(new string('-', 25));

/*************************************** 2 ***************************************/

Console.WriteLine("Main Starts");
// создаем задачу
Task task4 = new Task(() =>
{
    Console.WriteLine("Task Starts");
    Thread.Sleep(1000);     // задержка на 1 секунду - имитация долгой работы
    Console.WriteLine("Task Ends");
});
task4.Start();  // запускаем задачу
task4.Wait();   // ожидаем выполнения задачи
Console.WriteLine("Main Ends");
Console.WriteLine(new string('-', 25));

/************************************** 3 ****************************************/

Console.WriteLine("Main Starts");
// создаем задачу
Task task5 = new Task(() =>
{
    Console.WriteLine("Task Starts");
    Thread.Sleep(1000);
    Console.WriteLine("Task Ends");
});
task5.RunSynchronously(); // запускаем задачу синхронно
Console.WriteLine("Main Ends"); // этот вызов ждет завершения задачи task1 
Console.WriteLine(new string('-', 25));

/************************************** 4 ****************************************/

Task task6 = new Task(() =>
{
    Console.WriteLine($"Task{Task.CurrentId} Starts");
    Thread.Sleep(1000);
    Console.WriteLine($"Task{Task.CurrentId} Ends");
});
task6.Start(); //запускаем задачу

// получаем информацию о задаче
Console.WriteLine($"task1 Id: {task6.Id}");
Console.WriteLine($"task1 is Completed: {task6.IsCompleted}");
Console.WriteLine($"task1 Status: {task6.Status}");

task6.Wait(); // ожидаем завершения задачи
Console.WriteLine(new string('-', 25));

#endregion

#region Вложенные задачи
/************************************** 1 ****************************************/

var outer = Task.Factory.StartNew(() =>      // внешняя задача
{
    Console.WriteLine("Outer task starting...");
    var inner = Task.Factory.StartNew(() =>  // вложенная задача
    {
        Console.WriteLine("Inner task starting...");
        Thread.Sleep(2000);
        Console.WriteLine("Inner task finished.");
    });
});
outer.Wait(); // ожидаем выполнения внешней задачи
Console.WriteLine("End of Main");
Console.WriteLine(new string('-', 25));

/************************************** 2 ****************************************/

var outer2 = Task.Factory.StartNew(() =>      // внешняя задача
{
    Console.WriteLine("Outer task starting...");
    var inner2 = Task.Factory.StartNew(() =>  // вложенная задача
    {
        Console.WriteLine("Inner task starting...");
        Thread.Sleep(2000);
        Console.WriteLine("Inner task finished.");
    }, TaskCreationOptions.AttachedToParent);
});
outer2.Wait(); // ожидаем выполнения внешней задачи
Console.WriteLine("End of Main");
Console.WriteLine(new string('-', 25));

#endregion

#region Массив задач
/************************************** 1 ****************************************/

Task[] tasks1 = new Task[3]
{
    new Task(() => Console.WriteLine("First Task")),
    new Task(() => Console.WriteLine("Second Task")),
    new Task(() => Console.WriteLine("Third Task"))
};
// запуск задач в массиве
foreach (var t in tasks1)
    t.Start();

Console.WriteLine(new string('-', 25));

/************************************** 2 ****************************************/

Task[] tasks2 = new Task[3];
int j = 1;
for (int i = 0; i < tasks2.Length; i++)
    tasks2[i] = Task.Factory.StartNew(() => Console.WriteLine($"Task {j++}"));

Console.WriteLine(new string('-', 25));

/************************************** 3 ****************************************/

Task[] tasks = new Task[3];
for (var i = 0; i < tasks.Length; i++)
{
    tasks[i] = new Task(() =>
    {
        Thread.Sleep(1000); // эмуляция долгой работы
        Console.WriteLine($"Task{i} finished");
    });
    tasks[i].Start();   // запускаем задачу
}
Console.WriteLine("Завершение метода Main");

Task.WaitAll(tasks); // ожидаем завершения всех задач
Console.WriteLine(new string('-', 25));

#endregion

#region Возвращение результатов из задач
/************************************** 1 ****************************************/

int n1 = 4, n2 = 5;
Task<int> sumTask = new Task<int>(() => Sum(n1, n2));
sumTask.Start();

int result = sumTask.Result;
Console.WriteLine($"{n1} + {n2} = {result}"); // 4 + 5 = 9

int Sum(int a, int b) => a + b;
Console.WriteLine(new string('-', 25));

/************************************** 2 ****************************************/

Task<Person> defaultPersonTask = new Task<Person>(() => new Person("Tom", 37));
defaultPersonTask.Start();

Person defaultPerson = defaultPersonTask.Result;
Console.WriteLine($"{defaultPerson.Name} - {defaultPerson.Age}"); // Tom - 37
Console.WriteLine(new string('-', 25));

#endregion

#region Continuation task
/************************************** 1 ****************************************/

Task task11 = new Task(() =>
{
    Console.WriteLine($"Id задачи: {Task.CurrentId}");
});

// задача продолжения - task2 выполняется после task1
Task task21 = task1.ContinueWith(PrintTask);

task11.Start();

// ждем окончания второй задачи
task21.Wait();
Console.WriteLine("Конец метода Main");


void PrintTask(Task t)
{
    Console.WriteLine($"Id задачи: {Task.CurrentId}");
    Console.WriteLine($"Id предыдущей задачи: {t.Id}");
    Thread.Sleep(3000);
}
Console.WriteLine(new string('-', 25));
/************************************** 2 ****************************************/

Task<int> sumTask1 = new Task<int>(() => Sum1(4, 5));

// задача продолжения
Task printTask = sumTask1.ContinueWith(task => PrintResult(task.Result));

sumTask1.Start();

// ждем окончания второй задачи
printTask.Wait();
Console.WriteLine("Конец метода Main");

int Sum1(int a, int b) => a + b;
void PrintResult(int sum) => Console.WriteLine($"Sum: {sum}");
Console.WriteLine(new string('-', 25));

/************************************** 3 ****************************************/

Task task12 = new Task(() => Console.WriteLine($"Current Task: {Task.CurrentId}"));

// задача продолжения
Task task22 = task12.ContinueWith(t =>
    Console.WriteLine($"Current Task: {Task.CurrentId}  Previous Task: {t.Id}"));

Task task32 = task22.ContinueWith(t =>
    Console.WriteLine($"Current Task: {Task.CurrentId}  Previous Task: {t.Id}"));


Task task42 = task32.ContinueWith(t =>
    Console.WriteLine($"Current Task: {Task.CurrentId}  Previous Task: {t.Id}"));

task12.Start();

task42.Wait();   //  ждем завершения последней задачи
Console.WriteLine("Конец метода Main");
Console.WriteLine(new string('-', 25));
#endregion

#region Класс Parallel
/************************************** 1 ****************************************/
// метод Parallel.Invoke выполняет три метода
Parallel.Invoke(
    Print,
    () =>
    {
        Console.WriteLine($"Выполняется задача {Task.CurrentId}");
        Thread.Sleep(3000);
    },
    () => Square(5)
);

void Print()
{
    Console.WriteLine($"Выполняется задача {Task.CurrentId}");
    Thread.Sleep(3000);
}
// вычисляем квадрат числа
void Square(int n)
{
    Console.WriteLine($"Выполняется задача {Task.CurrentId}");
    Thread.Sleep(3000);
    Console.WriteLine($"Результат {n * n}");
}

Console.WriteLine(new string('-', 25));

/************************************** 2 ****************************************/


Console.WriteLine(new string('-', 25));

/************************************** 3 ****************************************/


Console.WriteLine(new string('-', 25));
#endregion



record class Person(string Name, int Age);