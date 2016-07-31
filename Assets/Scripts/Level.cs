using UnityEngine;
using System.Collections.Generic;	// We use generic for Data Structures with <YourClass> style declarations

public class LevelPageData
{
	public SongStruct levelAudio;
	public List<string> enemyAttacks;
	public Dictionary<BUTTON, string[]> playerInputConceptDict;
	public PlayerAnimMap animMap;
	public Dictionary<string, string[]> mainSoundDict;
	public int maxScore;


	public LevelPageData(List<string> enemyAttacks, Dictionary<BUTTON, string[]> playerInputConceptDict, SongStruct levelAudio, PlayerAnimMap animMap, Dictionary<string, string[]> mainSoundDict, int maxScore)
	{
		this.enemyAttacks = enemyAttacks;
		this.playerInputConceptDict = playerInputConceptDict;
		this.levelAudio = levelAudio;
		this.animMap = animMap;
		this.mainSoundDict = mainSoundDict;
		this.maxScore = maxScore;
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
						{ BUTTON.X, new string[] { "flame", "kill" } }, 
						{ BUTTON.Y, new string[] { "jump", "jumpend" } }
	};

	private Dictionary<BUTTON, string> firstActiveInputDict = new Dictionary<BUTTON, string>() { 
						{ BUTTON.A, "Jump" }
	};

	private Dictionary<BUTTON, string> secondActiveInputDict = new Dictionary<BUTTON, string>() { 
						{ BUTTON.A, "Jump" }, 
						{ BUTTON.B, "Hop" }
	};

	private Dictionary<BUTTON, string> thirdActiveInputDict = new Dictionary<BUTTON, string>() { 
						{ BUTTON.A, "Jump" }, 
						{ BUTTON.B, "Hop" }, 
						{ BUTTON.X, "Flame" }
	};


	private Dictionary<string, string[]> mainSoundDict = new Dictionary<string, string[]>() { 
						{ "jump", new string[] { "jump" } }, 
						{ "hop", new string[] { "bop" } }
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
		"", "flame", "Flame", "", "0",
		"pit", "jump", "Jump", "", "1",
		"pit", "def", "PitNo", "", "-1",
		"goomba", "jump", "Jump", "", "1",
		"goomba", "hop", "EnemyYes", "splat", "2",
		"goomba", "def", "EnemyNo", "", "-1",
		"chomp", "jump", "Jump", "", "1",
		"chomp", "def", "EnemyNo", "", "-1",
		"coinhi", "jump", "Jump", "collect", "1",
		"coinhi", "hop", "Hop", "", "0",
		"coinhi", "flame", "Flame", "", "0",
		"coinlo", "jump", "Jump", "collect", "1",
		"coinlo", "hop", "Hop", "collect", "1",
		"coinlo", "flame", "Flame", "", "0",
		"coingoomba", "jump", "Jump", "collect", "1",
		"coingoomba", "hop", "EnemyYes", "splat", "2",
		"coingoomba", "def", "EnemyNo", "", "-1",
		
			});

		//myPages.Add(new CutscenePage(8, "*No Speaky!*"));
		//myPages.Add(new CutscenePage(8, "One thing first, before we start!\nPick up, Flip up and blow in the cart!"));
		myPages.Add(new CutscenePage(8, "Let's try this first, see how it goes\nIt's time to play super mario bros!", thirdActiveInputDict));
		//myPages.Add(new CutscenePage(8, "Let's try this first, see how it goes\nIt's time to play super mario bros!", firstActiveInputDict));
		myPages.Add(qpg (new string[] { "", "", "", "", "", "", "", ""}, 1));
		Dictionary<string, string[]> customSoundDict = new Dictionary<string, string[]>();
		customSoundDict.Add("jump", new string[] {"bop"});
		myPages[myPages.Count - 1].AddCustomSounds(customSoundDict);
		myPages.Add(qpg (new string[] { "coinhi", "pit", "coinlo", "goomba" }, 2));
		myPages.Add(qpg (new string[] { "goomba", "", "pit", "pit" }, 3));
		myPages.Add(qpg (new string[] { "pit", "goomba", "pit", "" }, 3));
		myPages.Add(new CutscenePage(8, "one more skill, that's good to use\nYou can squish goombas to death with your shoes", thirdActiveInputDict));
		//myPages.Add(new CutscenePage(8, "one more skill, that's good to use\nYou can squish goombas to death with your shoes", secondActiveInputDict));
		myPages.Add(qpg (new string[] { "", "", "goomba", "" }, 1));
		myPages.Add(qpg (new string[] { "", "pit", "", "goomba" }, 2));
		myPages.Add(qpg (new string[] { "goomba", "", "pit", "pit" }, 3));
		myPages.Add(qpg (new string[] { "pit", "goomba", "pit", "" }, 3));
		myPages.Add(new CutscenePage(8, "8 bar interlude", thirdActiveInputDict));
		myPages.Add(qpg(new string[] { "", "goomba", "pit", "", "", "goomba", "pit", "" }, 4));
		myPages.Add(qpg(new string[] { "", "goomba", "pit", "", "", "goomba", "pit", "" }, 4));
		myPages.Add(qpg(new string[] { "", "goomba", "pit", "", "", "goomba", "pit", "" }, 4));
		myPages.Add(qpg(new string[] { "", "goomba", "pit", "", "", "goomba", "pit", "" }, 4));
		myPages.Add(new CutscenePage(8, "8 bar interlude"));
		myPages.Add(qpg(new string[] { "", "goomba", "pit", "", "", "goomba", "pit", "" }, 4));
		myPages.Add(qpg(new string[] { "", "goomba", "pit", "", "", "goomba", "pit", "" }, 4));
		myPages.Add(qpg(new string[] { "", "goomba", "pit", "", "", "goomba", "pit", "" }, 4));
		myPages.Add(qpg(new string[] { "", "goomba", "pit", "", "", "goomba", "pit", "" }, 4));
		myPages.Add(new CutscenePage(16, "Well done, you can go on to the next stage now..."));
		myPages.Add(new CutscenePage(4, "*fill*"));
	}

	private GamePlayPage qpg(string[] input, int maxscore = 0)
	{
		List<string> lst = new List<string>(input);
		return new GamePlayPage(new LevelPageData(lst, playerInputConceptDict, levelAudio, animMap, mainSoundDict, maxscore));
	}

	public List<Page> getPages()
	{
		return myPages;
	}
}