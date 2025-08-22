namespace TurnBasedBattleSystem;

public interface IUnit
{
	public int Health { get; set; }
	public int Speed { get; set; }

	public List<IStatus> Statuses { get; set; }
}