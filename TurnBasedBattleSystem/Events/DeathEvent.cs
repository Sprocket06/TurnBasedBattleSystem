namespace TurnBasedBattleSystem.Events;

public class DeathEvent(IUnit deadUnit) : BattleEvent
{
	public IUnit Unit { get; set; } = deadUnit;
}