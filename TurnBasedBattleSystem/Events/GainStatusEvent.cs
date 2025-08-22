namespace TurnBasedBattleSystem.Events;

public class GainStatusEvent(IUnit unit, IStatus status, int stacks) : BattleEvent
{
	public IUnit Unit { get; set; } = unit;
	public IStatus Status { get; set; } = status;
	public int Stacks { get; set; } = stacks;
}