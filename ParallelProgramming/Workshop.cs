using System.Timers;
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
        var timer = new System.Timers.Timer(_timeSlice);
        timer.Elapsed += OnTimedEvent;
        timer.Start();

        while (Detail.Details.Any(d => d.State != ProcessingStates.Completed))
        {
            if (Detail.DetailsInQueue.Count < 1)
                continue;
            var detail = Detail.DetailsInQueue.Dequeue();
            detail.Process();
        }
        
        while (Detail.Details.Any(d => d.State != ProcessingStates.Completed))
        {
            
            stopWatch.Restart();
            while (stopWatch.ElapsedMilliseconds < _timeSlice) { }
            Log();
        }
        
        timer.Stop();

        while (_sync) { }// Можно было бы заменить это на AutoResetEvent или добавить к спин-локу код, отдающий квант времени этого потока другому потоку (Thread.Curent.Join(100) или Thread.Sleep(0))
        
        timer.Elapsed -= OnTimedEvent;
        timer.Close();
        timer.Dispose();
    }
    
    private void OnTimedEvent(object? source, ElapsedEventArgs e)
    {
        lock (_ss)
        {
            _sync = true;

            foreach (var machine in Machine.Machines)
                Console.WriteLine(
                    $"Станок {machine.Name}. Обрабатываемые детали: {string.Join(", ", machine.DetailNames)}");

            Console.WriteLine();

            foreach (var detail in Detail.Details)
                Console.WriteLine(
                    $"Деталь {detail.Name}. Состояние: {detail.State}. Станок: {detail.TargetMachineName}");

            Console.WriteLine();

            _sync = false;
        }
    }

    private int _timeSlice;

    private bool _sync;

    private object _ss = new();
}