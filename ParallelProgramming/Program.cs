using ParallelProgramming.Enums;

namespace ParallelProgramming;

internal static class Program
{
    private static void Main()
    {
        // PA
        while (true)
        {
            Console.Write("Введите номер алгоритма планирования: ");
            if (!Enum.TryParse<PlanningAlgorithms>(Console.ReadLine() ?? string.Empty, out var planningAlgorithm))
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
            Console.Write("Введите максимальное время работы потоков в квантах для автоматической" +
                          "случайной генерации: ");
            maxCpuBurst = Convert.ToInt32(Console.ReadLine());
        }
        
        // MaxP
        var maxPriority = 0;
        while (maxPriority < 1)
        {
            Console.Write("Введите максимальный приоритет потоков для автоматической случайной генерации: ");
            maxPriority = Convert.ToInt32(Console.ReadLine());
        }
        
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
            
            var cpuBurst = 0;// Кол-во потоков
            while (cpuBurst < 1)
            {
                Console.Write($"Деталь {i}. Введите время обработки в квантах: ");
                cpuBurst = Convert.ToInt32(Console.ReadLine());
            }

            Detail.CreateDetail(machineNames, quantity, cpuBurst, timeSlice, name!);
        }

        var workshop = new Workshop(timeSlice);
        
        workshop.StartProduction();
    }
}