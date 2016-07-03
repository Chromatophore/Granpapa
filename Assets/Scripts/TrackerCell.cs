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

public class TrackerCell : MonoBehaviour
{

	// Tells unity to show private fields in editor:
	[SerializeField]
	private ButtonObjectAssoctiate[] DisplayBars;

	public Dictionary<BUTTON, int> enemyBars { get; private set; }
	public Dictionary<BUTTON, int> playerBars { get; private set; }

	private Dictionary<BUTTON, ButtonObjectAssoctiate> DisplayBarsDict;

	[SerializeField]
	private bool DeactivateOnStart;

	public SpriteRenderer spriteRenderer;

	void Start()
	{
		// Create a Dict<> struct from our serialised array:
		DisplayBarsDict = new Dictionary<BUTTON, ButtonObjectAssoctiate>();

		// Activebars and Enemybars are dicts of bars that are active, in theory!
		enemyBars = new Dictionary<BUTTON, int>();
		playerBars = new Dictionary<BUTTON, int>();

		foreach (var association in DisplayBars)
		{
			DisplayBarsDict[association.button] = association;
		}

		foreach (BUTTON button in System.Enum.GetValues(typeof(BUTTON)))
		{
			enemyBars.Add(button, 0);
			playerBars.Add(button, 0);
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
			bar.obj.SetActive(false);
			bar.obj.GetComponent<SpriteRenderer>().color = bar.startColor;
		}

		enemyBars.Clear();
		playerBars.Clear();
	}

	public void PlayerInput(BUTTON inputButton)
	{
		ButtonObjectAssoctiate result;
		if (DisplayBarsDict.TryGetValue(inputButton, out result))
		{
			int pressed;
			if (enemyBars.TryGetValue(inputButton, out pressed))
			{
				playerBars[inputButton] = 1;
				result.obj.GetComponent<SpriteRenderer>().color = Color.white;
			}
			else
			{
				playerBars[inputButton] = 1;
				result.obj.SetActive(true);
			}
		}
	}

	public void EnemyInput(BUTTON inputButton)
	{
		ButtonObjectAssoctiate result;
		if (DisplayBarsDict.TryGetValue(inputButton, out result))
		{
			enemyBars[inputButton] = 1;
			result.obj.SetActive(true);
		}
	}
}
