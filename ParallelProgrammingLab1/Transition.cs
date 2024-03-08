namespace ParallelProgrammingLab1;

public class Transition
{
    public void AddInputPlace(Place place) => _inputPlaces.Add(place);
    
    public void AddOutputPlace(Place place) => _outputPlaces.Add(place);
    
    public bool IsAvailabe()
    {
        foreach (var place in _inputPlaces)
        {
            if (place.Tokens < 1)
                return false;
        }

        return true;
    }

    public void Execute()
    {
        foreach (var place in _inputPlaces)
            place.Tokens--;
        
        foreach (var place in _outputPlaces)
            place.Tokens++;
    }
    
    private readonly List<Place> _inputPlaces = new();
    
    private readonly List<Place> _outputPlaces = new();
}