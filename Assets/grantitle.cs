using UnityEngine;
using System.Collections;

public class grantitle : MonoBehaviour {

	public Animator animator;
	// Use this for initialization
	void Start () {
		animator.Play("TitlePose");
		animator.Play("MouthSmile");
		animator.Play("EyesOpen");
	}
}
