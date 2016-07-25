using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public struct AnimMapSet
	{
		public AnimMapSet(string e, string p, string pa, string pe, string scoreString)
		{
			enemy = e;
			player = p;
			playerAnim = pa;
			enemyAnim = pe;
			score = int.Parse(scoreString);
		}
		public AnimMapSet(string e, string p, string pa, string pe, int score)
		{
			enemy = e;
			player = p;
			playerAnim = pa;
			enemyAnim = pe;
			this.score = score;
		}

		public string enemy;
		public string player;
		public string playerAnim;
		public string enemyAnim;
		public int score;
	}

public class PlayerAnimMap
{
	private Dictionary<string, Dictionary<string, AnimMapSet>> map;

	public PlayerAnimMap()
	{
		map = new Dictionary<string, Dictionary<string, AnimMapSet>>();
	}

	public PlayerAnimMap(AnimMapSet[] inputSets)
	{
		map = new Dictionary<string, Dictionary<string, AnimMapSet>>();
		AddFromData(inputSets);
	}

	public PlayerAnimMap(string[] inputStrings)
	{
		map = new Dictionary<string, Dictionary<string, AnimMapSet>>();
		AddFromData(inputStrings);
	}


	/*
	public string GetPlayerAnim(string enemy, string player)
	{
		Dictionary<string, string> actionAnimMap;
		if (map.TryGetValue(enemy, out actionAnimMap))
		{
			AnimMapSet animSet;
			if (!actionAnimMap.TryGetValue(player, out anim))
				actionAnimMap.TryGetValue("def", out anim);

			//Debug.Log(string.Format("Tried {0} + {1}, got {2}", enemy, player, anim));
			return anim;
		}
		else
		{
			return "";
		}
	}
	*/

	public int AssessSuccess(PageTrackerData situation)
	{
		if (map == null)
			return 0;

		var enemy = situation.enemy;
		var player = situation.player;
		int score = 0;

		Dictionary<string, AnimMapSet> actionAnimMap;
		if (map.TryGetValue(enemy, out actionAnimMap))
		{
			AnimMapSet animSet;
			if (actionAnimMap.TryGetValue(player, out animSet) || actionAnimMap.TryGetValue("def", out animSet))
			{
				situation.playerAnimation = animSet.playerAnim;
				situation.enemyAnimation = animSet.enemyAnim;
				score = animSet.score;
				situation.score = score;

				situation.success = score >= 0;
			}
		}

		return score;
	}

	public void AddMap(string enemy, string player, string playerAnim, string enemyAnim, int score)
	{
		AddMap(new AnimMapSet(enemy, player, playerAnim, enemyAnim, score));
	}
	public void AddMap(AnimMapSet animSet)
	{
		Dictionary<string, AnimMapSet> actionAnimMap;

		if (map.TryGetValue(animSet.enemy, out actionAnimMap))
		{
			actionAnimMap[animSet.player] = animSet;
		}
		else
		{
			var newDict = new Dictionary<string, AnimMapSet>();
			map[animSet.enemy] = newDict;
			newDict[animSet.player] = animSet;
		}
	}

	public void AddFromData(string[] inputArray)
	{
		for (int j = 0; j < inputArray.Length; j += 5)
		{
			if ((j + 4) >= inputArray.Length)
			{
				Debug.Log("Non multiple of 5 string array passed to PlayerAnimMap");
				return;
			}
			AddMap(new AnimMapSet(inputArray[j], inputArray[j+1], inputArray[j+2], inputArray[j+3], inputArray[j+4]));
		}
	}

	public void AddFromData(IEnumerable<AnimMapSet> input)
	{
		foreach (AnimMapSet inputSet in input)
		{
			AddMap(inputSet);
		}
	}
}