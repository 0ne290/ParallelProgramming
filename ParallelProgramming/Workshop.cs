namespace ParallelProgramming;

public class Workshop
{
    public Workshop(IEnumerable<Machine> machines) => _machines = machines;
    
    private IEnumerable<Machine> _machines;
}