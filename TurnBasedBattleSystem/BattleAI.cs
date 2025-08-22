using TurnBasedBattleSystem.Actions;

namespace TurnBasedBattleSystem;

public interface IBattleAI
{
	public IBattleAction DoAction(IUnit actor);
}