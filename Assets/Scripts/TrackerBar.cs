using UnityEngine;
using System.Collections;			// You will need this for IEnumerator, which Coroutines use
using System.Collections.Generic;

public interface ITrackerDisplay
{
	void ConsumeColor(Color inputColor);
	void ResetDisplay();
	void AddChild(GameObject child);
}

public class TrackerBar : MonoBehaviour, IObserver<BeatData>
{
	[SerializeField]
	private int trackerCellCount = 32;
	[SerializeField]
	private Vector3 trackerWidth;

	[SerializeField]
	private GameObject TrackerCellPrefab;

	[SerializeField]
	private Color mainColor;
	[SerializeField]
	private Color alternateColor;

	[SerializeField]
	private float tickTime = 0.5f;

	private TrackerList<TrackerData> trackerList;

	[SerializeField]
	private int writeNodeValue = 16;

	[SerializeField]
	private int enemyNodeValue = 24;

	[SerializeField]
	private int resolveNodeValue = 8;

	private Vector3 trackerPoint;

	//private Enemy mainEnemy;
	//private Resolver mainResolver;

	private Vector3 currentPosition;
	private Vector3 targetPosition;
	private float targetGoalTime;
	private float timeMotionStart = 0f;
	private float timeInMotion = 0f;

	private Color lastNodeColor;

	private Vector3 trackerInitResetPoint;

	[SerializeField]
	private AudioSource myAudio;

	private float timeAccumulator;
	public float SamplesPerBeat { get; private set; }

	private NoodleMain noodleMain;

	[SerializeField]
	private bool smoothBar = true;

	void Start()
	{
		if (noodleMain == null)
		{
			noodleMain = NoodleMain.GetSingleRef();
		}

		trackerList = new TrackerList<TrackerData>();

		//mainEnemy = new Enemy();
		//mainResolver = new Resolver();

		trackerInitResetPoint = transform.position;
		targetPosition = trackerInitResetPoint;


		trackerPoint = Vector3.zero;
		bool altColor = false;
		for (int j = 0; j < trackerCellCount; j++)
		{
			// Create all our tracker cells from the prefab:
			GameObject createdObject = Instantiate(TrackerCellPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			// Parent them to us so they are held within us in the heirarchy
			// (There is also a SetParent method but this is mostly useful to apply aspects of our transform)
			// (I'm always just lazy and reposition it after creation:)
			createdObject.transform.parent = transform;

			createdObject.transform.localPosition = trackerPoint;
			// spaced out by the specified width
			trackerPoint += trackerWidth;

			// Create an instance of our trackerData struct
			var data = new TrackerData();
			// and populate it
			data.obj = createdObject;
			//data.trackerCell = createdObject.GetComponent<TrackerCell>();
			data.trackerDisplay = createdObject.GetComponent(typeof(ITrackerDisplay)) as ITrackerDisplay;

			// Alternate the background colour of the bar for every other cell
			if (data.trackerDisplay != null)
			{
				var selectedColor = mainColor;
				if (altColor)
					selectedColor = alternateColor;
				data.trackerDisplay.ConsumeColor(selectedColor);
				altColor = !altColor;
			}

			// Add this data to the end of the linked list
			trackerList.Add(data);
		}
		
	}


	void Update()
	{
		/*
		if (Input.GetKeyDown(KeyCode.Z))
		{
			trackerList[writeNodeValue].trackerDisplay.AddChild(noodleMain.createdObjects["player_a"]);
		}
		if (Input.GetKeyDown(KeyCode.X))
		{
			trackerList[writeNodeValue].trackerDisplay.AddChild(noodleMain.createdObjects["player_b"]);
		}
		if (Input.GetKeyDown(KeyCode.C))
		{
			trackerList[writeNodeValue].trackerDisplay.AddChild(noodleMain.createdObjects["player_x"]);
		}
		if (Input.GetKeyDown(KeyCode.V))
		{
			trackerList[writeNodeValue].trackerDisplay.AddChild(noodleMain.createdObjects["player_y"]);
		}
		 */


		// interpolating between locations:
		if (targetGoalTime != 0)
		{
			timeInMotion = Time.time  - timeMotionStart;

			float ratio = Mathf.Clamp(timeInMotion / targetGoalTime,0f,1f);

			ratio = TrackerCurve(ratio);

			transform.position = Vector3.Lerp(currentPosition, targetPosition, ratio);

			if (ratio == 1f)
				targetGoalTime = 0;
		}

	}

	private void MoveTracker(Vector3 amount, float time)
	{
		transform.position = targetPosition;

		if (targetPosition.x < -100f)
		{
			TrackerFullReset();
		}

		timeMotionStart = Time.time;;
		
		currentPosition = transform.position;
		targetPosition = currentPosition + amount;

		targetGoalTime = time;
	}

	public float TrackerCurve(float ratio)
	{
		if (smoothBar)
			return ratio;
		float cosWave = (1f + Mathf.Cos((1 + ratio) * Mathf.PI)) / 2;
		cosWave *= cosWave;
		return cosWave;
	}

	public void TrackerFullReset()
	{
		// TODO: we would need to reset the position of all of the objects here

		var tempObjectList = new List<Transform>();

		while (transform.childCount != 0)
		{
			Transform childTrans = transform.GetChild(0);
			childTrans.parent = null;
			tempObjectList.Add(childTrans);
		}

		trackerPoint += transform.position - trackerInitResetPoint;


		transform.position = trackerInitResetPoint;

		foreach (var rechild in tempObjectList)
		{
			rechild.parent = transform;
		}
	}


	private void Tick()
	{
		MoveTracker(-trackerWidth, tickTime);

		// take the first node and then remove it
		var firstNode = trackerList[0];

		trackerList.Step();

		// reposition it to the end of the pile:
		firstNode.obj.transform.localPosition = trackerPoint;
		trackerPoint += trackerWidth;

		firstNode.trackerDisplay.ResetDisplay();

		// Enemy stuff. No idea what an enemy needs to know yet.
		//var enemyAction = mainEnemy.Step(trackerList[enemyNodeValue]);
		//trackerList[enemyNodeValue].trackerDisplay.AddChild(noodleMain.createdObjects[enemyAction]);

		// Resolution stuff. Things get resolved here?!
		//mainResolver.Step(trackerList[resolveNodeValue]);
	}

	public void AddChild(int position, GameObject child)
	{
		trackerList[position].trackerDisplay.AddChild(child);
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
		Tick();
	}


	private IDisposable unsubscriber;
	public void Setup(IObservable<BeatData> beatObserver, float tickTime)
	{
		unsubscriber = beatObserver.Subscribe(this);
		this.tickTime = tickTime;
	}
}
