using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct TempoData
{
	public TempoData(bool syncBeat, float speedFactor)
	{
		this.syncBeat = syncBeat;
		this.speedFactor = speedFactor;
	}
	public bool syncBeat;
	public float speedFactor;
}

public class Tempo : MonoBehaviour, IObservable<TempoData>, IObserver<BeatData>
{
	private static Tempo SingleRef = null;

	public static void SetTempo(int bpm)
	{
		SingleRef.SetTempoNonStatic(bpm);
	}
	public static IDisposable SSubscribe(IObserver<TempoData> observer)
	{
		return SingleRef.Subscribe(observer);
	}


	public static void RegisterBeatBox(IObservable<BeatData> beatBox)
	{
		SingleRef.RegisterBeatBoxNonStatic(beatBox);
	}




	private int lastBPM = 0;
	private float animSpeedScale = 1f;

	// Use this for initialization
	void Awake()
	{
		SingleRef = this;
	}

	public void SetTempoNonStatic(int bpm)
	{
		if (bpm == lastBPM)
			return;

		float tempoScale = bpm;
		tempoScale /= 120f;

		animSpeedScale = tempoScale;

		//Debug.Log("anim scale set to " + animSpeedScale);

		UpdateObservers(false);
	}

	public void UpdateObservers(bool syncBeat)
	{
		if (TempoDataObservers == null)
			return;

		var data = new TempoData(syncBeat, animSpeedScale);
		foreach (var observer in TempoDataObservers)
		{
			observer.OnNext(data);
		}
	}

	private IDisposable unsubscriber;
	public void RegisterBeatBoxNonStatic(IObservable<BeatData> beatBox)
	{
		if (unsubscriber != null)
			unsubscriber.Dispose();
			
		unsubscriber = beatBox.Subscribe(this);
	}
	

	private HashSet<IObserver<TempoData>> TempoDataObservers;
	public IDisposable Subscribe(IObserver<TempoData> observer)
	{
		// Generate list for game setting observers
		if (TempoDataObservers == null)
			TempoDataObservers = new HashSet<IObserver<TempoData>>();

		TempoDataObservers.Add(observer);

		// get instant data:
		observer.OnNext(new TempoData(false, animSpeedScale));

		// Now, return an IDisposable implimentation to whatever called us so that it can unsubscribe from our updates:
		return new GenericUnsubscriber<TempoData>(TempoDataObservers, observer);
	}

	// IObserver implimentation:
	public void OnCompleted()
	{

	}
	public void OnError(System.Exception exception)
	{

	}
	public void OnNext(BeatData data)
	{
		if (data.isFirstBeatOfBar)
		{
			UpdateObservers(true);
		}
	}
}
