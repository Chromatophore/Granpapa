using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class EnemyResolution
{
	public List<string> pass;

	public EnemyResolution(List<string> passes)
	{
		pass = passes;
	}

	public static EnemyResolution Quick(string param)
	{
		return new EnemyResolution(new List<string>(param.Split(',')));
	}
}

public class Page : IObserver<BeatData>
{
	private bool upNext;
	private bool isPlaying;
	private bool hasFailed;
	private int beatCount;

	private List<string> enemyAttacks = new List<string>(new string[] {"goomba", "", "pit", "", "goomba", ""});

	private Dictionary<BUTTON, string[]> playerInputConceptDict = new Dictionary<BUTTON, string[]>() { 
						{ BUTTON.A, new string[] { "jump" } }, 
						{ BUTTON.B, new string[] { "jump" } }, 
						{ BUTTON.X, new string[] { "jump" } }, 
						{ BUTTON.Y, new string[] { "jump", "jumpend" } }
	};

	private Dictionary<string, EnemyResolution> resolutionDict = new Dictionary<string, EnemyResolution>() {
		{"goomba", EnemyResolution.Quick("jump") },
		{"pit", EnemyResolution.Quick("jump") }
	};

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
	public Page(LevelPageData buildInfo)
	{
		enemyAttacks = buildInfo.enemyAttacks;
		playerInputConceptDict = buildInfo.playerInputConceptDict;
		resolutionDict = buildInfo.resolutionDict;
		Reset();
	}

	private void Reset()
	{
		upNext = false;
		isPlaying = false;
		hasFailed = false;
		beatCount = 0;
	}

	public List<string> GetAttacks()
	{
		return enemyAttacks;
	}

	public void UpNext()
	{
		upNext = true;
	}

	public Dictionary<BUTTON, string[]> GetPlayerInputConceptDict()
	{
		return playerInputConceptDict;
	}

	public Dictionary<string, EnemyResolution> GetResolutionDict()
	{
		return resolutionDict;
	}

	public void CheckSuccess(PageTrackerData thisCell)
	{
		var enemy = thisCell.enemy;
		var player = thisCell.player;
		EnemyResolution resolutions;
		if (resolutionDict.TryGetValue(enemy, out resolutions))
		{
			if (resolutions.pass.Contains(player) && !hasFailed)
			{
				thisCell.success = true;
			}
			else
			{
				thisCell.success = false;
				Failure();
			}
		}
	}

	private void Failure()
	{
		hasFailed = true;
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
		// Urgh this is another off by one; Do we need to do the Success Checking earlier in PageSequence?
		if (data.beatInBar == 0)
		{
			if (!hasFailed && isPlaying)
			{
				// If we unsubscribe during this process, we modify the hashset while it is being iterated over. Not good.
				//unsubscriber.Dispose();

				pageSequence.nextPage();
				Reset();
			}
			else if (isPlaying)
			{
				Reset();
			}
			else if (upNext)
			{
				isPlaying = true;
			}
		}
	}

	private PageSequence pageSequence;
	private IDisposable unsubscriber;
	public void Setup(IObservable<BeatData> beatObserver, PageSequence ps)
	{
		pageSequence = ps;
		unsubscriber = beatObserver.Subscribe(this);
	}
}