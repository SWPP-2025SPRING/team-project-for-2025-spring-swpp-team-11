using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public bool canControlPlayer = true;
    
    public UnityEvent<InputAction.CallbackContext> onMoveEvent;
    public UnityEvent<InputAction.CallbackContext> onJumpEvent;
    public UnityEvent<InputAction.CallbackContext> onShotEvent;
    
    
    // UI 관련
    public UnityEvent<InputAction.CallbackContext> onEnterEvent;
    public UnityEvent<InputAction.CallbackContext> onHorizontalEvent;
    public UnityEvent<InputAction.CallbackContext> onVerticalEvent;
    public UnityEvent<InputAction.CallbackContext> onEscapeEvent;

    private bool CanControlPlayerInGame()
    {
        return canControlPlayer;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!CanControlPlayerInGame()) return;
        onMoveEvent.Invoke(context);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!CanControlPlayerInGame()) return;
        onJumpEvent.Invoke(context);
    }

    public void OnShot(InputAction.CallbackContext context)
    {
        if (!CanControlPlayerInGame()) return;
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
