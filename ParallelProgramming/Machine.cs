namespace ParallelProgramming;

public class Machine
{
    public Machine(string name, int capacity)
    {
        Name = name;
        _semaphore = new Semaphore(capacity, capacity);
    }
    
    public void ProcessADetail()
    {
        _semaphore.WaitOne();

        _semaphore.Release();
    }
    
    public string Name { get; }

    private Semaphore _semaphore;
}