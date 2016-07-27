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
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	private Color storedColor;

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

		spriteRenderer.color = storedColor;
	}

	public void ConsumeColor(Color inputColor)
	{
		storedColor = inputColor;
		spriteRenderer.color = inputColor;
	}

	public void SetTemporaryColor(Color inputColor)
	{
		spriteRenderer.color = inputColor;
	}

	public void AddChild(GameObject child)
	{
		// Create all our tracker cells from the prefab:
		GameObject childObject = Instantiate(child, Vector3.zero, Quaternion.identity) as GameObject;
		// Parent them to us so they are held within us in the heirarchy
		childObject.transform.SetParent(transform,false);
		childObject.transform.localPosition = new Vector3(0.0f, 0.0f, -0.1f);

		myChildren.Add(childObject);
	}
}
