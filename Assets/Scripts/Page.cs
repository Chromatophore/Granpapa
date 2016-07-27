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

	private bool upNext;
	private bool isPlaying;
	private bool hasFailed;
	private int beatCount;
	protected PlayerAnimMap playerAnimMap;

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
	}

	public virtual void Reset()
	{
		upNext = false;
		isPlaying = false;
		hasFailed = false;
		beatCount = 0;

		Complete = false;
	}

	public List<string> GetAttacks()
	{
		return enemyAttacks;
	}

	public Dictionary<BUTTON, string[]> GetPlayerInputConceptDict()
	{
		return playerInputConceptDict;
	}

	public void AssessSequence(ICollection<PageTrackerData> dataSequence)
	{
		// This method receives the whole sequence of information from start to end
		// This gives us, as a page, greater options wrt 'if they did this then the next frame behavor changes'
		foreach (var data in dataSequence)
		{
			CheckSuccess(data);
		}

		if (!hasFailed)
			Complete = true;
	}

	public virtual void CheckSuccess(PageTrackerData thisCell)
	{
		int score = 0;

		if (playerAnimMap != null)
			score = playerAnimMap.AssessSuccess(thisCell);

		if (score < 0)
		{
			Failure();
		}

		if (thisCell.resolution == DATAMARKER.END)
			Complete = true;
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
	public GamePlayPage(LevelPageData buildInfo)
	{
		enemyAttacks = buildInfo.enemyAttacks;
		playerInputConceptDict = buildInfo.playerInputConceptDict;
		playerAnimMap = buildInfo.animMap;

		Reset();

		DisplaySuccess = true;
	}
}

public class CutscenePage : Page, IObserver<BeatData>
{
	private int beatsSoFar = 0;
	private int length = 0;

	private string inputString;

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

	public CutscenePage(int length, string testString)
	{
		enemyAttacks = new List<string>();

		length /= 2;

		this.length = length;
		inputString = testString;

		for (int j = 0; j < length; j++)
		{
			enemyAttacks.Add("cutscene");
		}
	}

	public override void Reset()
	{
		//
	}

	public override void UpNext()
	{

	}

	public void ActiveBeat(IObservable<BeatData> dataSource)
	{
		if (beatsSoFar == 0)
		{
			Unsubscriber = dataSource.Subscribe(this);
			Cutscener.SetText(inputString);
		}
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
		beatsSoFar++;

		if (beatsSoFar == length * 2)
		{
			base.Reset();
			Cutscener.Hide();
			beatsSoFar = 0;
			Unsubscriber = null;
		}
	}
}