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
		foreach (KeyValuePair<BUTTON, int> value in currentStep.trackerCell.enemyBars)
		{
			int player;
			if (currentStep.trackerCell.playerBars.TryGetValue(value.Key, out player))
			{
				if (player == value.Value)
				{
					Debug.Log("Yay you killed a thing but I haven't made a way to change colors that is sane!");
				}
			}
		}
	}
}
