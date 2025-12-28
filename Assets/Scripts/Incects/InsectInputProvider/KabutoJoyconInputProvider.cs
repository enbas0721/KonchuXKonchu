using System.Collections.Generic;
using UnityEngine;

public class KabutoJoyconInputProvider : MonoBehaviour, IInsectInputProvider
{
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon using_joycon;

    [SerializeField] private float accel_threshold_dodge = 2.0f;
    [SerializeField] private float accel_threshold_attack = 2.5f;

    private bool isEnabled = true;
    public void SetEnabled(bool enabled) => isEnabled = enabled;

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

        /*m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);*/

        if (m_joyconL == null && m_joycons.Count > 0) m_joyconL = m_joycons[0];
        if (m_joyconR == null && m_joycons.Count > 1) m_joyconR = m_joycons[1];

        using_joycon = m_joyconL;
    }

    private int CheckSwing(float swing_accel, float accel_th, bool flag)
    {
        if ((swing_accel >= accel_th) && flag) return 1;
        if (((-1f * swing_accel) >= 0.3f) && (!flag)) return 2;
        return 0;
    }

    public InsectInput GetInput()
    {
        if (!isEnabled) return default;

        if (m_joycons == null || m_joycons.Count <= 0 || using_joycon == null)
        {
            return default;
        }

        accel.x = (-1f) * using_joycon.GetAccel().y;
        accel.y = using_joycon.GetAccel().z;
        accel.z = (-1f) * using_joycon.GetAccel().x;

        float swing_accel_y = accel.y;
        int attack_flag = CheckSwing(swing_accel_y, accel_threshold_attack, attackable);

        float swing_accel_z = (-1f) * accel.z;
        int dodge_flag = CheckSwing(swing_accel_z, accel_threshold_dodge, dodgeable);

        if (attack_flag == 1) attackable = false;
        else if (attack_flag == 2) attackable = true;

        if (dodge_flag == 1) dodgeable = false;
        else if (dodge_flag == 2) dodgeable = true;

        return new InsectInput { AttackFlag = attack_flag, DodgeFlag = dodge_flag };
    }
}
