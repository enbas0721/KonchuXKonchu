using UnityEngine;
public enum InsectAnimState
{
    Idle,
    Attack,
    Dodge
}

[RequireComponent(typeof(AudioSource))]
public abstract class InsectControllerBase : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] protected RectTransform gage;

    [Header("HP")]
    [SerializeField] protected float damagePerAttack = 0.02f;
    [SerializeField] protected float hpMaxNormalized = 1f;

    [Header("Battle")]
    [SerializeField] private InsectControllerBase opponent;

    public InsectControllerBase Opponent => opponent;

    public InsectAnimState AnimState { get; private set; } = InsectAnimState.Idle;

    protected float gageMaxWidth;
    protected float attackedValue01;

    protected AudioSource audioSource;
    protected Animator anim;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        if (gage != null) gageMaxWidth = gage.sizeDelta.x;
        attackedValue01 = hpMaxNormalized;
        UpdateGage(attackedValue01);
    }
    protected T FindInterface<T>() where T : class
    {
        foreach (var mb in GetComponents<MonoBehaviour>())
            if (mb is T t) return t;
        return null;
    }

    protected void UpdateGage(float t01)
    {
        if (gage == null) return;
        float x = Mathf.Lerp(0f, gageMaxWidth, Mathf.Clamp01(t01));
        gage.sizeDelta = new Vector2(x, gage.sizeDelta.y);
    }

    protected void ApplyDamageDefault()
    {
        ApplyDamage(damagePerAttack);
    }

    protected void ApplyDamage(float damage01)
    {
        attackedValue01 -= damage01;
        if (attackedValue01 <= 0f) attackedValue01 = hpMaxNormalized;
        UpdateGage(attackedValue01);
    }
    protected virtual void UpdateAnimStateFromAnimator()
    {
        if (anim == null) return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) AnimState = InsectAnimState.Idle;
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) AnimState = InsectAnimState.Attack;
        else AnimState = InsectAnimState.Dodge;
    }
    public void SetAttackAnim(bool on)
    {
        anim.SetBool("attackOn", on);
    }
    public void SetOpponent(InsectControllerBase opp)
    {
        opponent = opp;
    }
}
