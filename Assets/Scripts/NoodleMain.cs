﻿using UnityEngine;

public struct PrefabConstruction
{
	public PrefabConstruction(string name, Color color, Vector3 position)
	{
		this.name = name;
		this.color = color;
		this.position = position;
	}
	public string name;
	public Color color;
	public Vector3 position;
}

public class NoodleMain : MonoBehaviour
{
	// This is singletonish stuff
	// Maybe use get/set here?
	public static NoodleMain SingleRef = null;
	{
		if (SingleRef != null)
		{
			Debug.Log("Singleton reference to NoodleMain has been overwritten!");
		}
		SingleRef = pRef;
	}
	{
		return SingleRef;
	}

	public List<PrefabConstruction> noodlePrefabs;

	public Dictionary<string, GameObject> createdObjects;
		NoodleMain.SetSingleRef(this);

		noodlePrefabs = new List<PrefabConstruction>();
		// Enemy doodies
		noodlePrefabs.Add(new PrefabConstruction("en_yellow", Color.yellow, new Vector3(0, 0.375f, 0)));
		noodlePrefabs.Add(new PrefabConstruction("en_blue", Color.blue, new Vector3(0, 0.125f, 0)));
		noodlePrefabs.Add(new PrefabConstruction("en_red", Color.red, new Vector3(0, -0.125f, 0)));
		noodlePrefabs.Add(new PrefabConstruction("en_green", Color.green, new Vector3(0, -0.375f, 0)));

		noodlePrefabs.Add(new PrefabConstruction("pl_a", Color.yellow, new Vector3(0, 0.375f, 0)));
		noodlePrefabs.Add(new PrefabConstruction("pl_b", Color.blue, new Vector3(0, 0.125f, 0)));
		noodlePrefabs.Add(new PrefabConstruction("pl_x", Color.red, new Vector3(0, -0.125f, 0)));
		noodlePrefabs.Add(new PrefabConstruction("pl_y", Color.green, new Vector3(0, -0.375f, 0)));

		createdObjects = new Dictionary<string, GameObject>();

		foreach (var prefab in noodlePrefabs)
		{
				// Create all our tracker cells from the prefab:
				GameObject parentObject = Instantiate(displayParent, Vector3.zero, Quaternion.identity) as GameObject;
				createdObjects[prefab.name] = parentObject;
				// Parent them to us so they are held within us in the heirarchy
				// (There is also a SetParent method but this is mostly useful to apply aspects of our transform)
				// (I'm always just lazy and reposition it after creation:)
				parentObject.transform.parent = transform;

				parentObject.transform.localPosition = Vector3.zero;

				// Create all our tracker cells from the prefab:
				GameObject childObject = Instantiate(displayBar, Vector3.zero, Quaternion.identity) as GameObject;
				// Parent them to us so they are held within us in the heirarchy
				// (There is also a SetParent method but this is mostly useful to apply aspects of our transform)
				// (I'm always just lazy and reposition it after creation:)
				childObject.transform.parent = parentObject.transform;

				childObject.transform.localPosition = prefab.position;

				childObject.GetComponent<SpriteRenderer>().color = prefab.color;
		}
	}