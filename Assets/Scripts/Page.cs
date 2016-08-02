using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class Page
{
	public bool useDataMarkers = false;

	public bool DisplaySuccess { get; protected set; }

	// External objects can call Complete to see if the page feels that it is time to move on
	public bool Complete { get; protected set; }

	protected List<string> enemyAttacks;
	protected Dictionary<BUTTON, string[]> playerInputConceptDict;
	protected Dictionary<string, string[]> mainSoundDict;
	protected Dictionary<string, string[]> customSoundDict;
	protected Dictionary<BUTTON, string> pageActiveInputDict;

	private bool upNext;
	private bool isPlaying;
	private bool hasFailed;
	private int beatCount;
	protected PlayerAnimMap playerAnimMap;
	public int pageScore;
	protected int maxScore;
	protected List<string> autoActions;

	public bool NoBreaks { get; protected set; }

	public virtual void UpNext()
	{
		upNext = true;
		hasFailed = false;
	}
	private void Failure()
	{
		hasFailed = true;
	}

	public Page()
	{
		DisplaySuccess = false;
		Complete = false;
	}

	public virtual void Reset()
	{
		upNext = false;
		isPlaying = false;
		hasFailed = false;
		beatCount = 0;
		pageScore = 0;

		Complete = false;
	}

	public List<string> GetAttacks()
	{
		return enemyAttacks;
	}

	public List<string> GetAutos()
	{
		return autoActions;
	}

	public Dictionary<BUTTON, string[]> GetPlayerInputConceptDict()
	{
		return playerInputConceptDict;
	}

	public Dictionary<BUTTON, string> GetPageActiveInputDict()
	{
		return pageActiveInputDict;
	}

	public Dictionary<string, string[]> GetMainSoundDict()
	{
		return mainSoundDict;
	}

	public void AddCustomSounds(Dictionary<string, string[]> custom)
	{
		customSoundDict = custom;
	}

	public void AssessSequence(ICollection<PageTrackerData> dataSequence)
	{
		// This method receives the whole sequence of information from start to end
		// This gives us, as a page, greater options wrt 'if they did this then the next frame behavor changes'
		foreach (var data in dataSequence)
		{
			CheckSuccess(data);
		}

		//if (!hasFailed)
			Complete = true;
	}

	public string ResolveSound(string concept)
	{
		string[] sound;
		if (customSoundDict == null || !customSoundDict.TryGetValue(concept, out sound))
		{
			if (mainSoundDict != null)
			{
				mainSoundDict.TryGetValue(concept, out sound);
			}
			else
			{
				return "";
			}
		}
		if (sound == null)
			return "";
		return sound[Random.Range(0, sound.Length)];
	}

	public virtual void CheckSuccess(PageTrackerData thisCell)
	{
		int score = thisCell.score;

		thisCell.resolved = true;

		//if (thisCell.enemy != "")
			//Debug.Log("Check Succes: " + thisCell.enemy);

		if (thisCell.enemy == "chomp")
		{
			int modulo = thisCell.sequenceNumber % 4;
			if (modulo == 1 || modulo == 2)
			{
				thisCell.enemy = "";
			}
		}

		if (playerAnimMap != null)
		{
			score += playerAnimMap.AssessSuccess(thisCell);
			//Debug.Log("Cell " + thisCell.sequenceNumber + " final score: " + score);
			pageScore += score;
			//Debug.Log("Page score " + pageScore);
		}

		if (score < 0)
		{
			Failure();
		}

		if (thisCell.resolution == DATAMARKER.END)
		{
			hasFailed = false;
			if (!hasFailed)
				Complete = true;
		}
		
	}

	public int AssessScore()
	{
		Debug.Log("Score: " + pageScore + " vs max: " + maxScore);
		if (pageScore > maxScore)
		{
			return 2;
		}
		else if (pageScore == maxScore && maxScore != 0)
		{
			return 1;
		}
		else if (pageScore >= maxScore * 0.5f || maxScore == 0)
		{
			return 0;
		}
		else
		{
			return -1;
		}
	}

	public virtual int PhaseBeats()
	{
		return 4;
	}

	public virtual void ProcessConcept(PageTrackerData node, ref string move, int sequenceNumber )
	{

	}
}



public class GamePlayPage : Page
{
	// What might a page deal with?
	/*
	We want to handle the idae of a single sequence, or at some point, a single enemy.
	We may well need to have child classes for this purpose
	For now we're dealing with
	a) we will have an enemy sequence that we loaded from somewhere. In this case, our most basic test is goombas and pits
	b) We will take in 'concepts' about abilities that apply to the game we are a part of. These concepts are of an ucertain type. Maybe just strings?
	These strings will at some point move us through a state system.
	c) we MAY be asked if we are completed?
	d) We MAY need to tell some kind of game display that will exist at a higher level than us what is happening on each beat.


	Let's run through then.
	So maybe we load a page, we're like, ok, we're like, ok, Page Manager, put @ incoming first of bar, our pattern is this:
	goomba, blank, goomba, blank
	Is like sure ok: Resource manager, I need a goomba prefab -> Game Display, and I need a goomba in a bar prefab -> Tracker Bar
	Beat continues!
	Gamestate is like: hey, PageManager, this was my input.
	Page Manager converts input to concepts, stores in data.
	End of bar hits, bar is given to page, page assesses success, since we need to arrange for the future at this point
	Page Manager continues to beat @ page which can do the game displayed input when needed

	*/
	public GamePlayPage(LevelPageData buildInfo, bool noBreaks = false, string[] autoActions = null)
	{
		enemyAttacks = buildInfo.enemyAttacks;
		playerInputConceptDict = buildInfo.playerInputConceptDict;
		playerAnimMap = buildInfo.animMap;
		mainSoundDict = buildInfo.mainSoundDict;
		maxScore = buildInfo.maxScore;

		Reset();

		if (autoActions != null)
		{
			this.autoActions = new List<string>(autoActions);
		}


		DisplaySuccess = true;

		NoBreaks = noBreaks;
	}

	public override void ProcessConcept(PageTrackerData node, ref string move, int sequenceNumber )
	{
		if (move == "flame")
		{
			if (node.enemy == "goomba" || node.enemy == "pit" || (node.enemy == "chomp" && (node.sequenceNumber % 4 == 0 || node.sequenceNumber % 4 == 3)))
			{
				move = "flamefail";
			}
		}

		//Debug.Log("Incoming Move: " + move + " on " + node);

		if (move == "kill")
		{
			if (node.enemy == "goomba")
			{
				node.enemy = "";
				node.score += 2;
			}
			else if (node.enemy == "chomp")
			{
				int smod = node.sequenceNumber % 4;
				//if (smod == 1 || smod == 2)
				node.enemy = "";

				node.score += 2;
			}
			else if (node.enemy == "coingoomba")
			{
				node.enemy = "coinhi";
				node.score += 2;
			}
			move = "";
		}

		if (move != "" && node.auto == "")
			node.player = move;

		//Debug.Log("Result: " + node);
	}
}

public class CutscenePage : Page, IObserver<BeatData>
{
	private int beatsSoFar = 0;
	private int length = 0;
	private int beatsTillEnd = -1;

	private string inputString;
	private string customAudio;

	private IDisposable _unsubscriber = null;
	private IDisposable Unsubscriber
	{
		get
		{
			return _unsubscriber;
		}
		set
		{
			if (_unsubscriber != null)
			{
				_unsubscriber.Dispose();
			}

			_unsubscriber = value;
		}
	}

	public CutscenePage(int length, string testString, Dictionary<BUTTON, string> pageActiveInputDict = null, string customAudio = "", string special = "")
	{
		enemyAttacks = new List<string>();

		NoBreaks = false;

		length /= 2;

		this.length = length;
		this.customAudio = customAudio;
		inputString = testString;

		for (int j = 0; j < length; j++)
		{
			enemyAttacks.Add("cutscene");
		}

		if (pageActiveInputDict != null)
		{
			this.pageActiveInputDict = pageActiveInputDict;
		}

		if (special != "")
		{
			enemyAttacks[0] = "cutscene" + special;
		}
	}

	public override void Reset()
	{
		//
	}

	public override void UpNext()
	{

	}

	public void ActivateBeat(IObservable<BeatData> dataSource)
	{
		beatsTillEnd = length * 2;
		Unsubscriber = dataSource.Subscribe(this);
		Cutscener.SetText(inputString);
	}

	public string GetCustomAudio()
	{
		return customAudio;
	}

	public void ResolveBeat()
	{
		
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
		if (Unsubscriber == null)
			return;

		beatsTillEnd--;
		//Debug.Log(beatsTillEnd);
		if (beatsTillEnd == 0)
		{
			base.Reset();
			Cutscener.Hide();
			beatsTillEnd = -1;
			Unsubscriber = null;
		}
	}
}