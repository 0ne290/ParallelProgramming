using ParallelProgramming.Enums;

namespace ParallelProgramming;

public class DetailProcessing
{
    public DetailProcessing(Machine machine, int timeSlice)
    {
        _machine = machine;
        _timeSlice = timeSlice;
        State = ProcessingStates.Queued;
    }

    public void Iterate()
    {
        if (_iterationsNumber < _detail.CpuBurst)
        {
            State = ProcessingStates.InProgress;
            
            var task = Task.Run(() => _machine.Mill(_timeSlice, _detail.Name));
            
            task.Wait();
            
            State = ProcessingStates.Queued;
        }
        else
            State = ProcessingStates.Completed;
    }

    public Detail Detail
    {
        get => _detail;
        set
        {
            _detail = value;
            _iterationsNumber = 0;
        }
    }
    
    public ProcessingStates State { get; private set; }

    private Detail _detail;

    private int _iterationsNumber;

    private Machine _machine;

    private int _timeSlice;
}