using UnityEngine;

public class CpuInputProvider : MonoBehaviour, IInsectInputProvider, IOpponentAware
{
    [Header("Attack")]
    [SerializeField] private float attackIntervalMin = 0.9f;
    [SerializeField] private float attackIntervalMax = 1.8f;
    [SerializeField, Range(0f, 1f)] private float attackChance = 0.8f;
    [SerializeField] private float rearmDelayAttack = 0.15f;

    [Header("Dodge")]
    [SerializeField] private bool enableDodge = true;
    [SerializeField] private float dodgeReactionMin = 0.08f;
    [SerializeField] private float dodgeReactionMax = 0.18f;
    [SerializeField, Range(0f, 1f)] private float dodgeChance = 0.85f;
    [SerializeField] private float rearmDelayDodge = 0.10f;

    [Header("Priority")]
    [SerializeField] private bool preferDodgeOverAttack = true;

    private bool isEnabled = true;
    public void SetEnabled(bool enabled) => isEnabled = enabled;

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
        if (!isEnabled) return default;

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
        if (enableDodge && !dodgeable && Time.time >= rearmDodgeTime)
        {
            dodgeable = true;
            dodgeFlag = 2;
        }

        // 回避予約（相手の攻撃モーションを見て反応）
        if (enableDodge && opponent != null && dodgeable)
        {
            if (opponent.AnimState == InsectAnimState.Attack)
            {
                if (scheduledDodgeTime < 0f && Random.value <= dodgeChance)
                    scheduledDodgeTime = Time.time + Random.Range(dodgeReactionMin, dodgeReactionMax);
            }
            else
            {
                // 予約が遠すぎるならキャンセル（任意）
                if (scheduledDodgeTime > 0f && Time.time + 0.2f < scheduledDodgeTime)
                    scheduledDodgeTime = -1f;
            }
        }

        // 回避発動
        if (enableDodge && dodgeable && scheduledDodgeTime > 0f && Time.time >= scheduledDodgeTime)
        {
            dodgeFlag = 1;
            dodgeable = false;
            rearmDodgeTime = Time.time + rearmDelayDodge;
            scheduledDodgeTime = -1f;
        }

        // 攻撃発動（回避を優先したいなら、同フレーム攻撃を抑止）
        bool canAttackThisFrame = attackable && Time.time >= nextAttackTime;
        if (preferDodgeOverAttack && dodgeFlag == 1) canAttackThisFrame = false;

        if (canAttackThisFrame)
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
