using UnityEngine;
using System.Collections;

public class goombasplat : MonoBehaviour, IPlayable
{
	public Animator animator;
	public void Play(string anim)
	{
		if (animator == null)
			return;
			
		animator.Play(anim,-1,0f);
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}
}
