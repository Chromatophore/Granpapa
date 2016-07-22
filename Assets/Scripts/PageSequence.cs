using UnityEngine;
using System.Collections;	// Needed for Array stuff?
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public enum DATAMARKER
{
	NONE,
	START,
	END
}

public class PageTrackerData
{
	public string enemy;
	public string player;
	public bool success;
	public bool active;
	public DATAMARKER resolution;

	public string playerAnimation;

	public PageTrackerData dataStartPoint;

	public PageTrackerData()
	{
		Reset();
	}

	public void Reset()
	{
		enemy = "";
		player = "";
		success = false;
		active = false;
		resolution = DATAMARKER.NONE;
		dataStartPoint = null;
	}

	public override string ToString()
	{
		return string.Format("'{0}' v '{1}': {2}", enemy, player, playerAnimation);
	}
}

public class PageSequence : MonoBehaviour, IObservable<BeatData>
{
	// Things that should probably be set elsewhere:
	private int trackerCellCount = 32;
	private int playerNodeValue = 16;

	// these values are informed by the bar length:
	private int enemyNodeValue = -1;
	private int resolveNodeValue = -1;

	// Audio tracks associated with this game:
	[SerializeField]
	private SongStruct[] audioList;
	private SongStruct currentAudio;
	[SerializeField]
	private AudioSource attachedAudioSource;

	// List of all pages:
	private List<Page> pageList;
	// Which page are we on?
	private int currentPageIndex;
	private Page currentPage;

	// Full tracker bar list
	private TrackerList<PageTrackerData> trackerList;

	public Animator kidAnim;


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

	public IGameDisplay gameDisplay;

	void Start()
	{
		if (noodleMain == null)
		{
			noodleMain = NoodleMain.GetSingleRef();
		}

		pageList = new List<Page>();
		currentPageIndex = 0;

		trackerList = new TrackerList<PageTrackerData>();
		for (int i = 0; i < trackerCellCount; i++)
		{
			trackerList.Add(new PageTrackerData());
		}


		Level TestLevel = new Level(this);

		StartCoroutine(LoadNewLevelDelayed(1f, TestLevel));

	}

	void Update()
	{
		UpdateBeatBox();
	}

	private IEnumerator LoadNewLevelDelayed(float delay, Level levelToLoad)
	{
		yield return new WaitForSeconds(delay);
		StartLevel(levelToLoad);
	}

	private void NextPage()
	{
		Debug.Log("Moving on to the next page.");

		// inform previous page that it's task is completed
		currentPage.Reset();

		currentPageIndex++;
		if (currentPageIndex == pageList.Count)
		{
			Debug.Log("Yay, we beat the level!");
			currentPageIndex = 0;
		}

		currentPage = pageList[currentPageIndex];
		currentPage.Setup(this, this);
	}

	private void StartLevel(Level thisLevel)
	{
		Debug.Log("Starting level...");

		pageList = thisLevel.getPages();

		if (pageList.Count == 0)
		{
			Debug.Log("No page data!");
			return;
		}
		currentPageIndex = 0;
		currentPage = pageList[currentPageIndex];

		// select audio from audiolist for this page sequence:
		//currentAudio = audioList[0];
		currentAudio = thisLevel.levelAudio;
		attachedAudioSource.clip = currentAudio.audioTrack;

		// configure beat calcuation values
		SamplesPerBeat = 44100f * currentAudio.beatTime;
		nextSampleValue += (int)SamplesPerBeat;

		// configure nodes to write/read from based on bar length:
		enemyNodeValue = playerNodeValue + currentAudio.beatsPerBar;
		resolveNodeValue = playerNodeValue - currentAudio.beatsPerBar;

		// Setup the tracker bar for this audio:
		trackerBar.Setup(this, currentAudio.beatTime);

		// start the son
		attachedAudioSource.Play();
		NewBeat();
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
					trackerBar.AddChild(playerNodeValue + i, noodleMain.GetPrefab(move));
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
		if (pageList == null || pageList.Count == 0) { return; }
		int currentSamples = attachedAudioSource.timeSamples;
		int difference = currentSamples - lastSamples;

		float beatRatio = (inputNextBeatFudge + (currentSamples / SamplesPerBeat));
		currentInputBeat = (int)beatRatio;
		if (currentInputBeat != beatNumber)
			inputFudgeOffset = 1;
		else
			inputFudgeOffset = 0;

		// calculate if a new beat has occured yet:
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
			lastSamples = currentSamples;

			if (debug)
				Debug.Log(beatNumber.ToString() + " " + trackerList[0].enemy + " time: " + Time.time + " " + (Time.time - timeLast).ToString());
			timeLast = Time.time;

			// At this point, we are at the very beginning of the 'next' beat, which is now the value stored in beatnumber
			NewBeat();

		}

	}

	// this method is called each time a new beat is detected. It is reponsible for first UPDATING everything, and then also doing certain behaviors
	private void NewBeat()
	{
		// Section to perform as THE END OF THE CURRENT BEAT



		// Section to perform as THE START OF THE NEXT BEAT

		int beatsPerBar = currentAudio.beatsPerBar;
		int beatInBar = (beatNumber) % beatsPerBar;
		int beatIn2Bar = (beatNumber) % (beatsPerBar * 2);

		// We need to update our tracker list to tell it that we are in a new beat, such that 0 now corresponds to the current beat that is occuring at this moment
		var lastEntry = trackerList.Step(); // (step returns an instance of its template class)
		// and reset the last value's information
		lastEntry.Reset();

		///////////////////
		/// Below this point, all our internal structures should have been stepped/informed of the change in indexes etc that needed to have occured.

		// We must first attempt to resolve any data sequences that are now in the resolution zone
		if (currentPage.useDataMarkers && trackerList[resolveNodeValue].resolution == DATAMARKER.START)
		{
			var dataSequence = new List<PageTrackerData>();
			int index = resolveNodeValue;
			var resolutionState = DATAMARKER.START;

			while (resolutionState != DATAMARKER.END)
			{
				resolutionState = trackerList[index].resolution;
				dataSequence.Add(trackerList[index++]);
			}

			Debug.Log(EasyPrint.MakeString(dataSequence));
			currentPage.AssessSequence(dataSequence);
		}
		else if (!currentPage.useDataMarkers)
		{
			// Check the previous entry once it has left us
			currentPage.CheckSuccess(trackerList[ playerNodeValue - 1] );
		}

		
		// if we are midway through the current point, ie, we are transitioning from the end of our input to the player playing.
		// This will be the FIRST BEAT of the player having to perform actions
		if (beatIn2Bar == beatsPerBar)
		{

			// Actually let's just check the success of input from start to finish all in go so we don't need to subscribe or not really actually
			// know what our data is and so on and so forth.
			//currentPage.CheckSuccess(trackerList[playerNodeValue - 1]);
			

			// if the current page feels that it is completed, we will move to the next page:
			Debug.Log(currentPage.Complete);
			if (currentPage.Complete)
			{
				NextPage();
			}

			// Tell the current page that it is being accessed
			currentPage.UpNext();

			// Load the input map from this page, in case it chaneges:
			playerInputConceptDict = currentPage.GetPlayerInputConceptDict();

			// Load some attacks from page:
			var attackList = currentPage.GetAttacks();

			// Insert data into PageTrackerDatas, from enemy point onwards
			PageTrackerData lastAccessed = null;
			PageTrackerData dataStartPoint = null;
			for (int i = 0; i < attackList.Count; i++)
			{
				var thisIndex = enemyNodeValue + i;
				var thisNode = trackerList[thisIndex];

				if (i == 0)
				{
					thisNode.resolution = DATAMARKER.START;
					dataStartPoint = thisNode;
				}

				thisNode.enemy = attackList[i];
				thisNode.dataStartPoint = dataStartPoint;
				thisNode.active = true;
				lastAccessed = thisNode;
			}

			if (lastAccessed != null)
				lastAccessed.resolution = DATAMARKER.END;

		}

		// NOW THAT ALL OF OUR DATA IS UPDATED:
		// We can perform graphical and other cosmetic changes.
		
		// NOW we will inform all our observers that a new beat has occured so that they are also aware of it
		if (beatDataObservers != null)
		{
			var dataPacket = new BeatData(beatNumber, beatInBar, currentAudio.beatTime, trackerList[resolveNodeValue].active);

			foreach (var beatListener in beatDataObservers)
			{
				beatListener.OnNext(dataPacket);
			}
		}

		if (beatIn2Bar == beatsPerBar)
		{
			for (int i = 0; i < beatsPerBar; i++)
			{
				var thisIndex = enemyNodeValue + i;
				string attack = trackerList[thisIndex].enemy;
				if (gameDisplay != null)
					gameDisplay.SetUpcomingAttack(0, attack);

				// Draw the attacks onto the tracker bar
				// Option 1: Display all enemy attacks at once:
				if (attack != "")
					trackerBar.AddChild(thisIndex, noodleMain.GetPrefab(attack));
			}

		}

		// Option 2: Add enemy attacks as they scroll past enemyNode:
		//trackerBar.AddChild(enemyNodeValue, noodleMain.GetPrefab(trackerList[enemyNodeValue].enemy));


		// For the node that is in the resolve location, reveal the resolution outcome:
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

//		Debug.Log(trackerList[resolveNodeValue]);
		gameDisplay.PassPlayerAnimation(trackerList[resolveNodeValue].playerAnimation);
		if (trackerList[resolveNodeValue].player == "jump")
		{
			kidAnim.CrossFade("Jump", 0.1f, 1, 0f);
			kidAnim.SetLayerWeight(1, 1.0f);
		}







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