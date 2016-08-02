using UnityEngine;
using System.Collections;

public class lookflash : MonoBehaviour {

	public GameObject looker;
	
	public void MakeFlash()
	{
		StartCoroutine(flash());
	}

	IEnumerator flash()
	{
		looker.SetActive(false);

		yield return new WaitForSeconds(2f);

		bool on = false;

		int j = 0;
		while (j < 10)
		{
			on = !on;
			looker.SetActive(on);
			j++;
			yield return new WaitForSeconds(0.5f);
		}

		looker.SetActive(false);
	}
}
