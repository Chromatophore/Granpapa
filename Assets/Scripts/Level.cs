using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class LevelPageData
{
	public SongStruct levelAudio;
	public List<string> enemyAttacks;
	public Dictionary<BUTTON, string[]> playerInputConceptDict;
	public PlayerAnimMap animMap;


	public LevelPageData(List<string> enemyAttacks, Dictionary<BUTTON, string[]> playerInputConceptDict, SongStruct levelAudio, PlayerAnimMap animMap)
	{
		this.enemyAttacks = enemyAttacks;
		this.playerInputConceptDict = playerInputConceptDict;
		this.levelAudio = levelAudio;
		this.animMap = animMap;
	}
}


public class Level
{
	public string gameName = "Mario";

	public SongStruct levelAudio;

	private NoodleMain noodleMain;

	private List<string> enemyAttacks = new List<string>(new string[] { "goomba", "", "pit", "" }); // , "goomba", "goomba", "", ""});
	private List<string> enemyAttacks2 = new List<string>(new string[] { "pit", "", "goomba", "goomba" }); //, "", "pit", "", ""});

	private Dictionary<BUTTON, string[]> playerInputConceptDict = new Dictionary<BUTTON, string[]>() { 
						{ BUTTON.A, new string[] { "jump" } }, 
						{ BUTTON.B, new string[] { "hop" } }, 
						{ BUTTON.X, new string[] { "jump" } }, 
						{ BUTTON.Y, new string[] { "jump", "jumpend" } }
	};

	private List<Page> myPages = new List<Page>();

	private PlayerAnimMap animMap;

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


	public Level()
	{
		if (noodleMain == null)
		{
			noodleMain = NoodleMain.GetSingleRef();
		}

		levelAudio.audioTrack = noodleMain.GetClip("mariotest");
		levelAudio.beatTime = 0.666666f;
		levelAudio.beatsPerBar = 4;

		animMap = new PlayerAnimMap( new string[] {
		"", "jump", "Jump", "", "0",
		"", "hop", "Hop", "", "0",
		"pit", "jump", "Jump", "", "1",
		"pit", "def", "PitNo", "", "-1",
		"goomba", "jump", "Jump", "", "1",
		"goomba", "hop", "EnemyYes", "splat", "2",
		"goomba", "def", "EnemyNo", "", "-1"
			});

		List<LevelPageData> pageBuilder = new List<LevelPageData>();
		int totalPages = 6;
		for (int i = 0; i < totalPages; i++)
		{
			var eA = enemyAttacks;
			if (i % 2 == 1)
			{
				eA = enemyAttacks2;
			}
			pageBuilder.Add(new LevelPageData(eA, playerInputConceptDict, levelAudio, animMap));
		}

		myPages.Add(new CutscenePage(8, "*No Speaky!*"));
		myPages.Add(new CutscenePage(8, "One thing first, before we start!\nPick up, Flip up and blow in the cart!"));
		myPages.Add(new CutscenePage(8, "Let's try this first, see how it goes\nIt's time to play super mario bros!"));
		myPages.Add(qpg (new string[] { "", "", "goomba", "" } ));
		myPages.Add(qpg (new string[] { "", "pit", "", "goomba" } ));
		myPages.Add(qpg (new string[] { "goomba", "", "pit", "pit" } ));
		myPages.Add(qpg (new string[] { "pit", "goomba", "pit", "" } ));
		myPages.Add(new CutscenePage(8, "one more skill, that's good to use\nYou can squish goombas to death with your shoes"));
		myPages.Add(qpg (new string[] { "", "", "goomba", "" } ));
		myPages.Add(qpg (new string[] { "", "pit", "", "goomba" } ));
		myPages.Add(qpg (new string[] { "goomba", "", "pit", "pit" } ));
		myPages.Add(qpg (new string[] { "pit", "goomba", "pit", "" } ));
		myPages.Add(new CutscenePage(8, "8 bar interlude"));
		myPages.Add(qpg (new string[] { "", "goomba", "pit", "", "", "goomba", "pit", ""} ));
		myPages.Add(qpg (new string[] { "", "goomba", "pit", "", "", "goomba", "pit", ""} ));
		myPages.Add(qpg (new string[] { "", "goomba", "pit", "", "", "goomba", "pit", ""} ));
		myPages.Add(qpg (new string[] { "", "goomba", "pit", "", "", "goomba", "pit", ""} ));
		myPages.Add(new CutscenePage(8, "8 bar interlude"));
		myPages.Add(qpg (new string[] { "", "goomba", "pit", "", "", "goomba", "pit", ""} ));
		myPages.Add(qpg (new string[] { "", "goomba", "pit", "", "", "goomba", "pit", ""} ));
		myPages.Add(qpg (new string[] { "", "goomba", "pit", "", "", "goomba", "pit", ""} ));
		myPages.Add(qpg (new string[] { "", "goomba", "pit", "", "", "goomba", "pit", ""} ));
		myPages.Add(new CutscenePage(16, "Well done, you can go on to the next stage now..."));
		myPages.Add(new CutscenePage(4, "*fill*"));
	}

	private GamePlayPage qpg(string[] input)
	{
		List<string> lst = new List<string>(input);
		return new GamePlayPage(new LevelPageData(lst, playerInputConceptDict, levelAudio, animMap));
	}

	public List<Page> getPages()
	{
		return myPages;
	}
}