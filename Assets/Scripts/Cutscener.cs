using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Cutscener : MonoBehaviour
{
	public static Cutscener SingleRef { get; private set; }

	public GameObject textBoxBG;
	public Text gameText;

	void Awake()
	{
		SingleRef = this;
	}

	public static void SetText(string text)
	{
		SingleRef.textBoxBG.SetActive(true);
		SingleRef.gameText.text = text;
	}

	public static void Clear()
	{
		SingleRef.gameText.text = "";
	}

	public static void Hide()
	{
		Clear();
		SingleRef.textBoxBG.SetActive(false);
	}
}
