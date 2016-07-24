using UnityEngine;
using System.Collections;

public class GrandpapaJumpTest : MonoBehaviour
{
	public Animator animator;

	
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Z))
		{
			animator.CrossFade("Jump", 0.1f, 1, 0f);
			animator.SetLayerWeight(1, 1.0f);

		}
	}
}
