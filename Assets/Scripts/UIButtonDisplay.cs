using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

[System.Serializable]
public struct ButtonToObject
{
	public BUTTON concept;
	public GameObject prefab;
	public Text text;
}

public class UIButtonDisplay : MonoBehaviour {

	[SerializeField]
	private ButtonToObject[] mainButtonsSerialized;
	private Dictionary<BUTTON, ButtonToObject> mainButtons;

	[SerializeField]
	private Text[] abilityText;

	// Use this for initialization
	void Start () {
		mainButtons = new Dictionary<BUTTON,ButtonToObject>();
		foreach (var pair in mainButtonsSerialized)
		{
			mainButtons[pair.concept] = pair;
		}
		DisableDisplay();
	}

	private void DisableDisplay()
	{
		foreach (var button in mainButtons.Values)
		{
			button.prefab.SetActive(false);		
		}
	}

	public void ParseActiveInputDict(Dictionary<BUTTON, string> pageActiveInputDict)
	{
		foreach (var button in pageActiveInputDict.Keys)
		{
			mainButtons[button].text.text = pageActiveInputDict[button];
			mainButtons[button].prefab.SetActive(true);
		}
	}
}
