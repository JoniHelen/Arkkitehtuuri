using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text m_Text;

    private Command moveLeftCommand;
    private Command moveRightCommand;
    private Command moveForwardCommand;
    private Command moveBackCommand;

    private Stack<Command> m_Commands = new();

    private Command m_LatestUndoneCommand;

    private State m_CurrentState;

    private State m_StandingState;
    private State m_JumpingState;
    private State m_CrouchingState;
    private State m_AbilityState;

    private bool m_IsJumping;
    private bool m_IsCrouching;
    private bool m_HasPowerup;
    
    private void Start()
    {
        moveLeftCommand = new MoveCommand(Vector3.left);
        moveRightCommand = new MoveCommand(Vector3.right);
        moveForwardCommand = new MoveCommand(Vector3.forward);
        moveBackCommand = new MoveCommand(Vector3.back);

        m_StandingState = new State("STANDING", () => !m_IsJumping && !m_IsCrouching);
        m_JumpingState = new State("JUMPING", () => m_IsJumping && !m_IsCrouching);
        m_CrouchingState = new State("CROUCHING", () => m_IsCrouching && !m_IsJumping);
        m_AbilityState = new State("ABILITY", () => m_HasPowerup);

        m_StandingState.AddTransitions(m_JumpingState, m_CrouchingState, m_AbilityState);
        m_JumpingState.AddTransitions(m_StandingState, m_CrouchingState, m_AbilityState);
        m_CrouchingState.AddTransitions(m_StandingState, m_JumpingState, m_AbilityState);
        m_AbilityState.AddTransitions(m_StandingState, m_JumpingState, m_CrouchingState);

        m_CurrentState = m_StandingState;
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!m_IsCrouching)
                StartCoroutine(Jump());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (!m_IsJumping && !m_IsCrouching)
            {
                m_IsCrouching = true;
                transform.localScale = new Vector3(1, 0.5f, 1);
            }
            else if (!m_IsJumping)
            {
                m_IsCrouching = false;
                transform.localScale = Vector3.one;
            }
        }

        m_CurrentState = m_CurrentState.GetFirstTransition();

        m_Text.text = m_CurrentState.Name;
    }

    private IEnumerator Jump()
    {
        m_IsJumping = true;
        float elapsedTime = 0;

        while (elapsedTime < 2) 
        {
            transform.position = new Vector3(transform.position.x, -Mathf.Pow(elapsedTime - 1, 2) + 2, transform.position.z);
            elapsedTime += Time.deltaTime * 4;
            yield return null;
        }

        transform.position = new Vector3(transform.position.x, 1, transform.position.z);
        m_IsJumping = false;
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