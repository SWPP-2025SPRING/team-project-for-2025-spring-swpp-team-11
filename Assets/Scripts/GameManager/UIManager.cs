using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour, IUIWindowSubject
{
    public List<IUIWindowObserver> uiWindows;

    private UIWindowVisitor _currentVisitor;
    
    private UIWindowEnterVisitor _enterVisitor;
    private UIWindowHorizontalVisitor _horizontalVisitor;
    private UIWindowVerticalVisitor _verticalVisitor;

    private void Start()
    {
        uiWindows = new List<IUIWindowObserver>();
        
        _enterVisitor = new UIWindowEnterVisitor();
        _horizontalVisitor = new UIWindowHorizontalVisitor();
        _verticalVisitor = new UIWindowVerticalVisitor();

        var inputManager = GameManager.Instance.InputManager;
        
        inputManager.onEnterEvent.AddListener(OnEnter);
        inputManager.onHorizontalEvent.AddListener(OnHorizontal);
        inputManager.onVerticalEvent.AddListener(OnVertical);

        _currentVisitor = _enterVisitor;
    }
    
    public void RegisterObserver(IUIWindowObserver observer)
    {
        uiWindows.Add(observer);
    }

    public void UnregisterObserver(IUIWindowObserver observer)
    {
        uiWindows.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var window in uiWindows)
        {
            window.ObserverUpdate(_currentVisitor);
        }
    }
    
    public void OnEnter(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _currentVisitor = _enterVisitor;
            NotifyObservers();
        }
    }
    
    public void OnHorizontal(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            float inputX = context.ReadValue<Vector2>().x;

            if (inputX == 0) return;
            
            _horizontalVisitor.SetDirection(inputX < 0 ? -1 : 1);

            _currentVisitor = _horizontalVisitor;
            NotifyObservers();
        }
    }
    
    public void OnVertical(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            float inputY = context.ReadValue<Vector2>().y;

            if (inputY == 0) return;
            
            _verticalVisitor.SetDirection(inputY < 0 ? -1 : 1);

            _currentVisitor = _verticalVisitor;
            NotifyObservers();
        }
    }
}
