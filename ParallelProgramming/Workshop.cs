using System.Diagnostics;
using ParallelProgramming.Enums;

namespace ParallelProgramming;

public class Workshop
{
    public Workshop(int timeSlice) => _timeSlice = timeSlice;
    
    /*
        Попытка №2 тоже не удалась(. Выкинуть таймер и однопоточно отслеживать кванты в ГЛАВНОМ ЦИКЛЕ (строка №16).
        Логика такая: главный цикл будет выполняться до тех пор, пока хотя бы одна деталь не находится в завершенном
        состоянии. Внутри этого цикла будет другой цикл, запускающийся каждый квант времени. Внутри этого цикла уже
        будет логироваться инфа о деталях и станках и запускаться выбираемые определённой логикой потоки до тех пор,
        пока не будут заняты все станки. Как это всё примерно должно выглядеть:
        while (Detail.Details.Any(d => d.State != ProcessingStates.Completed))
        {
            
            stopWatch.Reset();
            while (stopWatch.ElapsedMilliseconds < _timeSlice) { }
            Log();
        }
    */
    public void StartProduction()
    {
        var stopwatch = new Stopwatch();
        
        while (Detail.Details.Any(d => d.State != ProcessingStates.Completed))
        {
            var tasks = new List<Task<Detail>>();
            foreach (var detail in Detail.Details)
            {
                if (detail.IsAvailabe() && detail.State == ProcessingStates.Queued)
                {
                    detail.Preprocess();
                    tasks.Add(detail.Process());
                }
            }
            
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < _timeSlice) { }
            foreach (var task in tasks)
            {
                var detail = task.Result;
                detail.Postprocess();
            }
            Log();
            
        }
    }

    private void Log()
    {
        foreach (var machine in Machine.Machines)
            Console.WriteLine(
                $"Станок {machine.Name}. Обрабатываемые детали: {string.Join(", ", machine.DetailNames)}");

        Console.WriteLine();

        foreach (var detail in Detail.Details)
            Console.WriteLine(
                $"Деталь {detail.Name}. Состояние: {detail.State}. Станок: {detail.TargetMachineName}");

        Console.WriteLine();
    }

    private readonly int _timeSlice;
}