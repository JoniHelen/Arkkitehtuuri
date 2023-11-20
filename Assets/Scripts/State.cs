using System;

public class State
{
    public readonly string Name;
    private readonly Func<bool> Transition;
    private State[] TransitionableStates;

    public bool CanTransition => Transition();

    public State(string name, Func<bool> transition) 
    {
        Name = name;
        Transition = transition;
    }

    public void AddTransitions(params State[] states)
    {
        TransitionableStates = states;
    }

    public State GetFirstTransition()
    {
        foreach (var state in TransitionableStates)
            if (state.CanTransition)
                return state;

        return this;
    }
}