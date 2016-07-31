using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIButtonDisplay : MonoBehaviour {

	[SerializeField]
	private GameObject[] mainButtons;

	[SerializeField]
	private Text[] abilityText;

	// Use this for initialization
	void Start () {
		DisableDisplay();
	}

	private void DisableDisplay()
	{
		foreach (var button in mainButtons)
		{
			button.active = false;			
		}
	}

	public void ParseActiveInputDict()
	{

	}
}
