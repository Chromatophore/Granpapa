using UnityEngine;
using System.Collections;

public class goombasplat : MonoBehaviour, IPlayable
{
	public Animator animator;
	public void Play()
	{
		if (animator == null)
			return;
			
		animator.Play("splat",-1,0f);
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}
}
