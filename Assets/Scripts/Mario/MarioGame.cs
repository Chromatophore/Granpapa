using UnityEngine;
using System.Collections.Generic;


public class MarioGame : MonoBehaviour, IGameDisplay, IObserver<BeatData>
{
	public StringToPrefab[] prefabList;
	private Dictionary<string, GameObject> prefabDict;

	private IDisposable Unsubscriber;

	public GameObject playerAvatar;
	private Vector3 currentPosition;
	private Vector3 targetPosition;
	private float targetGoalTime;
	private float timeMotionStart = 0f;
	private float timeInMotion = 0f;

	private MoveAnimatorA playerAnimator;

	public float MoveCurve(float ratio)
	{
		return ratio;/*
		float cosWave = (1f + Mathf.Cos((1 + ratio) * Mathf.PI)) / 2;
		cosWave *= cosWave;
		return cosWave;*/
	}

	public void SetUpcomingAttack(int distanceAhead, string attack)
	{
		string key = attack;
		switch(key)
		{
			case "goomba":
				//Debug.Log("Goomba!");
				break;
			case "pit":
				//Debug.Log("Scary pit!");
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
			GameObject newObject = Instantiate(newObjectPrefab, pos, Quaternion.identity ) as GameObject;
			newObject.transform.parent = transform;

			Destroy(newObject,20f);
		}

		creationPoint += horizontalSpacing;
	}

	public void SetUpcomingAttacks(int distanceAhead, List<string> attacks)
	{
		Debug.Log(EasyPrint.MakeString<string>(attacks));

		// TODO make this better
		if (attacks == null)
			attacks = new List<string>(new string[] {"","","","","","" });

		foreach (var attack in attacks)
		{
			SetUpcomingAttack(distanceAhead, attack);
		}
	}

	public void PassPlayerAnimation(string animation)
	{
		if (playerAnimator == null)
			return;
		playerAnimator.Play(animation);
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

		IObservable<BeatData> beatObservable = GameState.State.CurrentPageSequence as IObservable<BeatData>;
		Unsubscriber = beatObservable.Subscribe(this);

		// hack:
		GameState.State.CurrentPageSequence.gameDisplay = this;

		targetPosition = playerAvatar.transform.position;

		playerAnimator = playerAvatar.GetComponentInChildren<MoveAnimatorA>();

		//SetUpcomingAttacks(0, null);
		//SetUpcomingAttacks(0, null);
	}

	void Update()
	{
		// interpolating between locations:
		if (targetGoalTime != 0)
		{
			timeInMotion = Time.time  - timeMotionStart;

			float ratio = Mathf.Clamp(timeInMotion / targetGoalTime,0f,1f);

			ratio = MoveCurve(ratio);

			playerAvatar.transform.position = Vector3.Lerp(currentPosition, targetPosition, ratio);

			if (ratio == 1f)
				targetGoalTime = 0;
		}
	}

	void OnDestroy()
	{
		Unsubscriber.Dispose();
	}



	// IObserver implimentation:
	public void OnCompleted()
	{

	}
	public void OnError(System.Exception exception)
	{

	}
	public void OnNext(BeatData data)
	{
		if (!data.resolutionBeat)
			return;

		playerAvatar.transform.position = targetPosition;

		timeMotionStart = Time.time;
		
		currentPosition = playerAvatar.transform.position;
		targetPosition = currentPosition;
		targetPosition.x += horizontalSpacing;

		targetGoalTime = data.beatTime;
	}
}