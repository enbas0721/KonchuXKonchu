using UnityEngine;

public class InsectController : InsectControllerBase
{
    private IInsectInputProvider inputProvider;

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
    }

    private void Update()
    {
        UpdateAnimStateFromAnimator();

        if (inputProvider == null) return;

        var opp = Opponent;
        var opp_state = (opp != null) ? opp.AnimState : InsectAnimState.Idle;

        var input = inputProvider.GetInput();

        if (Opponent != null &&
            Opponent.AnimState == InsectAnimState.Attack &&
            AnimState == InsectAnimState.Dodge)
        {
            special_attackable = true;
            if (special_effect != null) special_effect.SetActive(true);
        }

        // Attack
        if (input.Attack && AnimState == InsectAnimState.Idle && !anim.IsInTransition(0))
        {
            SetAttackAnim();

            if (opp_state != InsectAnimState.Dodge)
            {
                if (special_attackable)
                {
                    Opponent?.TakeDamage(damagePerSpecialAttack);
                    if (special_attack_sound != null) audioSource.PlayOneShot(special_attack_sound);
                }
                else
                {
                    Opponent?.TakeDamageDefault();
                    if (attack_sound != null) audioSource.PlayOneShot(attack_sound);
                }
            }
            if (special_attackable)
            {
                special_attackable = false;
                if (special_effect != null) special_effect.SetActive(false);
            }
        }

        // Dodge
        if (input.Dodge && AnimState == InsectAnimState.Idle && !anim.IsInTransition(0))
        {
            SetDodgeAnim();
            if (dodge_sound != null) audioSource.PlayOneShot(dodge_sound);
        }
    }

    public void SetInputProvider(IInsectInputProvider provider)
    {
        inputProvider = provider;
    }
}
