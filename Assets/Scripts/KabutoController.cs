using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KabutoController : MonoBehaviour
{
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    // private Joycon.Button? m_pressedButtonL;
    // private Joycon.Button? m_pressedButtonR;

    private float swing_accel = 0.0f;
    private float upswing_keep_time = 0.0f;
    private float downswing_keep_time = 0.0f;
    private bool attackable = true;
    private int num = 0;

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
        m_joyconL = m_joycons[2];
        m_joyconR = m_joycons[3];

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

        swing_accel = m_joyconR.GetAccel().z;
        
        // 攻撃判定、振り下ろし動作後の処理によりattackableがTrueかつ、
        // 上下軸で一定時間、一定の加速度以上の運動が行われたら攻撃
        // 前回の処理からの時間を加速度にかけて加速度ではなく速度で判定すべき？
        if ((swing_accel >= accel_threshold) && (upswing_keep_time > swing_time_threshold) && attackable)
        {
            // 攻撃時アクション
            num += 1;
            upswing_keep_time = 0.0f;
            attackable = false;

            attacked_value -= 0.02f;
            anim.SetBool("attackOn", true);
            audioSource.PlayOneShot(attack_sound);
            UpdateGage(attacked_value);
            // 相手のHPが０になったら
            if(attacked_value <= 0f)
            {
                attacked_value = 1f;
            }
        }
        else if((swing_accel >= accel_threshold)  && attackable)
        {
            upswing_keep_time += Time.deltaTime;
        }
        else if (((-1 * swing_accel) >= accel_threshold) && (downswing_keep_time > 0.005) && !attackable)
        {
            attackable = true;
            downswing_keep_time = 0.0f;
        }
        else if(((-1 * swing_accel) >= accel_threshold) && (!attackable))
        {
            downswing_keep_time = Time.deltaTime;
            anim.SetBool("attackOn", false);
        }
        else
        {
            upswing_keep_time = 0.0f;
            downswing_keep_time = 0.0f;
        }
    }
}
