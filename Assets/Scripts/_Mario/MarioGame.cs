using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct MarioGameCell
{
	public GameObject cellObject;
	public IPlayable playable;
}


public class MarioGame : MonoBehaviour, IGameDisplay
{
	public StringToPrefab[] prefabList;
	private Dictionary<string, GameObject> prefabDict;

	public StringToPrefab[] makeInsteadOfAnimatePrefabs;
	private Dictionary<string, GameObject> makeInsteadDict;

	public StringToPrefab[] trackerPrefabs;

	private IDisposable Unsubscriber = null;

	public GameObject playerAvatar;
	private Vector3 currentPosition;
	private Vector3 targetPosition;
	private float targetGoalTime;
	private float timeMotionStart = 0f;
	private float timeInMotion = 0f;

	private MoveAnimatorA playerAnimator;
	private Queue<MarioGameCell> cellQueue;

	private GameObject movementDolly;

	public GameObject marioA;
	public GameObject marioB;

	[SerializeField]
	private CameraEasyScaler camScaler; 

	public float MoveCurve(float ratio)
	{
		return ratio;/*
		float cosWave = (1f + Mathf.Cos((1 + ratio) * Mathf.PI)) / 2;
		cosWave *= cosWave;
		return cosWave;*/
	}

	public void SetUpcomingAttack(int distanceAhead, string attack, bool firstOfBar)
	{
		string key = attack;
		switch(key)
		{
			case "goomba":
			case "pit":
			case "chomp":
				break;
			case "reset":
				marioB.SetActive(false);
				marioA.SetActive(true);
				key = "default";
				break;
			default:
				key = "default";
				break;
		}

		GameObject newObjectPrefab;
		if (prefabDict.TryGetValue(key, out newObjectPrefab))
		{
			Vector3 pos = transform.position + creationStart;
			pos.x += creationPoint;
			pos += movementDolly.transform.localPosition;
			GameObject newObject = Instantiate(newObjectPrefab, pos, Quaternion.identity ) as GameObject;
			newObject.transform.parent = movementDolly.transform;

			if (firstOfBar)
			{
				if (prefabDict.TryGetValue("barstart", out newObjectPrefab))
				{
					GameObject barMarker = Instantiate(newObjectPrefab, pos, Quaternion.identity ) as GameObject;
					barMarker.transform.parent = newObject.transform;
				}
			}

			IPlayable playable = newObject.GetComponentInChildren<IPlayable>();

			var newCell = new MarioGameCell();
			newCell.cellObject = newObject;
			newCell.playable = playable;

			cellQueue.Enqueue(newCell);

			// give them a death sentence?
			//Destroy(newObject,40f);
		}

		creationPoint += horizontalSpacing;
	}

	public void SetUpcomingAttacks(int distanceAhead, List<string> attacks)
	{
		Debug.Log(EasyPrint.MakeString<string>(attacks));

		// TODO make this better
		if (attacks == null)
			attacks = new List<string>(new string[] {"","","","","","","",""});

		bool firstOfBar = true;
		foreach (var attack in attacks)
		{
			SetUpcomingAttack(distanceAhead, attack, firstOfBar);
			firstOfBar = false;
		}
	}

	public void PassPlayerAnimation(string animation)
	{
		if (playerAnimator == null)
			return;

		GameObject makePrefab;
		if (makeInsteadDict.TryGetValue(animation, out makePrefab))
		{
			playerAnimator.MakeObject(makePrefab);
		}
		else
		{
			playerAnimator.Play(animation);
		}
		
	}

	[SerializeField]
	private float creationPoint = 0f;
	[SerializeField]
	private float horizontalSpacing = 2f;
	[SerializeField]
	private Vector3 creationStart;

	void Start()
	{
		prefabDict = StringToPrefab.MakeDict(prefabList);

		makeInsteadDict = StringToPrefab.MakeDict(makeInsteadOfAnimatePrefabs);

		var prefabHandler = NoodleMain.GetSingleRef();
		foreach (var trackerPrefab in trackerPrefabs)
		{
			prefabHandler.AddPrefab(trackerPrefab.concept, trackerPrefab.prefab);
		}

		// hack:
		GameState.State.CurrentPageSequence.gameDisplay = this;

		targetPosition = Vector3.zero;

		playerAnimator = playerAvatar.GetComponentInChildren<MoveAnimatorA>();

		cellQueue = new Queue<MarioGameCell>();

		movementDolly = new GameObject("dolly");
		movementDolly.transform.position = transform.position;
		movementDolly.transform.parent = transform;

		//SetUpcomingAttacks(0, null);
		//SetUpcomingAttacks(0, null);
	}

	private float nextCloud = 0f;

	void Update()
	{
		// interpolating between locations:
		if (targetGoalTime != 0)
		{
			timeInMotion = Time.time  - timeMotionStart;

			float ratio = Mathf.Clamp(timeInMotion / targetGoalTime,0f,1f);

			ratio = MoveCurve(ratio);

			movementDolly.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, ratio);

			if (ratio == 1f)
				targetGoalTime = 0;
		}

		if (Time.time > nextCloud)
		{
			GameObject newObjectPrefab;
			if (prefabDict.TryGetValue("cloud", out newObjectPrefab))
			{
				Vector3 pos = playerAvatar.transform.position;
				pos.y += 3f + Random.value * 7f;
				pos.x += 20f;
				pos.z = 10;
				GameObject newCloud = Instantiate(newObjectPrefab, pos , Quaternion.identity ) as GameObject;
				var script = newCloud.GetComponent<MarioCloudScipt>();

				newCloud.transform.parent = transform;

				if (script != null)
				{
					script.lookTarget = playerAvatar;
					script.movementSpeed = new Vector3(-0.8f + -0.4f * Random.value,0f,0f);
				}
			}

			nextCloud += 6f + Random.value * 4f;
		}
	}

	void OnDestroy()
	{
		if (Unsubscriber != null)
			Unsubscriber.Dispose();
	}

	public void Beat(float beatTime, PageTrackerData data)
	{
		movementDolly.transform.localPosition = targetPosition;

		timeMotionStart = Time.time;
		
		currentPosition = movementDolly.transform.localPosition;
		targetPosition = currentPosition;
		targetPosition.x -= horizontalSpacing;

		targetGoalTime = beatTime;

		if (cellQueue.Count > 0)
		{
			var cell = cellQueue.Dequeue();
			Destroy(cell.cellObject,10f);

			if (cell.playable != null && data.success)
				cell.playable.Play(data.enemyAnimation);

			if (data.enemy == "firemario")
				StartCoroutine(MarioChange());

			if (data.player == "flame")
			{
				var nextCell = cellQueue.Peek();
				var nextData = data.next;
				if (nextCell.playable != null)// && nextData.enemy == "")
				{
					nextData.enemy = "";
					nextCell.playable.Play("kill");
				}
			}
		}

		PassPlayerAnimation(data.playerAnimation);
	}

	public void SetBeatsPerPhase(int beats)
	{
		if (beats == 4)
		{
			camScaler.ChangeSize(7);
			//camScaler.ChangeSize(13);
		}
		else if (beats == 8)
		{
			camScaler.ChangeSize(13);
		}
	}

	IEnumerator MarioChange()
	{
		bool toggle = false;

		for (int j = 0; j < 6; j++)
		{
			marioA.SetActive(toggle);
			marioB.SetActive(!toggle);
			toggle = !toggle;
			yield return new WaitForSeconds(0.333f);
		}

		marioA.SetActive(false);
		marioB.SetActive(true);
	}
}