using UnityEngine;
using System.Collections;

public class CoinCollect : MonoBehaviour, IPlayable
{
	public GameObject coin;
	bool collected = false;
	public void Play(string anim)
	{
		if (anim == "collect" && !collected)
		{
			collected = true;
			Destroy(coin,0.25f);
		}
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}
}
