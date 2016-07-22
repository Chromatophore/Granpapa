using System.Collections.Generic;

public interface IGameDisplay
{
	void SetUpcomingAttack(int distanceAhead, string attack);
	void SetUpcomingAttacks(int distanceAhead, List<string> attacks);
	void PassPlayerAnimation(string animation);
}