using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public enum BUTTON
{
	A,
	B,
	X,
	Y
}

// Make sure Unity tries tp display this in the editor:
[System.Serializable]
public struct ButtonObjectAssoctiate
{
	public BUTTON button;
	public GameObject obj;
	public Color startColor;
}

public class BlankTrackerCell : MonoBehaviour, ITrackerDisplay
{

	public SpriteRenderer spriteRenderer;

	private List<GameObject> myChildren;

	void Awake()
	{
		myChildren = new List<GameObject>();
	}

	public void ResetDisplay()
	{
		if (myChildren == null)
			return;
		
		foreach (var child in myChildren)
		{
			Destroy(child);
		}
		myChildren.Clear();
	}

	public void ConsumeColor(Color inputColor)
	{
		spriteRenderer.color = inputColor;
	}

	public void AddChild(GameObject child)
	{
		// Create all our tracker cells from the prefab:
		GameObject childObject = Instantiate(child, Vector3.zero, Quaternion.identity) as GameObject;
		// Parent them to us so they are held within us in the heirarchy
		// (There is also a SetParent method but this is mostly useful to apply aspects of our transform)
		// (I'm always just lazy and reposition it after creation:)
		childObject.transform.parent = transform;

		childObject.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);

		myChildren.Add(childObject);
	}
}
