
// Simple struct that is delivered to things that wish to know about beats as they occur:
public struct BeatData
{
	public BeatData(int beatNumber, int beatInBar)
	{
		this.beatNumber = beatNumber;
		this.beatInBar = beatInBar;
		this.isFirstBeatOfBar = (beatInBar == 0);
	}
	public int beatNumber;
	public int beatInBar;
	public bool isFirstBeatOfBar;
}