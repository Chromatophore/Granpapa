using UnityEngine;
using System.Collections;

public class finale : MonoBehaviour, IObserver<BeatData>
{

	public PageSequence ps;
	private IDisposable unsubscriber;

	public GameObject mario;
	public GameObject bowser;

	public GameObject FirebreathPrefab;
	public GameObject longFrebreathPrefab;

	public Animator finaleAnimator;


	public Animator bowserAnim;
	public Animator marioAnim;

	//private int startValue = 176;
	//private int startValue = 8;
	private int startValue = -24;
	
	void Start ()
	{
		
	}
	
	void Update ()
	{
	
	}

	private bool activated = false;
	public void Activate()
	{
		if (activated)
			return;
		unsubscriber = ps.Subscribe(this);
		activated = true;
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
		int finaleNumber = data.beatNumber - startValue;

		Debug.Log("Finale Beat " + finaleNumber + " ± "  + data);

		Vector3 loFire = mario.transform.position;
		Vector3 hiFire = loFire;
		Vector3 realloFire = loFire;

		loFire.y += 1.5f;
		hiFire.y += 3.5f;
		realloFire.y += 0f;

		switch (finaleNumber)
		{
			case 0:
			case 14:
			case 20:
			case 27:
				Instantiate(FirebreathPrefab, hiFire, Quaternion.identity);
				break;
			case 8:
			case 17:
			case 25:
				Instantiate(FirebreathPrefab, loFire, Quaternion.identity);
				break;
			case 38:
				Instantiate(longFrebreathPrefab, realloFire, Quaternion.identity);
				break;
			case 28:
				bowser.SetActive(true);
				break;
			case 32:
				finaleAnimator.Play("finaleA");
				break;
			case 40:
				finaleAnimator.Play("finaleB");
				break;
			case 46:
				Instantiate(longFrebreathPrefab, loFire, Quaternion.identity);
				break;
			case 48:
				Instantiate(longFrebreathPrefab, realloFire, Quaternion.identity);
				finaleAnimator.Play("finaleC");
				break;
			case 56:
				finaleAnimator.Play("finaleD");
				Instantiate(longFrebreathPrefab, realloFire, Quaternion.identity);
				break;
			case 64:
				finaleAnimator.Play("finaleE");
				break;
			default:
				break;
		}

		// 176 is the first beat of the finale sequence
	}

	public void Bowser(string animName)
	{
		bowserAnim.CrossFade(animName, 0.1f, -1, 0f);
	}
}
