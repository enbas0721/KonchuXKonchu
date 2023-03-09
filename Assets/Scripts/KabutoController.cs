using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KabutoController : MonoBehaviour
{
    private static readonly Joycon.Button[] m_buttons =
        Enum.GetValues(typeof(Joycon.Button)) as Joycon.Button[];

    private List<Joycon> m_joycons;
    private Joycon m_joyconL;
    private Joycon m_joyconR;
    private Joycon using_joycon;
    // private Joycon.Button? m_pressedButtonL;
    // private Joycon.Button? m_pressedButtonR;

    public string anim_state = "Idle";
    private string opponent_state = "Idle";
    private string opponent_player = "2P";

    private int attack_flag = -1;
    private int dodge_flag = -1;
    private float swing_accel_z = 0.0f;
    private float swing_accel_y = 0.0f;
    private bool attackable = true;
    private bool dodgeable = true;
    private bool special_attackable = false;

    private Vector3 accel;

    private float maxHP;
    private float attacked_value;

    private AudioSource audioSource;

    private Animator anim;

    [SerializeField]
    private string player = "1P";
    [SerializeField]
    private float accel_threshold_dodge = 2.5f;
    [SerializeField]
    private float accel_threshold_attack = 3.0f;
    [SerializeField]
    private RectTransform gage;
    [SerializeField]
    private AudioClip attack_sound;
    [SerializeField]
    private AudioClip dodge_sound;
    [SerializeField]
    private GameObject special_effect;
    [SerializeField]
    private AudioClip special_effect_sound;
    [SerializeField]
    private AudioClip special_attack_sound;

    private void Start()
    {
        m_joycons = JoyconManager.Instance.j;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        m_joyconL = m_joycons.Find(c => c.isLeft);
        m_joyconR = m_joycons.Find(c => !c.isLeft);

        if (player == "1P")
        {
            using_joycon = m_joyconL;
            opponent_player = "2P";
        }
        else
        {
            using_joycon = m_joyconR;
            opponent_player = "1P";
        }

        maxHP = gage.sizeDelta.x;
        attacked_value = 1f;

        audioSource = GetComponent<AudioSource>();

        anim = gameObject.GetComponent<Animator>();
    }

    private void UpdateGage(float t)
    {
        float x = Mathf.Lerp(0, maxHP, t);
        gage.sizeDelta = new Vector2(x, gage.sizeDelta.y);
    }

    private int CheckSwing(float swing_accel, float accel_th, bool flag)
    {
        if ((swing_accel >= accel_th) && flag) 
        {
            return 1;
        }
        else if (((-1 * swing_accel) >= 0.3) && (!flag)) 
        {
            return 2;
        }
        else 
        {
            return 0;
        }
    }

    void Update()
    {
        // m_pressedButtonL = null;
        // m_pressedButtonR = null;

        if (m_joycons == null || m_joycons.Count <= 0) return;

        opponent_state = GameObject.Find(opponent_player).GetComponent<KabutoController>().anim_state;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            anim_state = "Idle";
        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            anim_state = "Attack";
        }
        else
        {
            anim_state = "Dodge";
        }

        accel.x = (-1) * using_joycon.GetAccel().y;
        accel.y = using_joycon.GetAccel().z;
        accel.z = (-1) * using_joycon.GetAccel().x;

        Vector3 new_accel = accel;

        swing_accel_y = new_accel.y;
        attack_flag = CheckSwing(swing_accel_y, accel_threshold_attack, attackable);

        swing_accel_z = (-1) * new_accel.z;
        dodge_flag = CheckSwing(swing_accel_z, accel_threshold_dodge, dodgeable);

        if ((opponent_state == "Attack") && (anim_state == "Dodge"))
        {
            special_attackable = true;
            special_effect.SetActive(true);
            audioSource.PlayOneShot(special_effect_sound);
        }

        switch (attack_flag)
        {
            // 一定の加速度以上の上スイングが行われ、かつattackableがtrueだったら攻撃
            case 1:
                if (anim_state == "Idle")
                {
                    attackable = false;

                    anim.SetBool("attackOn", true);
                    if ((opponent_state == "Attack") || (opponent_state == "Idle"))
                    {
                        if (!special_attackable)
                        {
                            attacked_value -= 0.02f;
                            UpdateGage(attacked_value);
                            audioSource.PlayOneShot(attack_sound);
                        }
                        else
                        {
                            attacked_value -= 0.1f;
                            UpdateGage(attacked_value);
                            audioSource.PlayOneShot(special_attack_sound);
                            special_attackable = false;
                            special_effect.SetActive(false);
                        }
                    }
                    // 相手のHPが０になったら
                    if (attacked_value <= 0f)
                    {
                        attacked_value = 1f;
                    }
                    dodge_flag = 0;
                }
                break;
            // 一定以上の加速度で上スイング中だが一定時間を超えていないときは
            // 上スイング継続時間をプラス
            case 2:
                attackable = true;
                anim.SetBool("attackOn", false);
                break;
            case 0:
                break;
            default:
                Debug.Log("Exception");
                break;
        }
        switch (dodge_flag)
        {
            // 一定の加速度以上のpullが行われ、かつdodgeableがtrueだったら攻撃
            case 1:
                if (anim_state == "Idle")
                {
                    dodgeable = false;
                    anim.SetBool("dodgeOn", true);
                    audioSource.PlayOneShot(dodge_sound);
                }
                break;
            // 一定以上の加速度でpushされたら再度避け可能に
            case 2:
                dodgeable = true;
                anim.SetBool("dodgeOn", false);
                break;
            case 0:
                break;
            default:
                Debug.Log("Exception");
                break;
        }
    }
}
