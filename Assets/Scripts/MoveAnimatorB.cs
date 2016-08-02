using UnityEngine;
using System.Collections;

public class MoveAnimatorB : MonoBehaviour {

	public Animator animator;
	public AudioSource audiosource;

	private Coroutine activeCoroutine = null;

	public void Play(string gameName, string animation)
	{

		string anim = "";

		if (animation == "jump")
			anim = "Jump";
		else if (animation == "hop")
			anim = "Hop";
		else if (animation == "flame")
			anim = "Flame";

		if (anim != "")
		{
			animator.CrossFade(anim, 0.1f, 1, 0f);
			animator.SetLayerWeight(1, 1.0f);
		}
	}

	public void MakeSound(AudioClip sound)
	{
		if (sound == null)
		{
			return;
		}

		DoMouth(sound.length * 0.7f);

		audiosource.clip = sound;
		audiosource.Play();
	}

	public void DoMouth(float seconds)
	{
		if (activeCoroutine != null)
			StopCoroutine(activeCoroutine);
		activeCoroutine = StartCoroutine(MouthTimer(seconds));
	}

	IEnumerator MouthTimer(float duration)
	{
		animator.Play("MouthOpen");
		yield return new WaitForSeconds(duration);
		animator.Play("MouthFlat");
		activeCoroutine = null;
	}
}
