using UnityEngine;
using System.Collections;

public class chompkill : MonoBehaviour, IPlayable
{
	public Animator animator;

	public chompcandie tester;

	public void Play(string anim)
	{
		if (animator == null)
			return;

		//if (!tester.canDie && anim == "kill")
			//return;
			
		if (anim != "")
			animator.CrossFade(anim,0.1f,-1,0f);

		if (anim == "kill")
			animator = null;
	}

	public GameObject GetGameObject()
	{
		return gameObject;
	}


}
