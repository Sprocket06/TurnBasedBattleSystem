using TurnBasedBattleSystem.Actions;
using TurnBasedBattleSystem.Events;

namespace TurnBasedBattleSystem;

using TurnBasedBattleSystem.Events;

public static class BattleManager
{
    public static List<Unit> PlayerTeam { get; private set; } = new();
    public static List<Unit> AITeam { get; private set; } = new();
    private static BattleAI EnemyAI = new TestAI();
    public static bool BattleInProgress = false;
    
    public static Queue<BattleEvent> EventQueue = new();
    public static List<BattleAction> CurrentEnemyActions = new();

    public delegate void OnTurnStartListener(StartTurnEvent data);

    public static event OnTurnStartListener OnTurnStart = null!;

    public delegate void OnHitListener(HitEvent data);

    public static event OnHitListener OnHit = null!;

    public delegate void OnDeathListener(DeathEvent data);

    public static event OnDeathListener OnDeath = null!;

    public delegate void OnTurnEndListener(EndTurnEvent data);

    public static event OnTurnEndListener OnTurnEnd = null!;
    
    public static void StartBattle(List<Unit> playerUnits, List<Unit> enemyUnits)
    {
        BattleInProgress = true;
        PlayerTeam = playerUnits;
        AITeam = enemyUnits;
        
        OnTurnStart?.Invoke(new StartTurnEvent());

        foreach (Unit enemy in AITeam)
        {
            CurrentEnemyActions.Add(EnemyAI.DoAction(enemy));
        }
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

    public static void HandlePlayerInput(List<BattleAction> actions)
    {
        if (!BattleInProgress) throw new Exception();
        List<BattleAction> combinedActions = CurrentEnemyActions.Concat(actions).ToList();
        //TODO: tiebreaking for priority
        combinedActions.Sort(ActionSort);

        foreach (BattleAction action in combinedActions)
        {
            Resolve(action);
        }
        
        OnTurnEnd?.Invoke(new EndTurnEvent());
    }

    private static int ActionSort(BattleAction a, BattleAction b)
    {
        return a.Priority - b.Priority;
    }

    static void Resolve(BattleAction action)
    {
        if (action is AttackAction atk)
        {
            Resolve(atk.Attacker, atk.Target, atk.Attack);
        }
    }
    
    static void Resolve(Unit attacker, Unit target, Attack attack)
    {
        /*
         *
         * So long story short
         *
         * each attack has a function resolve that returns an Enumerator<BattleEvent>
         *
         * loop through that, emit each event created.
         */

        foreach (BattleEvent e in attack.Resolve(attacker, target))
        {
            // Err: Need defined order of operations for certain handlers.
            // do we tho?    
            if (e is HitEvent h)
            {
                OnHit?.Invoke(h);
            }
            else if (e is DeathEvent d)
            {
                OnDeath?.Invoke(d);
            }
        }
        
        //death check
        if (target.Health <= 0)
        {
            //push death event.
            DeathEvent ded = new(target);
            EventQueue.Enqueue(ded);
        }
    }
}