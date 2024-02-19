using ParallelProgramming.Enums;

namespace ParallelProgramming;

internal static class Program
{
    private static void Main()
    {
        while (true)// PA
        {
            Console.Write("Введите номер алгоритма планирования: ");
            if (!Enum.TryParse<PlanningAlgorithms>(Console.ReadLine() ?? string.Empty, out var planningAlgorithm))
                continue;
            if (Enum.IsDefined(planningAlgorithm))
                break;
        }

        var timeSliceInMs = 0;// QT
        while (timeSliceInMs < 1)
        {
            Console.Write("Введите продолжительность кванта времени в миллисекундах: ");
            timeSliceInMs = Convert.ToInt32(Console.ReadLine());
        }
        
        var maxTimeSliceInMs = 0;// MaxT
        while (maxTimeSliceInMs < 1)
        {
            Console.Write("Введите максимальную продолжительность кванта времени в миллисекундах для автоматической" +
                          "случайной генерации: ");
            maxTimeSliceInMs = Convert.ToInt32(Console.ReadLine());
        }
        
        var maxPriority = 0;// MaxP
        while (maxPriority < 1)
        {
            Console.Write("Введите максимальный приоритет потоков для автоматической случайной генерации: ");
            maxPriority = Convert.ToInt32(Console.ReadLine());
        }
        
        var amountResources = 0;// NR
        while (amountResources < 1)
        {
            Console.Write("Введите кол-во ресурсов: ");
            amountResources = Convert.ToInt32(Console.ReadLine());
        }

        var machines = new List<Machine>();// Ресурсы
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
            
            machines.Add(new Machine(name!, capacity));
        }
        machineNamesUsed.RemoveRange(0, 2);
        
        var amountThreads = 0;// NP
        while (amountThreads < 1)
        {
            Console.Write("Введите кол-во деталей: ");
            amountThreads = Convert.ToInt32(Console.ReadLine());
        }

        var details = new List<Detail>();// Потоки
        var detailNamesUsed = new List<string?> { null, string.Empty };
        for (var i = 1; i <= amountResources; i++)
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
                Console.Write($"Станок {i}. Введите кол-во одновременно обрабатываемых деталей: ");
                quantity = Convert.ToInt32(Console.ReadLine());
            }

            var emptyStringArray =new[] { string.Empty };
            var machineNames = emptyStringArray;// Названия ресурсов
            while (!machineNames.All(machineName => machineNamesUsed.Contains(machineName)))
            {
                Console.Write($"Деталь {i}. Введите названия станков в формате: ");
                machineNames = Console.ReadLine()?.Split(", ").Distinct().ToArray() ?? emptyStringArray;
            }
            
            details.Add(new Detail(name!, quantity, machineNames));
        }

        var workshop = new Workshop(machines, details);
    }
}