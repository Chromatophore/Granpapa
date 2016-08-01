using System.Collections.Generic;

public interface IGameDisplay
{
	void SetUpcomingAttack(int distanceAhead, string attack, bool firstOfBar);
	void SetUpcomingAttacks(int distanceAhead, List<string> attacks);
	//void PassPlayerAnimation(string animation);
	void Beat(float beatTime, PageTrackerData data);
	void SetBeatsPerPhase(int beats);
}