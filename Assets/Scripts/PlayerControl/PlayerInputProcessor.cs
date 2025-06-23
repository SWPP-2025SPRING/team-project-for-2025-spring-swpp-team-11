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
    public UnityEvent respawnEvent;
    public UnityEvent guideLineEvent;
    public Vector2 MoveInput { get; private set; }

    private void Start()
    {
        var inputManager = GameManager.Instance.InputManager;
        
        inputManager.onMoveEvent.AddListener(OnMove);
        inputManager.onJumpEvent.AddListener(OnJump);
        inputManager.onShotEvent.AddListener(OnShot);
        inputManager.onEscapeEvent.AddListener(OnEscape);
        inputManager.onRespawnEvent.AddListener(OnRespawn);
        inputManager.onGuideLineEvent.AddListener(OnGuideLine);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
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

    public void OnRespawn(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            respawnEvent.Invoke();
    }

    public void OnGuideLine(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started || context.phase == InputActionPhase.Canceled)
            guideLineEvent.Invoke();
    }
}
