using UnityEngine;

public class InsectController : InsectControllerBase
{
    private IInsectInputProvider inputProvider;

    private int attack_flag = 0;
    private int dodge_flag = 0;

    [Header("SFX")]
    [SerializeField] private AudioClip attack_sound;
    [SerializeField] private AudioClip dodge_sound;

    [Header("Special (optional)")]
    [SerializeField] private GameObject special_effect;
    [SerializeField] private AudioClip special_attack_sound;

    private bool special_attackable = false;

    protected override void Start()
    {
        base.Start();

        // BattleManager注入が基本。保険として拾うなら FindInterface を使う
        if (inputProvider == null)
        {
            inputProvider = FindInterface<IInsectInputProvider>();
            // 注入される前提なら Warning 程度でOK
            if (inputProvider == null)
                Debug.LogWarning($"{name}: InputProvider is not set yet. BattleManager should inject it.");
        }

        if (Opponent == null)
            Debug.LogWarning($"{name}: Opponent is not set. BattleManager should call SetOpponent().");

        if (special_effect != null) special_effect.SetActive(false);
    }

    private void Update()
    {
        UpdateAnimStateFromAnimator();

        if (inputProvider == null) return;

        var opp = Opponent;
        var oppState = (opp != null) ? opp.AnimState : InsectAnimState.Idle;

        var input = inputProvider.GetInput();
        attack_flag = input.AttackFlag;
        dodge_flag = input.DodgeFlag;

        if (Opponent != null &&
            Opponent.AnimState == InsectAnimState.Attack &&
            AnimState == InsectAnimState.Dodge)
        {
            special_attackable = true;
            if (special_effect != null) special_effect.SetActive(true);
        }

        // Attack
        switch (attack_flag)
        {
            case 1:
                if (AnimState == InsectAnimState.Idle)
                {
                    SetAttackAnim(true);

                    if (oppState == InsectAnimState.Attack || oppState == InsectAnimState.Idle)
                    {
                        Opponent?.TakeDamageDefault();

                        if (special_attackable)
                        {
                            if (special_attack_sound != null) audioSource.PlayOneShot(special_attack_sound);
                            special_attackable = false;
                            if (special_effect != null) special_effect.SetActive(false);
                        }
                        else
                        {
                            if (attack_sound != null) audioSource.PlayOneShot(attack_sound);
                        }
                    }

                    // 攻撃したフレームに回避を打ち消す（元の挙動維持）
                    dodge_flag = 0;
                }
                break;

            case 2:
                SetAttackAnim(false);
                break;
        }

        // Dodge
        switch (dodge_flag)
        {
            case 1:
                if (AnimState == InsectAnimState.Idle)
                {
                    anim.SetBool("dodgeOn", true);
                    if (dodge_sound != null) audioSource.PlayOneShot(dodge_sound);
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
