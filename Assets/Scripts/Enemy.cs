using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public interface IDisplay
{
	void Step(TrackerData currentStep);
}

public class Enemy : IDisplay
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
		currentStep.trackerDisplay.EnemyInput(GetRandomEnum<BUTTON>());
	}
}
