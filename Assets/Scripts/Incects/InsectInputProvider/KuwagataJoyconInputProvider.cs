using System.Collections.Generic;
using UnityEngine;

public class KuwagataJoyconInputProvider : MonoBehaviour, IInsectInputProvider
{
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;

    private bool isEnabled = true;
    public void SetEnabled(bool enabled) => isEnabled = enabled;

    [Header("Attack")]
    [SerializeField] private float accel_threshold = 2.0f;
    [SerializeField] private float swing_time_threshold = 0.02f;
    [SerializeField] private float close_grace_time = 0.06f;

    private float close_keep_timeR = 0.0f;
    private float close_keep_timeL = 0.0f;
    private float close_grace_timer = 0f;
    private bool attack_issued = false;

    [Header("Dodge")]
    [SerializeField] private float accel_threshold_dodge = 2.0f;
    [SerializeField] private float dodge_time_threshold = 0.02f;
    [SerializeField] private float pull_grace_time = 0.06f;
    
    private float pull_keep_timeR = 0f;
    private float pull_keep_timeL = 0f;
    private float pull_grace_timer = 0f;
    private bool dodge_issued = false;

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;
        if (m_joycons == null || m_joycons.Count <= 0)
        {
            Debug.Log("m_joycons is empty");
        };

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);

        if (m_joyconL == null && m_joycons.Count > 0) m_joyconL = m_joycons[0];
        if (m_joyconR == null && m_joycons.Count > 1) m_joyconR = m_joycons[1];
    }

    private static Vector3 MapAccel(Joycon j)
    {
        var a = j.GetAccel();
        return new Vector3((-1f) * a.y, a.z, (-1f) * a.x);
    }

    public InsectInput GetInput()
    {
        if (!isEnabled) return default;
        if (m_joyconL == null || m_joyconR == null) return default;

        var accel_R = MapAccel(m_joyconR);
        var accel_L = MapAccel(m_joyconL);

        float swing_accelR = accel_R.x;
        float swing_accelL = accel_L.x;

        bool closing = (((-1) * swing_accelR) >= accel_threshold) && 
                        (swing_accelL >= accel_threshold);

        bool attack_event = false;

        /* Attack */
        if (closing)
        {
            close_grace_timer = close_grace_time;

            close_keep_timeR += Time.deltaTime;
            close_keep_timeL += Time.deltaTime;

            if (!attack_issued &&
                close_keep_timeR > swing_time_threshold &&
                close_keep_timeL > swing_time_threshold)
            {
                attack_event = true;
                attack_issued = true;
            }
        }
        else
        {
            if (close_grace_timer > 0f)
            {
                close_grace_timer -= Time.deltaTime;
            }
            else
            {
                attack_issued = false;
                close_keep_timeR = close_keep_timeL = 0f;
            }
        }

        /* Dodge */
        bool pulling = (((-1) * accel_R.z) >= accel_threshold_dodge) ||
                       (((-1) * accel_L.z) >= accel_threshold_dodge);

        bool dodge_event = false;

        if (pulling)
        {
            pull_grace_timer = pull_grace_time;

            pull_keep_timeR += Time.deltaTime;
            pull_keep_timeL += Time.deltaTime;

            if (!dodge_issued &&
                pull_keep_timeR > dodge_time_threshold &&
                pull_keep_timeL > dodge_time_threshold)
            {
                dodge_event = true;
                dodge_issued = true;
            }
        }
        else
        {
            dodge_issued = false;

            if (pull_grace_timer > 0f)
            {
                pull_grace_timer -= Time.deltaTime;
            }
            else
            {
                pull_keep_timeR = pull_keep_timeL = 0f;
            }
        }

        return new InsectInput { Attack = attack_event, Dodge = dodge_event };
    }
}
