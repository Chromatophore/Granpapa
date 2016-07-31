using UnityEngine;
using System.Collections;

public class PhysPop : MonoBehaviour {

	public float tMax = 6f;
	private bool activeOnce = false;

	private Vector3 gravity = new Vector3(0f, -9.8f, 0f);
	private Vector3 velocity = new Vector3(0f, 10f, 0f);

	public void Pop(Transform newParent = null)
	{
		if (activeOnce)
			return;
		transform.parent = newParent;
		activeOnce = true;

		velocity = transform.up * 10;
		velocity.x += 4f;

		StartCoroutine(BallisticFall());

	}

	IEnumerator BallisticFall()
	{
		float timeSoFar = 0;

		while (timeSoFar < tMax)
		{
			float delta = Time.deltaTime;

			velocity += delta * gravity;

			transform.localPosition += delta * velocity;

			float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
			angle -= 90f;
			var newRot = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0,0,angle),90f * delta);
			transform.rotation = newRot;


			timeSoFar += delta;
			yield return null;
		}

		Destroy(gameObject);
	}
}
