using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputProcessor : MonoBehaviour
{
    public UnityEvent jumpEvent;
    public UnityEvent shotEvent;
    public UnityEvent releaseEvent;
    public UnityEvent escapeEvent;
    public Vector2 MoveInput { get; private set; }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("MOVE");
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) 
            jumpEvent.Invoke();
    }
    
    public void OnShot(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            shotEvent.Invoke();
        if (context.phase == InputActionPhase.Canceled)
            releaseEvent.Invoke();
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
            escapeEvent.Invoke();
    }
}
