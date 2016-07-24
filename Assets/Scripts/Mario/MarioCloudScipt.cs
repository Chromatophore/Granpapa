using UnityEngine;
using System.Collections;

public class MarioCloudScipt : MonoBehaviour {

	public GameObject lookTarget;
	
	public float eyeDist;

	public GameObject eyeLeft;
	public GameObject eyeRight;

	private bool saveEyes = true;
	private Vector3 eyeRootL;
	private Vector3 eyeRootR;

	public Vector3 movementSpeed;

	void Start()
	{
		Destroy(gameObject,30f);
	}

	// Update is called once per frame
	void Update ()
	{
		transform.position += movementSpeed * Time.deltaTime;

		if (lookTarget == null)
			return;
		
		if (saveEyes)
		{
			saveEyes = false;
			eyeRootR = eyeRight.transform.localPosition;
			eyeRootL = eyeLeft.transform.localPosition;
		}

		Vector3 eyeLvec = lookTarget.transform.position - (transform.position + eyeRootL);
		Vector3 eyeRvec = lookTarget.transform.position - (transform.position + eyeRootR);

		eyeLvec.z = 0f;
		eyeRvec.z = 0f;
		
		eyeLeft.transform.localPosition = eyeRootL + eyeLvec.normalized * eyeDist;
		eyeRight.transform.localPosition = eyeRootR + eyeRvec.normalized * eyeDist;
	}
}
