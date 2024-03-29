using ParallelProgrammingLab1.PetriNet;

namespace ParallelProgrammingLab1.PetriNetSemaphore;

public class HoldingTransition
{
    public HoldingTransition(Place semaphorePlace, Place innerPlace)
    {
        _semaphorePlace = semaphorePlace;
        _innerPlace = innerPlace;
    }
    
    public void Execute()
    {
        _innerPlace.Tokens++;
        _semaphorePlace.Tokens--;
    }

    public int GetCapacity() => _semaphorePlace.Tokens;
    
    private readonly Place _semaphorePlace;
    
    private readonly Place _innerPlace;
}