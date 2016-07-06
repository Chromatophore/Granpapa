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
	
	public string Step(TrackerData currentStep)
	{
		//currentStep.trackerDisplay.EnemyInput(GetRandomEnum<BUTTON>());
		//currentStep.obj.transform;
		var enemyAttacks = new List<string>();
		enemyAttacks.Add("en_red");
		enemyAttacks.Add("en_yellow");
		enemyAttacks.Add("en_green");
		enemyAttacks.Add("en_blue");

		return enemyAttacks[(int)(Random.value * enemyAttacks.Count)];
	}
}
