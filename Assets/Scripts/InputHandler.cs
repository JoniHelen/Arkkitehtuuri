using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Command moveLeftCommand;
    private Command moveRightCommand;
    private Command moveForwardCommand;
    private Command moveBackCommand;

    private Stack<Command> m_Commands = new();

    private Command m_LatestUndoneCommand;
    
    private void Start()
    {
        moveLeftCommand = new MoveCommand(Vector3.left);
        moveRightCommand = new MoveCommand(Vector3.right);
        moveForwardCommand = new MoveCommand(Vector3.forward);
        moveBackCommand = new MoveCommand(Vector3.back);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            ExecuteCommand(moveForwardCommand);
        
        if (Input.GetKeyDown(KeyCode.A))
            ExecuteCommand(moveLeftCommand);
        
        if (Input.GetKeyDown(KeyCode.S))
            ExecuteCommand(moveBackCommand);
        
        if (Input.GetKeyDown(KeyCode.D))
            ExecuteCommand(moveRightCommand);
        
        if (Input.GetKeyDown(KeyCode.Z))
            UndoLastCommand();

        if (Input.GetKeyDown(KeyCode.Y))
        {
            ExecuteCommand(m_LatestUndoneCommand);
            m_LatestUndoneCommand = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin") || other.CompareTag("Enemy"))
            Destroy(other.gameObject);
    }

    private void ExecuteCommand([CanBeNull] Command command)
    {
        if (command == null) return;
        
        command.Execute(this);
        m_Commands.Push(command);
    }

    private void UndoLastCommand()
    {
        if (m_Commands.TryPop(out Command command))
        {
            command.Undo(this);
            m_LatestUndoneCommand = command;
        }
    }
}