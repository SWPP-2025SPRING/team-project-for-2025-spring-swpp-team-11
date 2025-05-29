using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public UnityEvent<InputAction.CallbackContext> onMoveEvent;
    public UnityEvent<InputAction.CallbackContext> onJumpEvent;
    public UnityEvent<InputAction.CallbackContext> onShotEvent;
    public UnityEvent<InputAction.CallbackContext> onEscapeEvent;
    
    // UI 관련
    public UnityEvent<InputAction.CallbackContext> onEnterEvent;
    public UnityEvent<InputAction.CallbackContext> onHorizontalEvent;
    public UnityEvent<InputAction.CallbackContext> onVerticalEvent;

    public void OnMove(InputAction.CallbackContext context)
    {
        onMoveEvent.Invoke(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        onJumpEvent.Invoke(context);
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        onShotEvent.Invoke(context);
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        onEscapeEvent.Invoke(context);  
    }

    public void OnEnter(InputAction.CallbackContext context)
    {
        onEnterEvent.Invoke(context);
    }

    public void OnHorizontal(InputAction.CallbackContext context)
    {
        onHorizontalEvent.Invoke(context);
    }

    public void OnVertical(InputAction.CallbackContext context)
    {
        onVerticalEvent.Invoke(context);
    }
}
