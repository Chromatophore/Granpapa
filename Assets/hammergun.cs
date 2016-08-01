using UnityEngine;
using System.Collections;

public class hammergun : MonoBehaviour {

	public GameObject hammerPrefabHigh;
	public GameObject hammerPrefabLow;

	public bool fireLo;
	public bool fireHi;

	public float refire = 0.1f;
	public float lastFire = 0f;

	public GameObject hammerCreationPoint;

	// Update is called once per frame
	void Update () {
	
		if (fireHi || fireLo)
		{
			if (Time.time > lastFire + refire)
			{
				lastFire = Time.time;
				var obj = hammerPrefabHigh;
				if (fireLo)
					obj = hammerPrefabLow;
				Instantiate(obj, hammerCreationPoint.transform.position, Quaternion.identity);
			}
		}
	}
}
