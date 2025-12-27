using UnityEngine;

public class KuwagataCpuInputProvider : MonoBehaviour, IInsectInputProvider
{
    [Header("Attack")]
    [SerializeField] private float attackIntervalMin = 0.8f;
    [SerializeField] private float attackIntervalMax = 1.6f;
    [SerializeField, Range(0f, 1f)] private float attackChance = 0.85f;

    [Header("Dodge")]
    [SerializeField] private float dodgeReactionMin = 0.08f;
    [SerializeField] private float dodgeReactionMax = 0.18f;
    [SerializeField, Range(0f, 1f)] private float dodgeChance = 0.85f;

    [Header("Rearm")]
    [SerializeField] private float rearmDelayAttack = 0.25f;
    [SerializeField] private float rearmDelayDodge = 0.10f;

    private float nextAttackTime = 0f;
    private float rearmAttackTime = 0f;
    private float rearmDodgeTime;

    private bool attackable = true;
    private bool dodgeable = true;

    private float scheduledDodgeTime = -1f;

    private InsectControllerBase opponent;

    private void Start()
    {
        ScheduleNextAttack();
    }

    public void SetOpponent(InsectControllerBase opp)
    {
        opponent = opp;
    }

    private void ScheduleNextAttack()
    {
        nextAttackTime = Time.time + Random.Range(attackIntervalMin, attackIntervalMax);
    }

    public InsectInput GetInput()
    {
        // Ä•‘•‘Ò‚¿’†
        if (!attackable)
        {
            if (Time.time >= rearmAttackTime)
            {
                attackable = true;
                ScheduleNextAttack();
                return new InsectInput { AttackFlag = 2, DodgeFlag = 0 }; // Ä•‘•
            }
            return default;
        }

        // UŒ‚ƒ^ƒCƒ~ƒ“ƒO
        if (Time.time >= nextAttackTime)
        {
            ScheduleNextAttack();

            if (Random.value <= attackChance)
            {
                attackable = false;
                rearmAttackTime = Time.time + rearmDelayAttack;
                return new InsectInput { AttackFlag = 1, DodgeFlag = 0 }; // UŒ‚
            }
        }

        return default;
    }
}
