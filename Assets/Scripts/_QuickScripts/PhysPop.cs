using UnityEngine;
using System.Collections;

public class PhysPop : MonoBehaviour
{

	public float delay = 0.25f;
	public float tMax = 6f;
	private bool activeOnce = false;

	private Vector3 gravity = new Vector3(0f, -18.8f, 0f);
	private Vector3 velocity = new Vector3(0f, 10f, 0f);

	public void Pop(Transform newParent = null)
	{
		if (activeOnce)
			return;
		
		activeOnce = true;


		StartCoroutine(BallisticFall(newParent));

	}

	IEnumerator BallisticFall(Transform newParent)
	{
		yield return new WaitForSeconds(delay);

		transform.parent = newParent;
		velocity = transform.up * 10;
		velocity.x += 6f;

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
