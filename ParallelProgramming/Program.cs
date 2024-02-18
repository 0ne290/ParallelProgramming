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
            Console.Write("Введите количество ресурсов: ");
            amountResources = Convert.ToInt32(Console.ReadLine());
        }

        var machines = new List<Machine>();
        for (var i = 0; i < amountResources; i++)
        {
            machines.Add();
        }
        
        Console.WriteLine();
    }
}