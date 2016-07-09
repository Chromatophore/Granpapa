using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

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
	public static void SetSingleRef(NoodleMain pRef)
	{
		if (SingleRef != null)
		{
			Debug.Log("Singleton reference to NoodleMain has been overwritten!");
		}
		SingleRef = pRef;
	}
	public static NoodleMain GetSingleRef()
	{
		return SingleRef;
	}

	[SerializeField]
	private GameObject displayParent;

	[SerializeField]
	private GameObject displayBar;

	private List<List<Color>> barColors;

	public List<PrefabConstruction> noodlePrefabs;

	private Dictionary<string, GameObject> createdObjects;

	// Use this for initialization
	void Start () {
		NoodleMain.SetSingleRef(this);

		var newList = new List<Color>( new Color[] { Color.yellow, Color.blue, Color.red, Color.green } );
		barColors = new List<List<Color>>();
		barColors.Add(newList);
		newList = new List<Color>(new Color[] { Color.yellow, Color.blue, Color.red, Color.green });
		barColors.Add(newList);

		noodlePrefabs = new List<PrefabConstruction>();
		// Enemy doodies
		noodlePrefabs.Add(new PrefabConstruction("goomba", Color.yellow, new Vector3(0, 0.375f, 0)));
		noodlePrefabs.Add(new PrefabConstruction("en_blue", Color.blue, new Vector3(0, 0.125f, 0)));
		noodlePrefabs.Add(new PrefabConstruction("en_red", Color.red, new Vector3(0, -0.125f, 0)));
		noodlePrefabs.Add(new PrefabConstruction("en_green", Color.green, new Vector3(0, -0.375f, 0)));

		noodlePrefabs.Add(new PrefabConstruction("player_a", Color.white, new Vector3(0, 0.375f, -0.1f)));
		noodlePrefabs.Add(new PrefabConstruction("player_b", Color.white, new Vector3(0, 0.125f, -0.1f)));
		noodlePrefabs.Add(new PrefabConstruction("player_x", Color.white, new Vector3(0, -0.125f, -0.1f)));
		noodlePrefabs.Add(new PrefabConstruction("jump", Color.white, new Vector3(0, -0.375f, -0.1f)));

		noodlePrefabs.Add(new PrefabConstruction("", Color.clear, Vector3.zero));
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

	public GameObject GetPrefab(string name)
	{
		GameObject prefab;

		if (!createdObjects.TryGetValue(name, out prefab))
		{
			prefab = createdObjects[""];
		}
		return prefab;
	}
}
