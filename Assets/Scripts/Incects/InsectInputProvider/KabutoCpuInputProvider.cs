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

    private InsectControllerBase opponent; // ← 参照で持つ

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

        // 再武装（attack）
        if (!attackable && Time.time >= rearmAttackTime)
        {
            attackable = true;
            attackFlag = 2;
            ScheduleNextAttack();
        }

        // 再武装（dodge）
        if (!dodgeable && Time.time >= rearmDodgeTime)
        {
            dodgeable = true;
            dodgeFlag = 2;
        }

        // 相手の攻撃を見て回避予約（Opponent参照で判定）
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
                // 相手が攻撃じゃなくなったら予約解除（任意）
                if (scheduledDodgeTime > 0f && Time.time + 0.2f < scheduledDodgeTime)
                    scheduledDodgeTime = -1f;
            }
        }

        // 回避発動
        if (dodgeable && scheduledDodgeTime > 0f && Time.time >= scheduledDodgeTime)
        {
            dodgeFlag = 1;
            dodgeable = false;
            rearmDodgeTime = Time.time + rearmDelayDodge;
            scheduledDodgeTime = -1f;
        }

        // 攻撃発動
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
