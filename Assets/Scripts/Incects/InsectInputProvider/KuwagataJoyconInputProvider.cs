using System;
using System.Collections.Generic;
using UnityEngine;

public class KuwagataJoyconInputProvider : MonoBehaviour, IInsectInputProvider
{
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;

    private float closeswing_keep_timeR = 0.0f;
    private float closeswing_keep_timeL = 0.0f;
    private float openswing_keep_timeR = 0.0f;
    private float openswing_keep_timeL = 0.0f;

    private bool attackable = true;

    [SerializeField] private float accel_threshold = 0.03f;
    [SerializeField] private float swing_time_threshold = 0.02f;
    [SerializeField] private float closeGraceTime = 0.06f;
    [SerializeField] private float openGraceTime = 0.06f;

    private float closeGraceTimer = 0f;
    private float openGraceTimer = 0f;


    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;
        if (m_joycons == null || m_joycons.Count <= 0) return;

        // 左右はFindが安全
        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);

        // フォールバック
        if (m_joyconL == null && m_joycons.Count > 0) m_joyconL = m_joycons[0];
        if (m_joyconR == null && m_joycons.Count > 1) m_joyconR = m_joycons[1];
    }

    public InsectInput GetInput()
    {
        if (m_joycons == null || m_joycons.Count <= 0 || m_joyconL == null || m_joyconR == null)
            return default;

        float swing_accelR = m_joyconR.GetAccel().y;
        float swing_accelL = m_joyconL.GetAccel().y;

        bool closing = ((-1f * swing_accelR) >= accel_threshold) && (swing_accelL >= accel_threshold);
        bool opening = (swing_accelR >= accel_threshold) && ((-1f * swing_accelL) >= accel_threshold);

        int attack_flag = 0;

        // close 溜め -> 発動(1) -> 使用不可
        if (closing && attackable)
        {
            closeGraceTimer = closeGraceTime;
            closeswing_keep_timeR += Time.deltaTime;
            closeswing_keep_timeL += Time.deltaTime;

            if (closeswing_keep_timeR > swing_time_threshold &&
                closeswing_keep_timeL > swing_time_threshold)
            {
                attack_flag = 1;
                closeswing_keep_timeR = closeswing_keep_timeL = 0f;
                attackable = false;
            }
        }
        else
        {
            // 途切れても少しだけ猶予
            if (closeGraceTimer > 0f)
            {
                closeGraceTimer -= Time.deltaTime;
            }
            else
            {
                closeswing_keep_timeR = closeswing_keep_timeL = 0f;
            }
        }

        // open 溜め -> 再武装(2)
        // open 溜め -> 再武装
        if (opening && !attackable)
        {
            openGraceTimer = openGraceTime;

            openswing_keep_timeR += Time.deltaTime;
            openswing_keep_timeL += Time.deltaTime;

            if (openswing_keep_timeR > 0.005f &&
                openswing_keep_timeL > 0.005f)
            {
                attack_flag = 2; // 再武装
                openswing_keep_timeR = 0f;
                openswing_keep_timeL = 0f;
                attackable = true;
            }
        }
        else
        {
            if (openGraceTimer > 0f)
            {
                openGraceTimer -= Time.deltaTime;
            }
            else
            {
                openswing_keep_timeR = 0f;
                openswing_keep_timeL = 0f;
            }
        }


        return new InsectInput { AttackFlag = attack_flag, DodgeFlag = 0 };
    }
}
