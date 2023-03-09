using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KuwagataController : MonoBehaviour
{
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    // private Joycon.Button? m_pressedButtonL;
    // private Joycon.Button? m_pressedButtonR;

    private float swing_accelR = 0.0f;
    private float swing_accelL = 0.0f;
    private float closeswing_keep_timeR = 0.0f;
    private float closeswing_keep_timeL = 0.0f;
    private float openswing_keep_timeR = 0.0f;
    private float openswing_keep_timeL = 0.0f;
    private bool attackable = true;
    private int num;

    private float maxHP;
    private float attacked_value;

    private AudioSource audioSource;

    private Animator anim;

    [SerializeField]
    private float accel_threshold = 0.3f;
    [SerializeField]
    private float swing_time_threshold = 0.2f;
    [SerializeField]
    private RectTransform gage;
    [SerializeField]
    private AudioClip attack_sound;

    // Start is called before the first frame update
    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        // m_joyconL = m_joycons.Find(c => c.isLeft);
        // m_joyconR = m_joycons.Find(c => !c.isLeft);
        m_joyconL = m_joycons[0];
        m_joyconR = m_joycons[1];

        maxHP = gage.sizeDelta.x;
        attacked_value = 1f;

        audioSource = GetComponent<AudioSource>();

        anim = gameObject.GetComponent<Animator>();
    }

    private void UpdateGage(float t)
    {
        float x = Mathf.Lerp(0f, maxHP, t);
        gage.sizeDelta = new Vector2(x, gage.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        // m_pressedButtonL = null;
        // m_pressedButtonR = null;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        swing_accelR = m_joyconR.GetAccel().y;
        swing_accelL = m_joyconL.GetAccel().y;

        if (((-1 * swing_accelR) >= accel_threshold) && (swing_accelL >= accel_threshold) && 
            (closeswing_keep_timeR > swing_time_threshold) && (closeswing_keep_timeL > swing_time_threshold) && attackable)
        {
            // çUåÇéûÉAÉNÉVÉáÉì
            num += 1;
            closeswing_keep_timeR = 0.0f;
            closeswing_keep_timeL = 0.0f;
            attackable = false;

            attacked_value -= 0.02f;
            anim.SetBool("attackOn", true);
            audioSource.PlayOneShot(attack_sound);
            UpdateGage(attacked_value);
            // ëäéËÇÃHPÇ™ÇOÇ…Ç»Ç¡ÇΩÇÁ
            if(attacked_value <= 0f)
            {
                attacked_value = 1f;
            }

        }
        else if(((-1 * swing_accelR) >= accel_threshold) && (swing_accelL >= accel_threshold) && attackable)
        {
            closeswing_keep_timeR += Time.deltaTime;
            closeswing_keep_timeL += Time.deltaTime;
            anim.SetBool("attackOn", false);
        }
        else if ((swing_accelR >= accel_threshold) && ((-1 * swing_accelL) >= accel_threshold) && (openswing_keep_timeR > 0.005) && (openswing_keep_timeL > 0.005) && !attackable)
        {
            attackable = true;
            openswing_keep_timeR = 0.0f;
            openswing_keep_timeL = 0.0f;
            anim.SetBool("attackOn", false);
        }
        else if((swing_accelR >= accel_threshold) && ((-1 * swing_accelL) >= accel_threshold) && (!attackable))
        {
           openswing_keep_timeR = Time.deltaTime;
           openswing_keep_timeL = Time.deltaTime;
           anim.SetBool("attackOn", false);
        }
        else if (((swing_accelR >= accel_threshold) || ((-1 * swing_accelL) >= accel_threshold)) && (!attackable))
        {
            anim.SetBool("attackOn", false);
        }
        else
        {
            closeswing_keep_timeR = 0.0f;
            closeswing_keep_timeL = 0.0f;
            openswing_keep_timeR = 0.0f;
            openswing_keep_timeR = 0.0f;
        }
    }
}
