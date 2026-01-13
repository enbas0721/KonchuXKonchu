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
    [SerializeField] private float accel_threshold = 0.8f;

    private bool attack_issued = false;

    [Header("Dodge")]
    [SerializeField] private float accel_threshold_dodge = 2.0f;
    
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

            if (!attack_issued)
            {
                attack_event = true;
                attack_issued = true;
            }
        }
        else
        {
            attack_issued = false;
        }

        /* Dodge */
        bool pulling = (((-1) * accel_R.z) >= accel_threshold_dodge) ||
                       (((-1) * accel_L.z) >= accel_threshold_dodge);

        bool dodge_event = false;

        if (pulling)
        {
            if (!dodge_issued)
            {
                dodge_event = true;
                dodge_issued = true;
            }
        }
        else
        {
            dodge_issued = false;
        }

        return new InsectInput { Attack = attack_event, Dodge = dodge_event };
    }
}
