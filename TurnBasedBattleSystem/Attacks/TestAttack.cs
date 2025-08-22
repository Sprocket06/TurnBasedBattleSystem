using TurnBasedBattleSystem.Events;

namespace TurnBasedBattleSystem.Attacks;

public class TestAttack : IAttack
{
	public int Damage { get; set; } = 5;

	public IEnumerable<BattleEvent> Resolve(IUnit attacker, IUnit target)
	{
		attacker.Health -= Damage;

		HitEvent hit = new(attacker, target, this, Damage);
		yield return hit;
	}
}