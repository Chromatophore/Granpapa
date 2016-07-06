using System.Collections.Generic;

// Class used to unsubscribe observers from our updates:
// (Returned by a subsribe method as an IDisposable)
public class GenericUnsubscriber<T> : IDisposable
{
	private ICollection<IObserver<T>> observers;
	private IObserver<T> observer;

	public GenericUnsubscriber(ICollection<IObserver<T>> observers, IObserver<T> observer)
	{
		this.observers = observers;
		this.observer = observer;
	}

	// for IDisposable:
	public void Dispose() 
	{
		if (!(observer == null))
			observers.Remove(observer);
	}
}