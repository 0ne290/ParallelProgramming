namespace ParallelProgramming;

public class Workshop
{
    public Workshop(IEnumerable<Machine> machines, IEnumerable<Detail> details)
    {
        foreach (var machine in machines)
            _machines.Add(machine.Name, machine);

        _details = details;
    }
    
    private IDictionary<string, Machine> _machines = new Dictionary<string, Machine>();

    private IEnumerable<Detail> _details;
}