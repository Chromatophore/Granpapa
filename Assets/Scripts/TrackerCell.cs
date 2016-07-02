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
}

public class TrackerCell : MonoBehaviour
{

	// Tells unity to show private fields in editor:
	[SerializeField]
	private ButtonObjectAssoctiate[] DisplayBars;


	private Dictionary<BUTTON, GameObject> DisplayBarsDict;

	[SerializeField]
	private bool DeactivateOnStart;

	public SpriteRenderer spriteRenderer;

	void Start()
	{
		// Create a Dict<> struct from our serialised array:
		DisplayBarsDict = new Dictionary<BUTTON, GameObject>();

		foreach (var association in DisplayBars)
		{
			DisplayBarsDict[association.button] = association.obj;
		}


		if (DeactivateOnStart)
		{
			ResetAllBars();
		}
	}

	public void ResetAllBars()
	{
		foreach (var bar in DisplayBarsDict.Values)
		{
			bar.SetActive(false);
		}
	}

	public void TakeInput(BUTTON inputButton)
	{
		GameObject result;
		if (DisplayBarsDict.TryGetValue(inputButton, out result))
		{
			result.SetActive(true);
		}
	}
}
