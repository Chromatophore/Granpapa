using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TempoObserver : MonoBehaviour, IObserver<TempoData>
{
	[SerializeField]
	private Animator animator;

	[SerializeField]
	private bool hasJig = true;

	[SerializeField]
	private string beatName = "BeatA";

	private float lastSpeed = 0f;

	private IDisposable unsubscriber = null;

	void Start()
	{
		if (animator == null)
			animator = GetComponent<Animator>();

		if (animator != null)
		{
			unsubscriber = Tempo.SSubscribe(this);
		}
	}

	void OnDestroy()
	{
		if (unsubscriber != null)
			unsubscriber.Dispose();
	}

	// IObserver implimentation:
	public void OnCompleted()
	{

	}
	public void OnError(System.Exception exception)
	{

	}
	public void OnNext(TempoData data)
	{
		if (hasJig && data.syncBeat)
		{
			animator.CrossFade(beatName, 0.25f, -1, 0f);
			//animator.Play(beatName, -1, 0f);
		}

		if (data.speedFactor != lastSpeed)
		{
			lastSpeed = data.speedFactor;

			animator.speed = lastSpeed;
		}
	}
}