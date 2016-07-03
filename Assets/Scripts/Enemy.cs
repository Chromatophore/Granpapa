using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class Enemy
{
	public Enemy()
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
		currentStep.trackerCell.EnemyInput(GetRandomEnum<BUTTON>());
	}
}
