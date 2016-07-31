using UnityEngine;
using System.Collections;	// Needed for Array stuff?
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public enum DATAMARKER
{
	NONE,
	START,
	END,
	SYNC
}

public class PageSequence : MonoBehaviour, IObservable<BeatData>
{
	// Things that should probably be set elsewhere:
	private int trackerCellCount = 128;
	private int playerNodeValue = 64;

	// these values are informed by the bar length:
	private int enemyNodeValue = -1;
	private int resolveNodeValue = -1;
	private int nextResolveNodeValue = -1;

	// Audio tracks associated with this game:
	private SongStruct currentAudio;
	[SerializeField]
	private AudioSource attachedAudioSource;
	[SerializeField]
	private AudioSource additionalAudio;

	// List of all pages:
	private List<Page> pageList;
	// Which page are we on?
	private int currentPageIndex;
	private Page currentPage;

	// Full tracker bar list
	private TrackerList<PageTrackerData> trackerList;

	public MoveAnimatorB granpapaAnim;
	public MoveAnimatorB kidAnim;

	private int beatInPhaseNumber = 0;

	// Input to Concept system:
	//[SerializeField]
	//private ButtonConceptMap[] serializedPlayerInputConcept;
	private Dictionary<BUTTON, string[]> playerInputConceptDict;
	private Dictionary<string, string[]> mainSoundDict;
	private Dictionary<BUTTON, string> pageActiveInputDict;

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
	public string gameString;

	[SerializeField]
	private UIScoreDisplay scoreDisplay;

	[SerializeField]
	private UIButtonDisplay buttonDisplay;

	private bool lastBreak = false;

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
		for (int i = 0; i < trackerCellCount; i++)
		{
			trackerList[i].next = trackerList[i + 1];
		}

		Level TestLevel = new Level();

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

		Tempo.RegisterBeatBox(this);
	}

	private void NextPage()
	{
		Debug.Log("Moving on to the next page.");

		// inform previous page that it's task is completed
		if (currentPage != null)
			currentPage.Reset();

		currentPageIndex++;
		if (currentPageIndex == pageList.Count)
		{
			Debug.Log("Yay, we beat the level!");
			currentPageIndex = 0;
		}

		currentPage = pageList[currentPageIndex];

		// Tell the current page that it is being accessed
		currentPage.UpNext();
	}

	private void StartLevel(Level thisLevel)
	{
		Debug.Log("Starting level...");

		gameString = thisLevel.gameName;

		pageList = thisLevel.getPages();

		if (pageList.Count == 0)
		{
			Debug.Log("No page data!");
			return;
		}
		currentPageIndex = -1;
		//currentPage = pageList[currentPageIndex];

		// select audio from audiolist for this page sequence:

		SetAudio(thisLevel.levelAudio);
	}

	private void SetAudio(SongStruct inputAudio)
	{
		currentAudio = inputAudio;
		attachedAudioSource.clip = currentAudio.audioTrack;

		float tempo = 60f / currentAudio.beatTime;
		Tempo.SetTempo((int)tempo);

		// configure beat calcuation values
		SamplesPerBeat = 44100f * currentAudio.beatTime;
		nextSampleValue += (int)SamplesPerBeat;

		SetPhaseLength(4);

		// Setup the tracker bar for this audio:
		trackerBar.Setup(this, currentAudio.beatTime);

		// start the song
		attachedAudioSource.Play();
		// prejig a datamarker to achieve results:
		trackerList[playerNodeValue].resolution = DATAMARKER.END;
		NewBeat();
	}

	
	private void SetPhaseLength(int beats, int resolveBeats = -1)
	{
		if (resolveBeats == -1)
			resolveBeats = beats;
		// configure nodes to write/read from based on bar length:
		enemyNodeValue = playerNodeValue + beats;
		nextResolveNodeValue =  playerNodeValue - resolveBeats;

		if (resolveNodeValue == -1)
			resolveNodeValue = nextResolveNodeValue;

		trackerBar.SetBeatsPerPhase(beats);
		gameDisplay.SetBeatsPerPhase(beats);
	}

	public void PlayerInput(BUTTON inputButton)
	{
		if (playerInputConceptDict == null) { return; }
		if (pageActiveInputDict == null || !pageActiveInputDict.ContainsKey(inputButton)) { return; }
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
			int sequenceNumber = 0;

			if (inputConcept.Length > 1 && trackerList[playerNodeValue + inputConcept.Length - 1 + i].active == false)
			{
				return;
			}
			foreach (var move in inputConcept)
			{
				var node = trackerList[playerNodeValue + i];
				if (node.active && node.player == "")
				{
					string moveFeedback = move;
					node.originPage.ProcessConcept(node, ref moveFeedback, sequenceNumber);

					if (move != "")
						trackerBar.AddChild(playerNodeValue + i, noodleMain.GetPrefab(moveFeedback));

					sequenceNumber++;
					i++;
				}
				else
				{
					break;
				}
			}

			// play the animation associated with this:
			granpapaAnim.Play(gameString, inputConcept[0]);

			if (trackerList[playerNodeValue + inputFudgeOffset].originPage != null)
			{
				granpapaAnim.MakeSound(noodleMain.GetClip(trackerList[playerNodeValue + inputFudgeOffset].originPage.ResolveSound(inputConcept[0])));
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

		// We need to update our tracker list to tell it that we are in a new beat, such that 0 now corresponds to the current beat that is occuring at this moment
		var lastEntry = trackerList.Step(); // (step returns an instance of its template class)
		// and reset the last value's information
		lastEntry.Reset();

		///////////////////
		/// Below this point, all our internal structures should have been stepped/informed of the change in indexes etc that needed to have occured.

		// Section to perform as THE START OF THE NEXT BEAT
		int beatInBar = beatNumber % currentAudio.beatsPerBar;
		//int beatsPerPhase = phaseBarValue * currentAudio.beatsPerBar;
		//int beatInBar = (beatInPhaseNumber) % beatsPerPhase;
		//int beatIn2Bar = (beatInPhaseNumber) % (beatsPerPhase * 2);

		// We must first attempt to resolve any data sequences that are now in the resolution zone
		// We can either assess a whole bar in one go, based on if we are set to use DataMarkers
		var assessPage = trackerList[resolveNodeValue].originPage;
		if (assessPage != null)
		{
			if (assessPage.useDataMarkers && trackerList[resolveNodeValue].resolution == DATAMARKER.START)
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
				assessPage.AssessSequence(dataSequence);
			}
			// Or, we can assess as the events pass behind us, which allows us to move the kid closer to grandpapa
			else if (!assessPage.useDataMarkers)
			{
				// Check the previous entry once it has left us
				assessPage = trackerList[playerNodeValue - 1].originPage;
				assessPage.CheckSuccess(trackerList[ playerNodeValue - 1]);
			}
		}
		
		var playerNode = trackerList[playerNodeValue];

		// if we are midway through the current point, ie, we are transitioning from the end of our input to the player playing.
		// This will be the FIRST BEAT of the player having to perform actions

		// if the previous node was the end of a pattern, we need to
		// a) worry about the next pattern
		bool dealWithNewAttacks = (trackerList[ playerNodeValue - 1].resolution == DATAMARKER.END);

		if (playerNode.originPage != null && lastBreak != playerNode.originPage.NoBreaks)
		{
			if (lastBreak == false)
			{
				dealWithNewAttacks = true;
				lastBreak = true;
			}
			else
				lastBreak = false;

		}

		int newEnemyDisplayStart = -1;
		int newEnemyDisplayCount = -1;

		if (dealWithNewAttacks)
		//if (beatIn2Bar == beatsPerPhase)
		{
			// if the current page feels that it is completed, we will move to the next page:
			if (currentPage == null || currentPage.Complete || (currentPage.NoBreaks))
			{
				if (currentPage != null)
				{
					scoreDisplay.PageScore(currentPage.AssessScore());
				}
				NextPage();
			}

			if (currentPage != null)
				currentPage.UpNext();

			// Load the input map from this page, in case it chaneges:
			playerInputConceptDict = currentPage.GetPlayerInputConceptDict();
			var inputcheck = currentPage.GetPageActiveInputDict();
			if (inputcheck != null)
			{
				pageActiveInputDict = inputcheck;
				buttonDisplay.ParseActiveInputDict(pageActiveInputDict);
			}
			mainSoundDict = currentPage.GetMainSoundDict();

			// Load some attacks from page:
			var attackList = currentPage.GetAttacks();

			// increase attack length if needed so that it is a multiple of a length of a bar?
			// maybe not?
			int attackLength = attackList.Count;
			//while (attackLength % currentAudio.beatsPerBar != 0)
			//	attackLength++;
			// Now we have the length of the attack phase

//			Debug.Log("Attack patern length: " + attackLength);

			// Insert data into PageTrackerDatas, from enemy point onwards

			int enemyWriteLocation = trackerList[playerNodeValue].offsetToEndOfSequence;
			if (enemyWriteLocation == -1)
			{
				enemyWriteLocation = playerNodeValue;
			}
			else
			{
				enemyWriteLocation += playerNodeValue;
			}

			// we need to be multiple ahead during no break mode
			bool multipleAhead = trackerList[enemyWriteLocation].active;
			bool abandonPass = false;
			if (multipleAhead)
			{
				if (currentPage.NoBreaks == true)
				{
					while (trackerList[enemyWriteLocation].active == true)
					{
						enemyWriteLocation += trackerList[enemyWriteLocation].offsetToEndOfSequence;
					}
				}
				else
					abandonPass = true;	// if we can have breaks then we do not need this
			}

			if (!abandonPass)
			{
				newEnemyDisplayStart = enemyWriteLocation;
				newEnemyDisplayCount = attackLength;

				PageTrackerData lastAccessed = null;
				PageTrackerData dataStartPoint = null;
				int cap = 2 * attackLength;
				bool breaks = true;
				if (currentPage.NoBreaks)
				{
					breaks = false;
					cap = attackLength;
					SetPhaseLength(attackLength, 0);
				}
				else
				{
					SetPhaseLength(attackLength);
				}

				int offsetRemaining = cap;
				

				for (int i = 0; i < cap; i++)
				{
					var thisIndex = enemyWriteLocation + i;
					var thisNode = trackerList[thisIndex];

					if (i == 0)
					{
						thisNode.resolution = DATAMARKER.START;
						dataStartPoint = thisNode;
					}

					if (i < attackList.Count || breaks == false)
						thisNode.enemy = attackList[i];

					if (i < attackLength || breaks == false)
					{
					
						thisNode.active = true;
						thisNode.phaseLength = attackLength;
					}
					thisNode.dataStartPoint = dataStartPoint;
					thisNode.offsetToEndOfSequence = offsetRemaining;
					thisNode.sequenceNumber = i;

					//Debug.Log("Setting index " + thisIndex + " offset to " + offsetRemaining);

					if (i == attackLength - 1)
						thisNode.resolution = DATAMARKER.END;

					trackerBar.SetCellActive(thisIndex);

					thisNode.originPage = currentPage;



					offsetRemaining--;
				}


				//trackerList[enemyWriteLocation + cap].resolution = DATAMARKER.SYNC;
			}
		}

		// NOW THAT ALL OF OUR DATA IS UPDATED:
		// We can perform graphical and other cosmetic changes.
		var resolveNode = trackerList[resolveNodeValue];

		if (playerNode.resolution == DATAMARKER.START)
		{
			if (playerNode.phaseLength != -1)
			{
				//SetPhaseLength(playerNode.phaseLength);
			}
		}
		
		// NOW we will inform all our observers that a new beat has occured so that they are also aware of it
		if (beatDataObservers != null)
		{
			var dataPacket = new BeatData(beatNumber, beatInBar, currentAudio.beatTime, resolveNode.active);

			foreach (var beatListener in beatDataObservers)
			{
				beatListener.OnNext(dataPacket);
			}

			if (beatDataObserversToRemove.Count != 0)
			{
				foreach (var beatListenerToRemove in beatDataObserversToRemove)
				{
					beatDataObservers.Remove(beatListenerToRemove);
				}
				beatDataObserversToRemove.Clear();
			}
		}

		if (dealWithNewAttacks)
		{
			int enemyStartIndex = newEnemyDisplayStart;
			for (int i = 0; i < newEnemyDisplayCount; i++)
			{
				var thisIndex = enemyStartIndex + i;
				string attack = trackerList[thisIndex].enemy;
				if (gameDisplay != null)
					gameDisplay.SetUpcomingAttack(0, attack, i == 0);

				// Draw the attacks onto the tracker bar
				// Option 1: Display all enemy attacks at once:
				if (attack != "")
					trackerBar.AddChild(thisIndex, noodleMain.GetPrefab(attack));

				if (i == 0)
				{
					trackerBar.AddChild(thisIndex, noodleMain.GetPrefab("barstart"));
				}
				else if (i + 1 == newEnemyDisplayCount)
				{
					trackerBar.AddChild(thisIndex, noodleMain.GetPrefab("barend"));
				}
			}
		}

		// Option 2: Add enemy attacks as they scroll past enemyNode:
		//trackerBar.AddChild(enemyNodeValue, noodleMain.GetPrefab(trackerList[enemyNodeValue].enemy));
		if (playerNode.enemy == "cutscene" && playerNode.resolution == DATAMARKER.START)
		{
			CutscenePage cutscenePage = currentPage as CutscenePage;

			if (cutscenePage != null)
			{
				cutscenePage.ActivateBeat(this);
				string customAudioName = cutscenePage.GetCustomAudio();
				if (customAudioName != "")
				{
					AudioClip customClip = noodleMain.GetClip(customAudioName);
					if (customClip != null)
					{
						additionalAudio.clip = customClip;
						additionalAudio.Play();
					}
				}
			}
		}

		// For the node that is in the resolve location, reveal the resolution outcome:
		if (resolveNode.enemy != "")
		{
			if (resolveNode.originPage.DisplaySuccess)
			{
				if (resolveNode.success == false)
				{
					trackerBar.AddChild(resolveNodeValue, noodleMain.GetPrefab("fail"));
				}
				else
				{
					trackerBar.AddChild(resolveNodeValue, noodleMain.GetPrefab("success"));
				}
			}

			if (resolveNode.enemy == "cutscene")
			{
				CutscenePage cutscenePage = resolveNode.originPage as CutscenePage;

				if (cutscenePage != null)
				{
					cutscenePage.ResolveBeat();
				}
			}
		}

//		Debug.Log(trackerList[resolveNodeValue]);
		if (resolveNode.player != "")
		{
			kidAnim.Play(gameString, resolveNode.player);
		}

		if (resolveNode.active)
		{
			gameDisplay.Beat(currentAudio.beatTime, resolveNode);
			resolveNode.active = false;
		}

		// if there is a pending resolution node change:
		if (resolveNodeValue != nextResolveNodeValue)
		{
			if (trackerList[resolveNodeValue].resolution == DATAMARKER.END)
			{
				resolveNodeValue = nextResolveNodeValue;
			}
		}
		//Debug.Log(playerNode);

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
	private HashSet<IObserver<BeatData>> beatDataObserversToRemove;
	public IDisposable Subscribe(IObserver<BeatData> observer)
	{
		// Generate list for game setting observers
		if (beatDataObservers == null)
			beatDataObservers = new HashSet<IObserver<BeatData>>();
		// Generate list for game setting observers
		if (beatDataObserversToRemove == null)
			beatDataObserversToRemove = new HashSet<IObserver<BeatData>>();

		beatDataObservers.Add(observer);

		// Now, return an IDisposable implimentation to whatever called us so that it can unsubscribe from our updates:
		return new GenericUnsubscriber<BeatData>(beatDataObserversToRemove, observer, true);
	}
}