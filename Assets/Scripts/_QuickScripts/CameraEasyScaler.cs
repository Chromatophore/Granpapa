using UnityEngine;
using System.Collections;

public class CameraEasyScaler : MonoBehaviour
{
	[SerializeField]
	private float changeTime = 4f;

	private Vector3 baseLocal;
	private float baseSize;

	public Vector3 fixedPoint;
	private Vector3 offset;

	[SerializeField]
	private Camera cam;

	private Coroutine resizer;

	private Vector3 lastPosition;
	private float lastSize = 0f;
	private float sizeChangeSpeed = 0f;

	void Awake ()
	{
		baseLocal = transform.localPosition;
		baseSize = cam.orthographicSize;
		lastSize = baseSize;



		offset = baseLocal - fixedPoint;

		Debug.Log(offset);

		offset /= baseSize;

		Debug.Log(offset);

	}
	
	public void ChangeSize(float newSize)
	{
		if (resizer != null)
			StopCoroutine(resizer);

		if (newSize != lastSize)
			resizer = StartCoroutine(ResizeCoroutine(newSize));
	}

	private IEnumerator ResizeCoroutine(float newSize)
	{
		float remainingTime = changeTime;

		while (remainingTime >= 0)
		{
			lastSize = Mathf.SmoothDamp(lastSize, newSize, ref sizeChangeSpeed, remainingTime);

			Debug.Log(lastSize);

			cam.orthographicSize = lastSize;
			lastPosition = offset * lastSize + fixedPoint;
			lastPosition.z = -100;
			cam.transform.localPosition = lastPosition;

			yield return null;
			remainingTime -= Time.deltaTime;
		}

		cam.orthographicSize = newSize;
		lastPosition = offset * newSize + fixedPoint;
		lastPosition.z = -100;
		cam.transform.localPosition = lastPosition;
	}
}
