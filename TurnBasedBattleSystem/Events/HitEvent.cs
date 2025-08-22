namespace TurnBasedBattleSystem.Events;

public class HitEvent(IUnit attacker, IUnit target, IAttack attack, int damage) : BattleEvent
{
	public IUnit Attacker { get; set; } = attacker;
	public IUnit Target { get; set; } = target;
	public IAttack Attack { get; set; } = attack;
	public int Damage { get; set; } = damage;
}