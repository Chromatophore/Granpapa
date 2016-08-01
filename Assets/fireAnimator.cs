using UnityEngine;
using System.Collections;

public class fireAnimator : MonoBehaviour {

	public float rate = 0.1f;
	public float tempo = 0.6666f;
	// Use this for initialization
	void Start () {
		StartCoroutine(Flip());
		Destroy(gameObject,30f);
	}

	void Update()
	{
		float speedPerSecond = 6f / tempo;
		transform.localPosition -= new Vector3(speedPerSecond * Time.deltaTime, 0f, 0f);
	}

	IEnumerator Flip()
	{
		while (true)
		{
			var scale = transform.localScale;
			scale.y *= -1;
			transform.localScale = scale;
			yield return new WaitForSeconds(rate);
		}
	}

}
