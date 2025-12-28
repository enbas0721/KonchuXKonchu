using System.Collections.Generic;
using UnityEngine;

public class KabutoJoyconInputProvider : MonoBehaviour, IInsectInputProvider
{
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon using_joycon;

    [Header("Thresholds")]
    [SerializeField] private float accel_threshold_dodge = 2.0f;
    [SerializeField] private float accel_threshold_attack = 2.5f;

    [Header("Rearm (gesture release)")]
    [SerializeField] private float rearm_threshold = 0.3f;

    private bool is_enabled = true;
    public void SetEnabled(bool enabled) => is_enabled = enabled;

    private bool attackable = true;
    private bool dodgeable = true;

    private Vector3 accel;

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;
        if (m_joycons == null || m_joycons.Count <= 0)
        {
            Debug.Log("m_joycons == null!!");
        };

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);

        if (m_joyconL == null && m_joycons.Count > 0) m_joyconL = m_joycons[0];
        if (m_joyconR == null && m_joycons.Count > 1) m_joyconR = m_joycons[1];

        using_joycon = m_joyconL;
    }

    private static Vector3 MapAccel(Joycon j)
    {
        var a = j.GetAccel();
        return new Vector3((-1f) * a.y, a.z, (-1f) * a.x);
    }

    public InsectInput GetInput()
    {
        if (!is_enabled) return default;

        if (using_joycon == null) return default;

        accel = MapAccel(using_joycon);

        float swing_accel_attack = accel.y;
        float swing_accel_dodge = (-1f) * accel.z;

        bool attack_event = false;
        bool dodge_event = false;

        if (attackable)
        {
            if (swing_accel_attack >= accel_threshold_attack)
            {
                attackable = false;
                attack_event = true;
            }
        }
        else
        {
            // ‹t‘¤‚Ì“®‚«‚ÅÄ•‘•
            if ((-1f * swing_accel_attack) >= rearm_threshold)
            {
                attackable = true;
            }
        }

        if (dodgeable)
        {
            if (swing_accel_dodge >= accel_threshold_dodge)
            {
                dodge_event = true;
                dodgeable = false;
            }
        }
        else
        {
            if ((-1f * swing_accel_dodge) >= rearm_threshold)
            {
                dodgeable = true;
            }
        }

        return new InsectInput { Attack = attack_event, Dodge = dodge_event };
    }
}
