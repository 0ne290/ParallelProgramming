namespace ParallelProgrammingLab1;

internal static class Program
{
    private static void Main()
    {
        var res1 = new Resourse("R1", 2);
        var res2 = new Resourse("R2", 2);
        var resourses1 = new Resourse[] { res1, res2 };
        //var resourses2 = new Resourse[] { res1, res2 };

        var petriSubnet1 = PetriSubnet.CreatePetriSubnet(resourses1);
        var petriSubnet2 = PetriSubnet.CreatePetriSubnet(resourses1);
        var petriSubnet3 = PetriSubnet.CreatePetriSubnet(resourses1);

        var thread1 = new MyThread("P1", 3, 2, 3);
        //var thread2 = new MyThread("P2", 3, 1000);
        var thread3 = new MyThread("P3", 3, 2, 2);
        var thread4 = new MyThread("P4", 3, 2, 1);

        var dict = new Dictionary<MyThread, PetriSubnet>();
        dict.Add(thread1, petriSubnet1);
        //dict.Add(thread2, petriSubnet2);
        dict.Add(thread3, petriSubnet2);
        dict.Add(thread4, petriSubnet3);

        var petriNet = new PetriNet(dict, new List<MyThread>() { thread1, thread3, thread4 }, 500);
        
        petriNet.Execute1();

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
