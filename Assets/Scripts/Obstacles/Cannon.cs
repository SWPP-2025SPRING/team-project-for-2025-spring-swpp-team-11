using UnityEngine;
using System.Collections.Generic;

public class Cannon : MonoBehaviour, ICannonSubject
{
    public float fireInterval = 3f;

    private float _timer = 0f;
    private List<ICannonObserver> observers = new List<ICannonObserver>();

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= fireInterval)
        {
            _timer = 0f;
            NotifyObservers();
        }
    }

    public void RegisterObserver(ICannonObserver o)
    {
        if (!observers.Contains(o))
            observers.Add(o);
    }

    public void UnregisterObserver(ICannonObserver o)
    {
        if (observers.Contains(o))
            observers.Remove(o);
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
            observer.OnCannonFired();
    }
}
