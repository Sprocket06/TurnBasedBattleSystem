using TurnBasedBattleSystem.Actions;
using TurnBasedBattleSystem.Events;

namespace TurnBasedBattleSystem;

public static class BattleManager
{
	public static event OnDeathListener? OnDeath;

	public delegate void OnDeathListener(DeathEvent data);

	public static event OnGainStatusListener? OnGainStatus;

	public delegate void OnGainStatusListener(GainStatusEvent data);

	public static event OnHitListener? OnHit;

	public delegate void OnHitListener(HitEvent data);

	public static event OnTurnEndListener? OnTurnEnd;

	public delegate void OnTurnEndListener(EndTurnEvent data);

	public static event OnTurnStartListener? OnTurnStart;

	public delegate void OnTurnStartListener(StartTurnEvent data);

	public static readonly List<IBattleAction> CurrentEnemyActions = [];
	public static bool BattleInProgress;
	public static IBattleAI EnemyAi = new TestAI();
	public static List<IUnit> PlayerTeam { get; private set; } = [];
	public static List<IUnit> AiTeam { get; private set; } = [];

	public static void StartBattle(List<IUnit> playerUnits, List<IUnit> enemyUnits)
	{
		BattleInProgress = true;
		PlayerTeam = playerUnits;
		AiTeam = enemyUnits;

		OnTurnStart?.Invoke(new StartTurnEvent());

		foreach (var enemy in AiTeam)
			CurrentEnemyActions.Add(EnemyAi.DoAction(enemy));
	}

	public static void Cleanup()
	{
		BattleInProgress = false;
		OnTurnStart = null!;
		OnTurnEnd = null!;
		OnHit = null!;
		OnDeath = null!;
	}

	/*
	 * Start Battle
	 * Queue Enemy Actions
	 * Init battle/turn specific
	 * loop:wait for player input
	 * resolve player input
	 * resolve turn end
	 * resolve turn begin
	 * queue enemy actions
	 * jmp loop
	 *
	 * Event registry
	 */

	public static void HandlePlayerInput(List<IBattleAction> actions)
	{
		if (!BattleInProgress)
			throw new Exception();

		var combinedActions = CurrentEnemyActions.Concat(actions).ToList();

		//TODO: tiebreaking for priority
		combinedActions.Sort(ActionSort);

		foreach (var action in combinedActions)
			Resolve(action);

		OnTurnEnd?.Invoke(new EndTurnEvent());
		CurrentEnemyActions.Clear();
		OnTurnStart?.Invoke(new StartTurnEvent());
		foreach (var enemy in AiTeam)
			CurrentEnemyActions.Add(EnemyAi.DoAction(enemy));
	}

	private static int ActionSort(IBattleAction a, IBattleAction b) => a.Priority - b.Priority;

	private static void Resolve(IBattleAction action)
	{
		if (action is AttackAction atk)
			Resolve(atk.Attacker, atk.Target, atk.Attack);
	}

	private static void Resolve(IUnit attacker, IUnit target, IAttack attack)
	{
		/*
		 *
		 * So long story short
		 *
		 * each attack has a function resolve that returns an Enumerator<BattleEvent>
		 *
		 * loop through that, emit each event created.
		 */
		foreach (var e in attack.Resolve(attacker, target))
			switch (e)
			{
				// Err: Need defined order of operations for certain handlers.
				// do we tho?    
				case HitEvent h:
					OnHit?.Invoke(h);
					break;
				case DeathEvent d:
					OnDeath?.Invoke(d);
					break;
				case GainStatusEvent s:
					OnGainStatus?.Invoke(s);
					break;
			}

		//death check
		if (target.Health <= 0)
		{
			//push death event.
			DeathEvent ded = new(target);
			OnDeath?.Invoke(ded);
		}
	}
}