using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class Resolver
{
	public Resolver()
	{
	}

	static T GetRandomEnum<T>()
	{
		System.Array A = System.Enum.GetValues(typeof(T));
		T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
		return V;
	}

	public void Step(TrackerData currentStep)
	{
		foreach (BUTTON button in System.Enum.GetValues(typeof(BUTTON)))
		{
			int player = -1;
			currentStep.trackerCell.playerBars.TryGetValue(button.ToString(), out player);

			int enemy = -1;
			currentStep.trackerCell.enemyBars.TryGetValue(button.ToString(), out enemy);

			if (enemy == player && enemy > 0) // Hey, looks like the actions align!
			{
				currentStep.trackerCell.SetColor(button, Color.magenta);
			}
			else if (enemy > 0) // Only the enemy went here.
			{

			}
			else if (player > 0) // Only the player went here.
			{
				currentStep.trackerCell.SetColor(button, Color.gray);
			}
		}
	}
}
