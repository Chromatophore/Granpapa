using UnityEngine;
using System.Collections;

public class blink : MonoBehaviour {

	public float minBlink = 3f;
	public float maxBlink = 6f;

	public Animator animator;

	void Start () {
		StartCoroutine(blinkCo());
	}

	IEnumerator blinkCo()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(minBlink,maxBlink));
			animator.Play("blink", -1, 0f);
		}
	}
}
