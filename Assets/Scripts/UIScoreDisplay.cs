using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIScoreDisplay : MonoBehaviour {
	[SerializeField]
	private Text[] scoreDisplays;

	[SerializeField]
	private Color onColor;
	[SerializeField]
	private Color offColor;
	[SerializeField]
	private float flashTime;
	private Coroutine activeCoroutine;

	private int displayCase;

	// Use this for initialization
	void Start () {
		TurnOffText();
		Reset();
	}
	
	public void Reset()
	{
		displayCase = 6;
	}

	public void PageScore(int score)
	{
		int direction = displayCase % 3;
		int cap = 6;
		if (score == 2)
		{
			cap = 9;
			score = 1;
		}
		else if (displayCase > 6) // We're already grampin' "Great". We may want to prevent recovery to 9 without high scoring plays?
		{
			cap = 9;
		}
		if (direction == 0)
		{
		}
		else if (direction == 1) // Up
		{
			if (score > 0)
				score++;
		}
		else if (direction == 2) // Down
		{
			if (score < 0)
				score--;
		}
		displayCase += score;
		if (displayCase > cap)
		{
			displayCase = cap;
		}
		else if (displayCase < 0)
		{
			displayCase = 0;
		}
		//Debug.Log("Scores: " + score + " " + displayCase + " " + direction);
		DrawScore();
	}

	private void DrawScore()
	{
		TurnOffText();
		switch (displayCase)
		{
			case 9:
				scoreDisplays[3].color = onColor;
				break;
			case 8:
				scoreDisplays[3].color = onColor;
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[2]));
				break;
			case 7:
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[3]));
				scoreDisplays[2].color = onColor;
				break;
			case 6:
				scoreDisplays[2].color = onColor;
				break;
			case 5:
				scoreDisplays[2].color = onColor;
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[1]));
				break;
			case 4:
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[2]));
				scoreDisplays[1].color = onColor;
				break;
			case 3:
				scoreDisplays[1].color = onColor;
				break;
			case 2:
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[0]));
				scoreDisplays[1].color = onColor;
				break;
			case 1:
				scoreDisplays[0].color = onColor;
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[1]));
				break;
			case 0:
				scoreDisplays[0].color = onColor;
				break;
			default:
				break;
		}
		/*
		if (totalScore > 80)
		{
			if (lastScore <= 80)
			{
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[3]));
				scoreDisplays[2].color = onColor;
			}
			else
			{
				scoreDisplays[3].color = onColor;
			}
		}
		else if (totalScore >= 60)
		{
			if (lastScore >= 80)
			{
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[2]));
				scoreDisplays[3].color = onColor;
			}
			else if (lastScore < 60)
			{
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[3]));
				scoreDisplays[2].color = onColor;
			}
			else
			{
				scoreDisplays[2].color = onColor;
			}
		}
		else if (totalScore >= 30)
		{
			if (lastScore >= 60)
			{
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[1]));
				scoreDisplays[2].color = onColor;
			}
			else if (lastScore < 30)
			{
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[2]));
				scoreDisplays[1].color = onColor;
			}
			else
			{
				scoreDisplays[1].color = onColor;
			}
		}
		else
		{
			if (lastScore >= 60)
			{
				activeCoroutine = StartCoroutine(FlashText(scoreDisplays[0]));
				scoreDisplays[1].color = onColor;
			}
			else
			{
				scoreDisplays[0].color = onColor;
			}
		}
		lastScore = totalScore;
		 * */
	}

	private void TurnOffText()
	{
		foreach (var text in scoreDisplays)
		{
			text.color = offColor;
		}
		if (activeCoroutine != null)
		{
			StopCoroutine(activeCoroutine);
			activeCoroutine = null;
		}
	}

	private IEnumerator FlashText(Text text)
	{
		while(true)
		{
			text.color = onColor;
			yield return new WaitForSeconds(flashTime);
			text.color = offColor;
			yield return new WaitForSeconds(flashTime);
		}
	}
}
