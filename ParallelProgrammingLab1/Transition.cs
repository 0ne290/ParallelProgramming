namespace ParallelProgrammingLab1;

public class Transition
{
    public Transition(int cpuBurst = 0)
    {
        SjfNonpremptiveCheck = () =>
        {
            foreach (var place in _inputPlaces)
            {
                if (place.Tokens < 1)
                    return false;
            }

            return true;
        };

        SjfPremptiveAbsolutePriorityCheck = () =>
            _inputPlaces[0].Tokens == cpuBurst && _inputPlaces[1].Tokens > 0 && _outputPlaces[0].Tokens == 0;

        CurrentCheck = SjfNonpremptiveCheck;
    }
    
    public void AddInputPlace(Place place) => _inputPlaces.Add(place);
    
    public void AddOutputPlace(Place place) => _outputPlaces.Add(place);

    public bool IsAvailabe() => CurrentCheck();

    public void Execute()
    {
        foreach (var place in _inputPlaces)
            place.Tokens--;
        
        foreach (var place in _outputPlaces)
            place.Tokens++;
    }
    
    public Func<bool> SjfNonpremptiveCheck { get; }
    
    public Func<bool> SjfPremptiveAbsolutePriorityCheck { get; }// Это нужно устанавливать только для занимающих переходов в случае алгоритма SjfPremptiveAbsolutePriorityCheck
    
    public Func<bool> CurrentCheck { get; set; }
    
    private readonly List<Place> _inputPlaces = new();
    
    private readonly List<Place> _outputPlaces = new();
}