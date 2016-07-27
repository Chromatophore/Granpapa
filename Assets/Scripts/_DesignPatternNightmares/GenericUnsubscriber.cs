using System.Collections.Generic;

// Class used to unsubscribe observers from our updates:
// (Returned by a subsribe method as an IDisposable)
public class GenericUnsubscriber<T> : IDisposable
{
	private ICollection<IObserver<T>> observers;
	private IObserver<T> observer;
	private bool addMode = false;

	public GenericUnsubscriber(ICollection<IObserver<T>> observers, IObserver<T> observer, bool addMode = false)
	{
		this.observers = observers;
		this.observer = observer;
	}

	// for IDisposable:
	public void Dispose() 
	{
		if (addMode)
		{
			if (!(observer == null))
				observers.Add(observer);
		}
		else
		{
			if (!(observer == null))
				observers.Remove(observer);
		}
	}
}