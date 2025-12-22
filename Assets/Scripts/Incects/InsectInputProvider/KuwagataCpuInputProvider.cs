using UnityEngine;

public class KuwagataCpuInputProvider : MonoBehaviour, IInsectInputProvider
{
    [Header("CPU Timing")]
    [SerializeField] private float attackIntervalMin = 0.8f;
    [SerializeField] private float attackIntervalMax = 1.6f;
    [SerializeField, Range(0f, 1f)] private float attackChance = 0.85f;

    [Header("Rearm")]
    [SerializeField] private float rearmDelay = 0.25f;

    private float nextAttackTime = 0f;
    private float rearmTime = 0f;

    private bool attackable = true;

    private void Start()
    {
        ScheduleNextAttack();
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
            if (Time.time >= rearmTime)
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
                rearmTime = Time.time + rearmDelay;
                return new InsectInput { AttackFlag = 1, DodgeFlag = 0 }; // UŒ‚
            }
        }

        return default;
    }
}
