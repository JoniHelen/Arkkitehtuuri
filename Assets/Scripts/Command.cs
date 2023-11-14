using UnityEngine;

public abstract class Command
{
    public abstract void Execute(MonoBehaviour obj);
    public abstract void Undo(MonoBehaviour obj);
}