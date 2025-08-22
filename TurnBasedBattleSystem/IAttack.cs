using TurnBasedBattleSystem.Events;

namespace TurnBasedBattleSystem;

public interface IAttack
{
	public IEnumerable<BattleEvent> Resolve(IUnit attacker, IUnit target);
}