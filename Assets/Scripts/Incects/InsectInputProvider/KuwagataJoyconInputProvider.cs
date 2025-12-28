using System.Collections.Generic;
using UnityEngine;

public class KuwagataJoyconInputProvider : MonoBehaviour, IInsectInputProvider
{
    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;

    private bool isEnabled = true;
    public void SetEnabled(bool enabled) => isEnabled = enabled;

    // --- Attack (close/open) ---
    private float closeswing_keep_timeR = 0.0f;
    private float closeswing_keep_timeL = 0.0f;
    private float openswing_keep_timeR = 0.0f;
    private float openswing_keep_timeL = 0.0f;
    private bool attackable = true;

    [Header("Attack")]
    [SerializeField] private float accel_threshold = 0.03f;
    [SerializeField] private float swing_time_threshold = 0.02f;
    [SerializeField] private float closeGraceTime = 0.06f;
    [SerializeField] private float openGraceTime = 0.06f;

    private float closeGraceTimer = 0f;
    private float openGraceTimer = 0f;

    // --- Dodge (pull) ---
    private float pull_keep_timeR = 0f;
    private float pull_keep_timeL = 0f;
    private bool dodgeable = true;

    [Header("Dodge")]
    [SerializeField] private float accel_threshold_dodge = 2.0f;
    [SerializeField] private float dodge_time_threshold = 0.02f;
    [SerializeField] private float pullGraceTime = 0.06f;
    private float pullGraceTimer = 0f;

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;
        if (m_joycons == null || m_joycons.Count <= 0)
        {
            Debug.Log("m_joycons == null!!");
        };

        /* m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft); */

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
        if (m_joycons == null || m_joycons.Count <= 0 || m_joyconL == null || m_joyconR == null)
        {
            Debug.LogError("Joycon could not find.");
            return default;
        }

        float swing_accelR = m_joyconR.GetAccel().y;
        float swing_accelL = m_joyconL.GetAccel().y;

        bool closing = ((-1f * swing_accelR) >= accel_threshold) && (swing_accelL >= accel_threshold);
        bool opening = (swing_accelR >= 0.05) && ((-1f * swing_accelL) >= 0.05);

        int attack_flag = 0;
        int dodge_flag = 0;

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
            if (closeGraceTimer > 0f) closeGraceTimer -= Time.deltaTime;
            else closeswing_keep_timeR = closeswing_keep_timeL = 0f;
        }

        if (opening && !attackable)
        {
            openGraceTimer = openGraceTime;
            openswing_keep_timeR += Time.deltaTime;
            openswing_keep_timeL += Time.deltaTime;

            if (openswing_keep_timeR > 0.005f &&
                openswing_keep_timeL > 0.005f)
            {
                attack_flag = 2;
                openswing_keep_timeR = openswing_keep_timeL = 0f;
                attackable = true;
            }
        }
        else
        {
            if (openGraceTimer > 0f) openGraceTimer -= Time.deltaTime;
            else openswing_keep_timeR = openswing_keep_timeL = 0f;
        }

        var accel_R = MapAccel(m_joyconR);
        var accel_L = MapAccel(m_joyconL);

        bool pulling = (accel_R.z >= accel_threshold_dodge) && (accel_L.z >= accel_threshold_dodge);

        if (pulling && dodgeable)
        {
            pullGraceTimer = pullGraceTime;
            pull_keep_timeR += Time.deltaTime;
            pull_keep_timeL += Time.deltaTime;

            if (pull_keep_timeR > dodge_time_threshold &&
                pull_keep_timeL > dodge_time_threshold)
            {
                dodge_flag = 1;
                pull_keep_timeR = pull_keep_timeL = 0f;
                dodgeable = false;
            }
        }
        else
        {
            if (pullGraceTimer > 0f) pullGraceTimer -= Time.deltaTime;
            else pull_keep_timeR = pull_keep_timeL = 0f;
        }

        bool releasePull = (accel_R.z <= 0.05f) && (accel_L.z <= 0.05f);
        if (!dodgeable && releasePull)
        {
            Debug.Log("Release Pull!!");
            dodge_flag = 2;
            dodgeable = true;
        }

        return new InsectInput { AttackFlag = attack_flag, DodgeFlag = dodge_flag };
    }
}
