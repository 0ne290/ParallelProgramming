namespace ParallelProgramming;

public class Machine
{
    public Machine(string name, int capacity)
    {
        Name = name;
        Capacity = capacity;
    }
    
    public void ProcessADetail()
    {
        
    }
    
    public string Name { get; }
    
    public int Capacity { get; }
}