using UnityEngine;
using System.Collections;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public struct AnimMapSet
	{
		public AnimMapSet(string e, string p, string a)
		{
			enemy = e;
			player = p;
			anim = a;
		}
		public string enemy;
		public string player;
		public string anim;
	}

public class PlayerAnimMap
{

	private Dictionary<string, Dictionary<string, string>> map;

	public PlayerAnimMap()
	{
		map = new Dictionary<string, Dictionary<string, string>>();
	}

	public PlayerAnimMap(AnimMapSet[] inputSets )
	{
		map = new Dictionary<string, Dictionary<string, string>>();
		AddFromData(inputSets);
	}

	public PlayerAnimMap(string[] inputStrings)
	{
		map = new Dictionary<string, Dictionary<string, string>>();
		AddFromData(inputStrings);
	}


	public string GetPlayerAnim(string enemy, string player)
	{
		Dictionary<string, string> actionAnimMap;
		if (map.TryGetValue(enemy, out actionAnimMap))
		{
			string anim = "";
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

	public void AddMap(string enemy, string player, string anim)
	{
		Dictionary<string, string> actionAnimMap;

		if (map.TryGetValue(enemy, out actionAnimMap))
		{
			actionAnimMap[player] = anim;
		}
		else
		{
			var newDict = new Dictionary<string, string>();
			map[enemy] = newDict;
			newDict[player] = anim;
		}
	}

	public void AddFromData(string[] inputArray)
	{
		for (int j = 0; j < inputArray.Length; j += 3)
		{
			if ((j + 2) >= inputArray.Length)
			{
				Debug.Log("Non multiple of 3 string array passed to PlayerAnimMap");
				return;
			}
			AddMap(inputArray[j], inputArray[j+1], inputArray[j+2]);
		}
	}

	public void AddFromData(IEnumerable<AnimMapSet> input)
	{
		foreach (AnimMapSet inputSet in input)
		{
			AddMap(inputSet.enemy, inputSet.player, inputSet.anim);
		}
	}
}