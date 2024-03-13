namespace ParallelProgrammingLab1.PetriNetSemaphore;

public class HoldingTransition
{
    public void Execute()
    {
        if (!IsAvailable())
            throw new Exception("Bad...");
        
        _innerPlace.Tokens++;
        _semaphorePlace.Tokens--;
    }

    public bool IsAvailable() => _semaphorePlace.Tokens > 0;
    
    private Place _semaphorePlace;
    
    private Place _innerPlace;
}