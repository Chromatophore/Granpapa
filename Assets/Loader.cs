using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Loader : MonoBehaviour {

	[SerializeField]
	private int scene;

	public bool pushedKey = false;
	public bool loaded = false;

	public Text loadText;
	public Text pushKey;

	// Use this for initialization
	void Start () {
		StartCoroutine(LoadNewScene());
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!loaded)
			return;

		if (Input.GetKeyDown(KeyCode.Z))
		{
			pushedKey = true;
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			pushedKey = true;
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			pushedKey = true;
		}
	}

	IEnumerator LoadNewScene()
	{
		Debug.Log("Loading begun...");

		AsyncOperation async = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
		async.allowSceneActivation = false;

		// While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
		while (async.progress < 0.9f) {
			yield return null;
		}

		loaded = true;
		loadText.gameObject.SetActive(false);
		pushKey.gameObject.SetActive(true);

		while (pushedKey == false)
		{
			yield return null;
		}

		async.allowSceneActivation = true;
		
		Debug.Log("Main scene loaded!");

	}
}
