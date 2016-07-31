

public class PageTrackerData
{
	public string enemy;
	public string player;
	public bool success;
	public bool active;
	public DATAMARKER resolution;

	public string playerAnimation;
	public string enemyAnimation;
	public int score;
	public int sequenceNumber = 0;

	public Page originPage;

	public PageTrackerData dataStartPoint;

	public int offsetToEndOfSequence = -1;
	public int phaseLength = -1;

	public bool resolved = false;

	public PageTrackerData next = null;

	public PageTrackerData()
	{
		Reset();
	}

	public void Reset()
	{
		enemy = "";
		player = "";
		success = false;
		active = false;
		resolution = DATAMARKER.NONE;
		dataStartPoint = null;

		score = 0;
		playerAnimation = "";
		enemyAnimation = "";

		originPage = null;
		offsetToEndOfSequence = -1;
		phaseLength = -1;
		resolved = false;
		sequenceNumber = 0;
	}

	public override string ToString()
	{
		return string.Format("'{0}' v '{1}': {2} {3} {4}", enemy, player, resolution, offsetToEndOfSequence, phaseLength);
	}
}