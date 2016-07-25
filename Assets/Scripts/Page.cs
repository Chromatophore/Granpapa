using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class Page
{
	private bool upNext;
	private bool isPlaying;
	private bool hasFailed;
	private int beatCount;

	private PlayerAnimMap playerAnimMap;

	public bool useDataMarkers = false;

	private List<string> enemyAttacks;
	private Dictionary<BUTTON, string[]> playerInputConceptDict;

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
		playerAnimMap = buildInfo.animMap;

		Reset();
	}

	// External objects can call Complete to see if the page feels that it is time to move on
	public bool Complete { get; private set; }

	public void Reset()
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

	public void UpNext()
	{
		upNext = true;
		hasFailed = false;
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

	public void CheckSuccess(PageTrackerData thisCell)
	{
		int score = playerAnimMap.AssessSuccess(thisCell);

		if (score < 0)
		{
			Failure();
		}

		if (thisCell.resolution == DATAMARKER.END)
			Complete = true;
	}

	private void Failure()
	{
		hasFailed = true;
	}

	private PageSequence pageSequence;
	public void Setup(IObservable<BeatData> beatObserver, PageSequence ps)
	{
		pageSequence = ps;
	}
}