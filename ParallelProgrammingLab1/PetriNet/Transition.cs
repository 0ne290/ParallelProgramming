namespace ParallelProgrammingLab1.PetriNet;

public class Transition
{
    public Transition(IEnumerable<Place> inputPlaces, IEnumerable<Place> outputPlaces)
    {
        _inputPlaces = inputPlaces;
        _outputPlaces = outputPlaces;
    }

    public void Execute()
    {
        foreach (var place in _inputPlaces)
            place.Unload();
        
        foreach (var place in _outputPlaces)
            place.Load();
    }

    public bool IsAvailable()
    {
        foreach (var place in _inputPlaces)
            if (!place.IsAvailable())
                return false;

        return true;
    }
    
    private readonly IEnumerable<Place> _inputPlaces;
    
    private readonly IEnumerable<Place> _outputPlaces;
}