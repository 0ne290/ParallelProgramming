using System.Diagnostics;
using ParallelProgrammingLab1.Enums;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace ParallelProgrammingLab1;

public class Workshop
{
    public Workshop(int timeSlice) => _timeSlice = timeSlice;
    
    public void StartProduction(PlanningAlgorithms planningAlgorithm)
    {
        switch (planningAlgorithm)
        {
            case PlanningAlgorithms.SjfPreemptiveAbsolutePriority:
                PreemptiveMultitasking();
                break;
            case PlanningAlgorithms.SjfNonpreemptive:
                NonPreemptiveMultitasking();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(planningAlgorithm), planningAlgorithm,
                    "Algorithm is not supported");
        }
    }

    private void PreemptiveMultitasking()
    {
        Comparison<Detail> comparator = (x, y) =>
        {
            if (x.GetRestOfCpuBurst() < y.GetRestOfCpuBurst())
                return -1;
        
            return x.GetRestOfCpuBurst() > y.GetRestOfCpuBurst() ? 1 : 0;
        };
        var details = new List<Detail>(Detail.Details);
        details.Sort(comparator);
        var stopwatch = new Stopwatch();
        var logTask = Task.CompletedTask;
        
        while (details.Any(d => d.State != ProcessingStates.Completed))
        {
            var tasks = new List<Task<Detail>>();
            foreach (var detail in details.Where(detail => detail.IsAvailabe()))
            {
                detail.Preprocess();
                tasks.Add(detail.Process());
            }
            
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < _timeSlice) { }
            
            logTask.Wait();
            logTask = Log();
            
            foreach (var detail in Task.WhenAll(tasks).Result)
                detail.Postprocess();

            details.Sort(comparator);
        }
        
        _writer.Dispose();
    }
    
    private void NonPreemptiveMultitasking()
    {
        var details = new List<Detail>(Detail.Details);
        details.Sort((x, y) =>
        {
            if (x.CpuBurst < y.CpuBurst)
                return -1;

            return x.CpuBurst > y.CpuBurst ? 1 : 0;
        });
        var stopwatch = new Stopwatch();
        var logTask = Task.CompletedTask;
        
        while (details.Any(d => d.State != ProcessingStates.Completed))
        {
            var tasks = new List<Task<Detail>>();
            foreach (var detail in details.Where(detail => detail.IsAvailabe()))
            {
                detail.Preprocess();
                tasks.Add(detail.Process());
            }
            
            stopwatch.Restart();
            while (stopwatch.ElapsedMilliseconds < _timeSlice) { }
            
            logTask.Wait();
            logTask = Log();
            
            foreach (var detail in Task.WhenAll(tasks).Result)
                detail.Postprocess();
        }
        
        _writer.Dispose();
    }

    private async Task Log()
    {
        foreach (var machine in Machine.Machines)
            await _writer.WriteLineAsync(
                $"Станок {machine.Name}. Обрабатываемые детали: {string.Join(", ", machine.DetailNames)}");

        await _writer.WriteLineAsync();

        foreach (var detail in Detail.Details)
            await _writer.WriteLineAsync(
                $"Деталь {detail.Name}. Состояние: {detail.State}. Станок: {detail.TargetMachineName}");

        await _writer.WriteLineAsync();
    }

    private readonly int _timeSlice;

    private readonly StreamWriter _writer = new("Output.txt", false);
}
