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
        if (GetCapacity() < 1)
            throw new Exception("Bad...");
        
        _innerPlace.Tokens++;
        _semaphorePlace.Tokens--;
    }

    public int GetCapacity() => _semaphorePlace.Tokens;
    
    private Place _semaphorePlace;
    
    private Place _innerPlace;
}