namespace TurnBasedBattleSystem.Actions;

public class AttackAction(IUnit attacker, IUnit target, IAttack attack) : IBattleAction
{
	public IUnit Attacker { get; set; } = attacker;
	public IUnit Target { get; set; } = target;
	public IAttack Attack { get; set; } = attack;
	public int Priority => Attacker.Speed;
}