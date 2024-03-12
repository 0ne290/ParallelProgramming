namespace ParallelProgrammingLab1;

public class PetriSubnet
{
    private PetriSubnet(Transition[] transitions, Resourse[] resourses)
    {
        _transitions = transitions;
        _resourses = resourses;
    }

    public static PetriSubnet CreatePetriSubnetForSjfNonpremptive(Resourse[] resourses)
    {
        var transitions = new Transition[resourses.Length * 2];

        var places = new Place[resourses.Length * 2];
        places[0] = new Place(1);
        for (var i = 1; i < places.Length; i++)
        {
            places[i] = new Place();
        }

        for (var i = 0; i < transitions.Length; i += 2)
        {
            transitions[i] = new Transition();
            transitions[i].AddInputPlace(places[i]);
            transitions[i].AddInputPlace(Resourse.ResoursePlaces[resourses[i / 2]]);
            transitions[i].AddOutputPlace(places[i + 1]);
        }
        
        for (var i = 1; i < transitions.Length - 1; i += 2)
        {
            transitions[i] = new Transition();
            transitions[i].AddInputPlace(places[i]);
            transitions[i].AddOutputPlace(Resourse.ResoursePlaces[resourses[i / 2]]);
            transitions[i].AddOutputPlace(places[i + 1]);
        }

        var j = transitions.Length - 1;
        transitions[j] = new Transition();
        transitions[j].AddInputPlace(places[j]);
        transitions[j].AddOutputPlace(Resourse.ResoursePlaces[resourses[j / 2]]);
        transitions[j].AddOutputPlace(places[0]);

        return new PetriSubnet(transitions, resourses);
    }
    
    public static PetriSubnet CreatePetriSubnetForSjfPremptiveAbsolutePriority(int cpuBurst, Resourse[] resourses)
    {
        var transitions = new Transition[resourses.Length * 2];

        var places = new Place[resourses.Length * 2];
        places[0] = new Place(cpuBurst);
        for (var i = 1; i < places.Length; i++)
        {
            places[i] = new Place();
        }

        for (var i = 0; i < transitions.Length; i += 2)
        {
            transitions[i] = new Transition(cpuBurst);
            transitions[i].AddInputPlace(places[i]);
            transitions[i].AddInputPlace(Resourse.ResoursePlaces[resourses[i / 2]]);
            transitions[i].AddOutputPlace(places[i + 1]);
            transitions[i].CurrentCheck = transitions[i].SjfPremptiveAbsolutePriorityCheck;
        }
        
        for (var i = 1; i < transitions.Length - 1; i += 2)
        {
            transitions[i] = new Transition();
            transitions[i].AddInputPlace(places[i]);
            transitions[i].AddOutputPlace(Resourse.ResoursePlaces[resourses[i / 2]]);
            transitions[i].AddOutputPlace(places[i + 1]);
        }

        var j = transitions.Length - 1;
        transitions[j] = new Transition();
        transitions[j].AddInputPlace(places[j]);
        transitions[j].AddOutputPlace(Resourse.ResoursePlaces[resourses[j / 2]]);
        transitions[j].AddOutputPlace(places[0]);

        return new PetriSubnet(transitions, resourses);
    }
    
    public bool ExecuteTransition()
    {
        _transitions[_indexTransitions].Execute();

        if (_indexTransitions == _transitions.Length - 1)
        {
            _indexTransitions = 0;
            return true;
        }

        _indexTransitions++;
        return false;
    }

    public bool TransitionIsAvailable() => _transitions[_indexTransitions].IsAvailabe();

    public bool TransitionIsHolding() => _indexTransitions % 2 == 0;

    public void HoldResourse(string name) => _resourses[_indexTransitions / 2].Hold(name);
    
    public void RealeseResourse(string name) => _resourses[(_indexTransitions - 1) / 2].Realese(name);

    private Transition[] _transitions;

    private Resourse[] _resourses;
    
    private int _indexTransitions;
}