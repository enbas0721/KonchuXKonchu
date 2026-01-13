using UnityEngine;

public class CpuInputProvider : MonoBehaviour, IInsectInputProvider, IOpponentAware
{
    [Header("Attack")]
    [SerializeField] private float attack_interval_min = 0.9f;
    [SerializeField] private float attack_interval_max = 1.8f;
    [SerializeField, Range(0f, 1f)] private float attack_chance = 0.8f;
    [SerializeField] private float attack_cool_down = 0.15f;

    [Header("Dodge")]
    [SerializeField] private bool enable_dodge = true;
    [SerializeField] private float dodge_reaction_min = 0.08f;
    [SerializeField] private float dodge_reaction_max = 0.18f;
    [SerializeField, Range(0f, 1f)] private float dodge_chance = 0.85f;
    [SerializeField] private float dodge_cool_down = 0.10f;

    [Header("Priority")]
    [SerializeField] private bool prefer_dodge_over_attack = true;

    private bool is_enabled = true;
    public void SetEnabled(bool enabled) => is_enabled = enabled;

    private float next_attack_time;
    private float next_attack_allowed_time;
    private float next_dodge_allowed_time;

    private float scheduled_dodge_time = -1f;
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
        next_attack_time = Time.time + Random.Range(attack_interval_min, attack_interval_max);
    }

    public InsectInput GetInput()
    {
        if (!is_enabled) return default;

        bool attack_event = false;
        bool dodge_event = false;

        // Dodge scheduling
        if (enable_dodge && opponent != null && Time.time >= next_dodge_allowed_time)
        {
            if (opponent.AnimState == InsectAnimState.Attack)
            {
                if (scheduled_dodge_time < 0f && Random.value <= dodge_chance)
                {
                    scheduled_dodge_time = Time.time + Random.Range(dodge_reaction_min, dodge_reaction_max);
                }
            }
            else
            {
                if (scheduled_dodge_time > 0f && Time.time + 0.2f < scheduled_dodge_time)
                {
                    scheduled_dodge_time = -1f;
                }
            }
        }

        // Dodge on
        if (enable_dodge && scheduled_dodge_time > 0f && Time.time >= scheduled_dodge_time)
        {
            if (Time.time >= next_dodge_allowed_time)
            {
                dodge_event = true;
                next_dodge_allowed_time = Time.time + dodge_cool_down;
            }
            scheduled_dodge_time = -1f;
        }

        // Attack on
        bool can_attack_this_frame =
            Time.time >= next_attack_time &&
            Time.time >= next_attack_allowed_time;

        if (prefer_dodge_over_attack && dodge_event) can_attack_this_frame = false;

        if (can_attack_this_frame)
        {
            ScheduleNextAttack();
            if (Random.value <= attack_chance)
            {
                attack_event = true;
                next_attack_allowed_time = Time.time + attack_cool_down;
            }
        }

        return new InsectInput { Attack = attack_event, Dodge = dodge_event };
    }
}
