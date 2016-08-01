
// Simple struct that is delivered to things that wish to know about beats as they occur:
public struct BeatData
{
	public BeatData(int beatNumber, int beatInBar, float beatTime, bool activeResolutionBeat)
	{
		this.beatNumber = beatNumber;
		this.beatInBar = beatInBar;
		this.isFirstBeatOfBar = (beatInBar == 0);
		this.beatTime = beatTime;
		this.resolutionBeat = activeResolutionBeat;
	}
	public int beatNumber;
	public int beatInBar;
	public bool isFirstBeatOfBar;
	public float beatTime;
	public bool resolutionBeat;

	public override string ToString()
	{
		return string.Format("{0} {1} {2} {3} {4}", beatNumber, beatInBar, isFirstBeatOfBar, beatTime, resolutionBeat);
	}
}