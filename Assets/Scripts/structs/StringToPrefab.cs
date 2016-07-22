using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct StringToPrefab
{
	public string concept;
	public GameObject prefab;

	static public List<StringToPrefab> MakeList(StringToPrefab[] inputArray)
	{
		return new List<StringToPrefab>(inputArray);
	}
	static public Dictionary<string, GameObject> MakeDict(StringToPrefab[] inputArray)
	{
		Dictionary<string, GameObject> outputDict = new Dictionary<string, GameObject>();
		foreach (var keypair in inputArray)
		{
			outputDict[keypair.concept] = keypair.prefab;
		}
		return outputDict;
	}
}