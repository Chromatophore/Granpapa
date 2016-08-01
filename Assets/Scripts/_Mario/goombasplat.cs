using UnityEngine;
using System.Collections;

public class goombasplat : MonoBehaviour, IPlayable
{
	public PhysPop popper;

	public Animator animator;
	public void Play(string anim)
	{
		if (anim == "kill")
		{
			if (popper != null)
			{
				popper.Pop(transform);
				popper = null;
			}
			return;
		}
		if (animator == null)
			return;
			
		
		if (anim == "collect")
			return;
		
		animator.Play(anim,-1,0f);
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}


}
