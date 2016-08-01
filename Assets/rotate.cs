using UnityEngine;
using System.Collections;

public class rotate : MonoBehaviour {

	public float startVel;

	public float angle = 45;

	public float rngA = -5;
	public float rngB = 10;

	public float rspeed = 540;
	// Update is called once per frame

	private Vector3 vel;
	public Vector3 grav;

	void Start()
	{
		Destroy(gameObject,10f);

		angle += rngA + Random.value * rngB;
		angle *= Mathf.Deg2Rad;
		vel = new Vector3( -Mathf.Sin(angle) * startVel, Mathf.Cos(angle) * startVel);
	}


	void Update ()
	{
		transform.Rotate(0f, 0f, rspeed * Time.deltaTime);


		vel += grav * Time.deltaTime;

		transform.position += vel * Time.deltaTime;
	}
}
