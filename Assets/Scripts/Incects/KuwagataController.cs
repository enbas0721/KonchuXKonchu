using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KuwagataController : InsectControllerBase
{
    private IInsectInputProvider inputProvider;

    private int attack_flag = 0;
    private int dodge_flag = 0;

    private bool special_attackable = false;

    [SerializeField] private AudioClip attack_sound;
    [SerializeField] private AudioClip dodge_sound;
    [SerializeField] private GameObject special_effect;
    [SerializeField] private AudioClip special_attack_sound;

    protected override void Start()
    {
        base.Start();

        inputProvider = GetComponent<IInsectInputProvider>();
        if (inputProvider == null)
        {
            Debug.LogError("IInsectInputProvider not found. Attach KuwagataJoyconInputProvider.");
        }

        if (Opponent == null)
        {
            Debug.LogWarning($"{name}: Opponent is not set. BattleManager should call SetOpponent().");
        }

        if (special_effect != null) special_effect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimStateFromAnimator();

        var opp = Opponent;
        var oppState = (opp != null) ? opp.AnimState : InsectAnimState.Idle;

        if (inputProvider == null) return;

        var input = inputProvider.GetInput();
        attack_flag = input.AttackFlag;
        dodge_flag = input.DodgeFlag;

        if (Opponent != null && Opponent.AnimState == InsectAnimState.Attack && AnimState == InsectAnimState.Dodge)
        {
            special_attackable = true;
            if (special_effect != null) special_effect.SetActive(true);
        }

        switch (attack_flag)
        {
            case 1:
                if (AnimState == InsectAnimState.Idle)
                {
                    SetAttackAnim(true);

                    if ((oppState == InsectAnimState.Attack) || (oppState == InsectAnimState.Idle))
                    {
                        if (!special_attackable)
                        {
                            ApplyDamageDefault();
                            audioSource.PlayOneShot(attack_sound);
                        }
                        else
                        {
                            ApplyDamageDefault();
                            audioSource.PlayOneShot(special_attack_sound);
                            special_attackable = false;
                            if (special_effect != null) special_effect.SetActive(false);
                        }
                    }
                    dodge_flag = 0;
                }
                break;

            case 2:
                SetAttackAnim(false);
                break;
            default:
                break;
        }

        switch (dodge_flag)
        {
            case 1:
                if (AnimState == InsectAnimState.Idle)
                {
                    anim.SetBool("dodgeOn", true);
                    audioSource.PlayOneShot(dodge_sound);
                }
                break;

            case 2:
                anim.SetBool("dodgeOn", false);
                break;
        }
    }
    public void SetInputProvider(IInsectInputProvider provider)
    {
        inputProvider = provider;
    }

}
