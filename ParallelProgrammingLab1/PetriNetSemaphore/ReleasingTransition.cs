namespace ParallelProgrammingLab1.PetriNetSemaphore;

public class ReleasingTransition
{
    public ReleasingTransition(Place innerPlace, Place semaphorePlace)
    {
        _innerPlace = innerPlace;
        _semaphorePlace = semaphorePlace;
    }
    
    public void Execute()
    {
        _innerPlace.Tokens--;
        _semaphorePlace.Tokens++;
    }
    
    private readonly Place _innerPlace;
    
    private readonly Place _semaphorePlace;
}