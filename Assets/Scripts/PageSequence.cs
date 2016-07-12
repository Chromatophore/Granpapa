using UnityEngine;
using System.Collections;	// Needed for Array stuff?
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class PageTrackerData
{
	public string enemy;
	public string player;
	public bool success;

	public PageTrackerData()
	{
		Reset();
	}

	public void Reset()
	{
		enemy = "";
		player = "";
		success = false;
	}
}

public class PageSequence : MonoBehaviour, IObservable<BeatData>
{
	// Things that should probably be set elsewhere:
	private int trackerCellCount = 32;
	private int enemyNodeValue = 24;
	private int playerNodeValue = 16;
	private int resolveNodeValue = 8;

	// Audio tracks associated with this game:
	[SerializeField]
	private SongStruct[] audioList;
	private SongStruct currentAudio;
	[SerializeField]
	private AudioSource attachedAudioSource;

	// List of all pages:
	private List<Page> pageList;
	// Which page are we on?
	private int currentPage;

	// Full tracker bar list
	private TrackerList<PageTrackerData> trackerList;


	// Input to Concept system:
	//[SerializeField]
	//private ButtonConceptMap[] serializedPlayerInputConcept;
	private Dictionary<BUTTON, string[]> playerInputConceptDict;

	// This is the area that will deal with the music of the level:

	// A tracker bar game object?
	[SerializeField]
	private TrackerBar trackerBar;
	private int lastSamples = 0;
	private int beatNumber = 0;
	private float SamplesPerBeat = 0f;
	private int nextSampleValue;
	[SerializeField]
	private int currentInputBeat = 0;	// The beat of the track that we would input for from 0 to the length of the song - not very useful
	private int inputFudgeOffset = 0;	// a Fudge value that we can use in indexing calculations for input:
	// if NextBeatFudge is 0.5 will be 0 when we are from 0->50% through a beat, and will be 1 from 50% ->100% through
	[SerializeField]
	private float inputNextBeatFudge = 0.5f;


	private NoodleMain noodleMain;

	void Start()
	{
		if (noodleMain == null)
		{
			noodleMain = NoodleMain.GetSingleRef();
		}

		pageList = new List<Page>();
		currentPage = 0;

		Page TestPage = new Page();
		pageList.Add(TestPage);

		trackerList = new TrackerList<PageTrackerData>();
		for (int i = 0; i < trackerCellCount; i++)
		{
			trackerList.Add(new PageTrackerData());
		}

		/*
		playerInputConceptDict = new Dictionary<BUTTON, string>();
		foreach (var map in serializedPlayerInputConcept)
		{
			playerInputConceptDict[map.inputButton] = map.concept;
		}
		 * */


		currentAudio = audioList[0];
		attachedAudioSource.clip = currentAudio.audioTrack;
		SamplesPerBeat = 44100f * currentAudio.beatTime;
		attachedAudioSource.Play();

		nextSampleValue += (int)SamplesPerBeat;


		trackerBar.Setup(this, currentAudio.beatTime);
	}

	void Update()
	{
		UpdateBeatBox();
	}



	public void PlayerInput(BUTTON inputButton)
	{
		if (playerInputConceptDict == null) { return; }
		string[] concept;

		bool success = playerInputConceptDict.TryGetValue(inputButton, out concept);

		if (success)
		{
			// this button exists and the concept string is in concept
			//Debug.Log(string.Format("Player inputted button: {0} and got concept: {1}", inputButton, concept));
			ConceptConsumer(0, concept);
		}
	}

	private void ConceptConsumer(int actorID, string[] inputConcept)
	{
		if (actorID == 0)
		{
			int i = 0 + inputFudgeOffset;
			foreach (var move in inputConcept)
			{
				if (trackerList[playerNodeValue + i].player == "")
				{
					trackerList[playerNodeValue + i].player = move;
					trackerBar.AddChild(playerNodeValue + i - 1, noodleMain.GetPrefab(move));
					i++;
				}
				else
				{
					break;
				}
			}
		}
	}

	private float timeLast = 0f;
	private void UpdateBeatBox()
	{
		int currentSamples = attachedAudioSource.timeSamples;
		int difference = currentSamples - lastSamples;

		float beatRatio = (inputNextBeatFudge + (currentSamples / SamplesPerBeat));
		currentInputBeat = (int)beatRatio;
		if (currentInputBeat != beatNumber)
			inputFudgeOffset = 1;
		else
			inputFudgeOffset = 0;

		if (currentSamples >= nextSampleValue || (difference < 0))
		{
			inputFudgeOffset = 0;
			bool debug = false;
			if ((Time.time - timeLast) < 0.2f)
			{
				debug = true;
			}

			if (debug)
				Debug.Log(currentSamples.ToString() + " Beat difference vs Samples: " + difference.ToString());


			if (difference < 0)
				beatNumber = 0;
			else
				beatNumber++;
			
			nextSampleValue = (int)((beatNumber + 1) * SamplesPerBeat);

			if (nextSampleValue >= currentAudio.audioTrack.samples)
			{
				// don't really need to do anything
			}

			int beatInBar = beatNumber % currentAudio.beatsPerBar;
			int beatIn2Bar = beatNumber % (currentAudio.beatsPerBar * 2);

			var lastEntry = trackerList.Step();
			lastEntry.Reset();

			if (beatIn2Bar == 0)
			{
				var attackList = pageList[currentPage].getAttacks();
				for (int i = 0; i < attackList.Count; i++)
				{
					trackerList[enemyNodeValue + i].enemy = attackList[i];
				}
				playerInputConceptDict = pageList[currentPage].getPlayerInputConceptDict();
			}

			if (debug)
				Debug.Log(beatNumber.ToString() + " " + trackerList[0].enemy + " time: " + Time.time + " " + (Time.time - timeLast).ToString());

			timeLast = Time.time;
			trackerBar.AddChild(enemyNodeValue, noodleMain.GetPrefab(trackerList[enemyNodeValue].enemy));

			pageList[currentPage].CheckSuccess(trackerList[playerNodeValue - 1]);

			if (trackerList[resolveNodeValue].enemy != "")
			{
				if (trackerList[resolveNodeValue].success == false)
				{
					trackerBar.AddChild(resolveNodeValue, noodleMain.GetPrefab("fail"));
				}
				else
				{
					trackerBar.AddChild(resolveNodeValue, noodleMain.GetPrefab("success"));
				}
			}

			if (beatDataObservers != null)
			{
				var dataPacket = new BeatData(beatNumber, beatInBar);

				foreach (var beatListener in beatDataObservers)
				{
					beatListener.OnNext(dataPacket);
				}
			}

			lastSamples = currentSamples;

			// Why is it so hard to debug this trackerList ;-;
			/*
			string tldebug = "";
			for (int i = 0; i < trackerCellCount; i++)
			{
				tldebug += trackerList[i].enemy + "/" + trackerList[i].player + ", ";	
			}
			Debug.Log(tldebug);
			 */
		}
	}

	private HashSet<IObserver<BeatData>> beatDataObservers;
	public IDisposable Subscribe(IObserver<BeatData> observer)
	{
		// Generate list for game setting observers
		if (beatDataObservers == null)
			beatDataObservers = new HashSet<IObserver<BeatData>>();

		beatDataObservers.Add(observer);

		// Now, return an IDisposable implimentation to whatever called us so that it can unsubscribe from our updates:
		return new GenericUnsubscriber<BeatData>(beatDataObservers, observer);
	}
}