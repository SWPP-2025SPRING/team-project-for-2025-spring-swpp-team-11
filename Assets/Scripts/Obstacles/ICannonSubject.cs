public interface ICannonSubject
{
    void RegisterObserver(ICannonObserver o);
    void UnregisterObserver(ICannonObserver o);
    void NotifyObservers();
}
