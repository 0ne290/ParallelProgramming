using Newtonsoft.Json;
using ParallelProgrammingLab1.PetriNetSemaphore;

namespace ParallelProgrammingLab1;

internal static class Program
{
    private static void Main()
    {
        var inputData = JsonConvert.DeserializeObject<InputData>(File.ReadAllText("Input.json"));

        if (inputData == null)
            throw new Exception("Не получилось...");
        
        Console.WriteLine(inputData.Pa);
        Console.WriteLine(inputData.Qt);
        Console.WriteLine(inputData.MaxT);
        Console.WriteLine(inputData.MaxP);

        foreach (var res in inputData.Resources)
        {
            Console.WriteLine($"{res.Name} {res.Capacity}");
        }
        Console.WriteLine();
        foreach (var thread in inputData.Threads)
        {
            Console.WriteLine($"{thread.Name} {thread.Priority} {thread.CpuBurst} {thread.Quantity} {string.Join(", ", thread.ResourceNames)}");
        }
        
        
        /*var res1 = Resource.CreateResource("R1", 2);
        var res2 = Resource.CreateResource("R2", 2);
        var resources = new[] { res1, res2 };
        var resources1 = new[] { res2, res1 };
        Resource.Timeslice = 500;
        Resource.Premptive = true;

        var threads = new[]
        {
            new MyThread("P1", 1, 3, 4, resources), new MyThread("P2", 2, 1, 3, resources1),
            new MyThread("P3", 3, 5, 2, resources), new MyThread("P4", 1, 3, 5, resources1),
            new MyThread("P5", 2, 2, 2, resources),
            new MyThread("P6", 3, 4, 1, resources1)
        };

        foreach (var thread in threads)
        {
            Task.Run(() => thread.Execute(true));
        }
        Resource.Execute();*/

        /*Console.WriteLine
        ("Ввод данных в программу осуществляется через консоль. Результаты работы программы будут асинхронно " +
         "выводиться в файл \"Output.txt\". Формат входных данных:\nНомер алгоритма планирования - [0;1], 0 - " +
         "SjfNonpreemptive, 1 - SjfPreemptiveAbsolutePriority;\nЕсли для детали ввести время обработки 0, то будет " +
         "сгенерировано случайное значение;\nДля всех остальных численных переменных значение должно быть больше " +
         "нуля;\nОграничений на значения текстовых переменных названий нет;\nРазделителем между названиями станков " +
         "для деталей является строка \", \".\n");

        // PA
        PlanningAlgorithms planningAlgorithm;
        while (true)
        {
            Console.Write("Введите номер алгоритма планирования: ");
            if (!Enum.TryParse(Console.ReadLine() ?? string.Empty, out planningAlgorithm))
                continue;
            if (Enum.IsDefined(planningAlgorithm))
                break;
        }

        // QT
        var timeSlice = 0;
        while (timeSlice < 1)
        {
            Console.Write("Введите продолжительность кванта времени в миллисекундах: ");
            timeSlice = Convert.ToInt32(Console.ReadLine());
        }

        // MaxT
        var maxCpuBurst = 0;
        while (maxCpuBurst < 1)
        {
            Console.Write("Введите максимальное время работы потоков в квантах для автоматической " +
                          "случайной генерации: ");
            maxCpuBurst = Convert.ToInt32(Console.ReadLine());
        }

        //// MaxP
        //var maxPriority = 0;
        //while (maxPriority < 1)
        //{
        //    Console.Write("Введите максимальный приоритет потоков для автоматической случайной генерации: ");
        //    maxPriority = Convert.ToInt32(Console.ReadLine());
        //}

        // NR
        var amountResources = 0;
        while (amountResources < 1)
        {
            Console.Write("Введите кол-во ресурсов: ");
            amountResources = Convert.ToInt32(Console.ReadLine());
        }

        // Ресурсы
        var machineNamesUsed = new List<string?> { null, string.Empty };
        for (var i = 1; i <= amountResources; i++)
        {
            var name = string.Empty;// Название
            while (machineNamesUsed.Contains(name))
            {
                Console.Write($"Станок {i}. Введите название: ");
                name = Console.ReadLine()?.Trim();
            }
            machineNamesUsed.Add(name);

            var capacity = 0;// Кол-во потоков одновременного доступа
            while (capacity < 1)
            {
                Console.Write($"Станок {i}. Введите кол-во одновременно обрабатываемых деталей: ");
                capacity = Convert.ToInt32(Console.ReadLine());
            }

            Machine.CreateMachine(name!, capacity);
        }
        machineNamesUsed.RemoveRange(0, 2);

        // NP
        var amountThreads = 0;
        while (amountThreads < 1)
        {
            Console.Write("Введите кол-во деталей: ");
            amountThreads = Convert.ToInt32(Console.ReadLine());
        }

        // Потоки
        var detailNamesUsed = new List<string?> { null, string.Empty };
        var emptyStringArray = new[] { string.Empty };
        for (var i = 1; i <= amountThreads; i++)
        {
            var name = string.Empty;// Название
            while (detailNamesUsed.Contains(name))
            {
                Console.Write($"Деталь {i}. Введите название: ");
                name = Console.ReadLine()?.Trim();
            }
            detailNamesUsed.Add(name);

            var quantity = 0;// Кол-во потоков
            while (quantity < 1)
            {
                Console.Write($"Деталь {i}. Введите кол-во: ");
                quantity = Convert.ToInt32(Console.ReadLine());
            }

            var machineNames = emptyStringArray;// Названия ресурсов
            while (!machineNames.All(machineName => machineNamesUsed.Contains(machineName)))
            {
                Console.Write($"Деталь {i}. Введите названия станков: ");
                machineNames = Console.ReadLine()?.Split(", ").Distinct().ToArray() ?? emptyStringArray;
            }

            Console.Write($"Деталь {i}. Введите время обработки в квантах: ");
            var cpuBurst = Convert.ToInt32(Console.ReadLine()); // Время обработки
            if (cpuBurst < 1)
            {
                var random = new Random();
                cpuBurst = random.Next(1, maxCpuBurst + 1);
            }

            Detail.CreateDetail(machineNames, quantity, timeSlice, cpuBurst, name!);
        }

        Console.WriteLine();

        DisplayEntities();

        var workshop = new Workshop(timeSlice);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        workshop.StartProduction(planningAlgorithm);
        Console.Write(
            $"\nВсе детали обработаны. Затраченное время - {stopwatch.ElapsedMilliseconds} мс.\n\nНажмите любую " +
            $"клавишу для завершения программы... ");
        Console.ReadKey();*/
    }
}
