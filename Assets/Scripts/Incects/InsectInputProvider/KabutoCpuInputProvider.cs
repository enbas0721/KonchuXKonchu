using UnityEngine;

public class KabutoCpuInputProvider : MonoBehaviour, IInsectInputProvider, IOpponentAware
{
    [Header("Attack")]
    [SerializeField] private float attackIntervalMin = 0.9f;
    [SerializeField] private float attackIntervalMax = 1.8f;
    [SerializeField, Range(0f, 1f)] private float attackChance = 0.8f;

    [Header("Dodge")]
    [SerializeField] private float dodgeReactionMin = 0.08f;
    [SerializeField] private float dodgeReactionMax = 0.18f;
    [SerializeField, Range(0f, 1f)] private float dodgeChance = 0.85f;

    [Header("Rearm")]
    [SerializeField] private float rearmDelayAttack = 0.15f;
    [SerializeField] private float rearmDelayDodge = 0.10f;

    private float nextAttackTime;
    private float rearmAttackTime;
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
        int attackFlag = 0;
        int dodgeFlag = 0;

        if (!attackable && Time.time >= rearmAttackTime)
        {
            attackable = true;
            attackFlag = 2;
            ScheduleNextAttack();
        }

        if (!dodgeable && Time.time >= rearmDodgeTime)
        {
            dodgeable = true;
            dodgeFlag = 2;
        }

        // ‘Šè‚ÌUŒ‚‚ğŒ©‚Ä‰ñ”ğ—\–ñiOpponentQÆ‚Å”»’èj
        if (opponent != null && dodgeable)
        {
            if (opponent.AnimState == InsectAnimState.Attack)
            {
                if (scheduledDodgeTime < 0f && Random.value <= dodgeChance)
                {
                    scheduledDodgeTime = Time.time + Random.Range(dodgeReactionMin, dodgeReactionMax);
                }
            }
            else
            {
                // ‘Šè‚ªUŒ‚‚¶‚á‚È‚­‚È‚Á‚½‚ç—\–ñ‰ğœi”CˆÓj
                if (scheduledDodgeTime > 0f && Time.time + 0.2f < scheduledDodgeTime)
                    scheduledDodgeTime = -1f;
            }
        }

        // ‰ñ”ğ”­“®
        if (dodgeable && scheduledDodgeTime > 0f && Time.time >= scheduledDodgeTime)
        {
            dodgeFlag = 1;
            dodgeable = false;
            rearmDodgeTime = Time.time + rearmDelayDodge;
            scheduledDodgeTime = -1f;
        }

        // UŒ‚”­“®
        if (attackable && Time.time >= nextAttackTime)
        {
            ScheduleNextAttack();
            if (Random.value <= attackChance)
            {
                attackFlag = 1;
                attackable = false;
                rearmAttackTime = Time.time + rearmDelayAttack;
            }
        }

        return new InsectInput { AttackFlag = attackFlag, DodgeFlag = dodgeFlag };
    }
}
