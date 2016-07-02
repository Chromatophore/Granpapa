using System;
 
public interface IObserver<TType> {
	void OnCompleted();
	void OnError(Exception exception);
	void OnNext(TType value);
}

public interface IObservable<TType> {
	IDisposable Subscribe(IObserver<TType> observer);
}

public interface IDisposable {
	void Dispose();
}