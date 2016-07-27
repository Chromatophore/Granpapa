

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

	public Page originPage;

	public PageTrackerData dataStartPoint;

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
	}

	public override string ToString()
	{
		return string.Format("'{0}' v '{1}': {2}", enemy, player, playerAnimation);
	}
}