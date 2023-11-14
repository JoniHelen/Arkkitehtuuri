using UnityEngine;

public class MoveCommand : Command
{
    private Vector3 m_Translation;

    public MoveCommand(Vector3 translation)
    {
        m_Translation = translation;
    }
    
    public override void Execute(MonoBehaviour obj)
    {
        obj.transform.Translate(m_Translation);
    }

    public override void Undo(MonoBehaviour obj)
    {
        obj.transform.Translate(-m_Translation);
    }
}
