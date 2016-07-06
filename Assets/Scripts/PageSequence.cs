using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations



public class PageSequence : MonoBehaviour, IObservable<BeatData>
{
	// Audio tracks associated with this game:
	[SerializeField]
	private SongStruct[] audioList;
	private SongStruct currentAudio;
	[SerializeField]
	private AudioSource attachedAudioSource;

	// List of all pages:
	private List<Page> pageList;


	// Input to Concept system:
	[SerializeField]
	private ButtonConceptMap[] serializedPlayerInputConcept;
	private Dictionary<BUTTON, string> playerInputConceptDict;

	// This is the area that will deal with the music of the level:

	// A tracker bar game object?
	[SerializeField]
	private TrackerBar trackerBar;
	private int lastSamples = 0;
	private int beatNumber = 0;
	private float nextBeatTime = 0f;
	private float SamplesPerBeat = 0f;

	void Start()
	{
		pageList = new List<Page>();

		playerInputConceptDict = new Dictionary<BUTTON, string>();
		foreach (var map in serializedPlayerInputConcept)
		{
			playerInputConceptDict[map.inputButton] = map.concept;
		}


		currentAudio = audioList[0];
		attachedAudioSource.clip = currentAudio.audioTrack;
		SamplesPerBeat = 44100f * currentAudio.beatTime;
		attachedAudioSource.Play();


		trackerBar.Setup(this);
	}

	void Update()
	{
		UpdateBeatBox();
	}



	public void PlayerInput(BUTTON inputButton)
	{
		string concept;

		bool success = playerInputConceptDict.TryGetValue(inputButton, out concept);

		if (success)
		{
			// this button exists and the concept string is in concept
			Debug.Log(string.Format("Player inputted button: {0} and got concept: {1}", inputButton, concept));
			ConceptConsumer(0, concept);
		}
	}

	private void ConceptConsumer(int actorID, string inputConcept)
	{

	}






	private void UpdateBeatBox()
	{
		nextBeatTime += Time.deltaTime;
		if (nextBeatTime >= currentAudio.beatTime)
		{
			nextBeatTime -= currentAudio.beatTime;

			int currentSamples = attachedAudioSource.timeSamples;

			int difference = currentSamples - lastSamples;
			//Debug.Log("Beat difference vs Samples: " + difference.ToString());

			if (difference >= 0 && difference < 88200)
			{
				beatNumber++;
				float currentBeatNumber = (currentSamples / SamplesPerBeat);
				//Debug.Log("Hone Factor: " + currentBeatNumber.ToString() + " vs " + beatNumber.ToString());
				currentBeatNumber -= beatNumber;
				currentBeatNumber *= currentAudio.beatTime;

				nextBeatTime = currentBeatNumber;
			}
			else
			{
				beatNumber = 0;
			}




			if (beatDataObservers != null)
			{
				int beatInBar = beatNumber % currentAudio.beatsPerBar;
				var dataPacket = new BeatData(beatNumber, beatInBar);

				foreach (var beatListener in beatDataObservers)
				{
					beatListener.OnNext(dataPacket);
				}
			}

			lastSamples = currentSamples;
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