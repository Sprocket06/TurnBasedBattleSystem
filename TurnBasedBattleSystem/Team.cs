namespace TurnBasedBattleSystem;

public class Team
{
	public List<IUnit> Members { get; set; } = [];
	public bool HasAi { get; set; }
}