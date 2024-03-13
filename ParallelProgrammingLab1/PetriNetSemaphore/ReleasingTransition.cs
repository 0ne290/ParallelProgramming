namespace ParallelProgrammingLab1.PetriNetSemaphore;

public class ReleasingTransition
{
    public void Execute()
    {
        if (_innerPlace.Tokens < 1)
            throw new Exception("...daB");
        
        _innerPlace.Tokens--;
        _semaphorePlace.Tokens++;
    }
    
    private Place _innerPlace;
    
    private Place _semaphorePlace;
}