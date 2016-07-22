using UnityEngine;
using System.Collections;

public class MoveAnimatorA : MonoBehaviour {

	public Animator animator;

	

	void Start()
	{

	}


	public void Play(string animName, bool fade = false)
	{
		if (animator == null)
			return;
		
		animator.Play(animName, -1, 0f);
	}
}
