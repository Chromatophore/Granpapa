using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class BlankTrackerCell : MonoBehaviour, ITrackerDisplay
{

	public Dictionary<string, int> enemyBars { get; private set; }
	public Dictionary<string, int> playerBars { get; private set; }

	public GameObject DisplayCellPrefab;

	public SpriteRenderer spriteRenderer;

	private DisplayData displayData;

	private List<GameObject> myChildren;

	void Start()
	{
		myChildren = new List<GameObject>();
		/*
		// Create all our tracker cells from the prefab:
		displayData.obj = Instantiate(DisplayCellPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		// Parent them to us so they are held within us in the heirarchy
		// (There is also a SetParent method but this is mostly useful to apply aspects of our transform)
		// (I'm always just lazy and reposition it after creation:)
		displayData.obj.transform.parent = transform;

		displayData.obj.transform.localPosition = new Vector3(0.0f, 0.375f, -0.1f);

		displayData.trackerDisplay = displayData.obj.GetComponent(typeof(IDisplay)) as IDisplay;
		 * */

		// Activebars and Enemybars are dicts of bars that are active, in theory!
		enemyBars = new Dictionary<string, int>();
		playerBars = new Dictionary<string, int>();

	}

	public void ResetDisplay()
	{
		foreach (var child in myChildren)
		{
			Destroy(child);
		}
		myChildren.Clear();
		enemyBars.Clear();
		playerBars.Clear();
	}

	public void TempGetResolutionData(out Dictionary<string, int> eBars, out Dictionary<string, int> pBars)
	{
		eBars = enemyBars;
		pBars = playerBars;
	}

	public void PlayerInput(BUTTON inputButton)
	{
		int used;
		if (playerBars.TryGetValue("Used", out used))
		{
			if (used != 0)
			{
				return;
			}
		}

		displayData.trackerDisplay.RenderInput(inputButton);

		/*
		ButtonObjectAssoctiate result;
		if (DisplayBarsDict.TryGetValue(inputButton, out result))
		{
			int pressed;
			if (enemyBars.TryGetValue(inputButton.ToString(), out pressed))
			{
				playerBars["Used"] = 1;
				playerBars[inputButton.ToString()] = 1;
				SetColor(inputButton, Color.white);
			}
			else
			{
				playerBars["Used"] = 1;
				playerBars[inputButton.ToString()] = 1;
				result.obj.SetActive(true);
			}
		}
		 */
	}

	public void ConsumeColor(Color inputColor)
	{
		spriteRenderer.color = inputColor;
	}

	public void SetColor(BUTTON whichBar, Color newColor) // Oh god why am I using buttons everywhere? This needs to change.
	{
		/*
		ButtonObjectAssoctiate result;
		if (DisplayBarsDict.TryGetValue(whichBar, out result))
		{
			result.obj.GetComponent<SpriteRenderer>().color = newColor;
		}
		 * */
	}

	public void EnemyInput(BUTTON inputButton)
	{
		displayData.trackerDisplay.RenderInput(inputButton);
		/*
		ButtonObjectAssoctiate result;
		if (DisplayBarsDict.TryGetValue(inputButton, out result))
		{
			enemyBars[inputButton.ToString()] = 1;
			result.obj.SetActive(true);
		}
		 * */
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
