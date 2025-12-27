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

    [Header("HP")]
    [SerializeField] protected float damagePerAttack = 0.02f;
    [SerializeField] protected float hpMax = 300f;

    [Header("Battle")]
    [SerializeField] private InsectControllerBase opponent;

    public InsectControllerBase Opponent => opponent;

    public InsectAnimState AnimState { get; private set; } = InsectAnimState.Idle;

    protected RectTransform gage;
    protected float gageMaxWidth;

    protected float attackedValue;

    protected AudioSource audioSource;
    protected Animator anim;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Start()
    {

    }
    protected T FindInterface<T>() where T : class
    {
        foreach (var mb in GetComponents<MonoBehaviour>())
            if (mb is T t) return t;
        return null;
    }

    protected void UpdateGage(float val)
    {
        if (gage == null) return;
        Debug.Log("val: " + val);
        gage.sizeDelta = new Vector2(val, gage.sizeDelta.y);
        Debug.Log("gage.sizeDelata: " + gage.sizeDelta);
    }

    protected void ApplyDamageDefault()
    {
        ApplyDamage(damagePerAttack);
    }

    protected void ApplyDamage(float damageValue)
    {
        attackedValue -= damageValue;
        /* [TODO] 現在はHPが0になったらループ。本来はゲーム終了。 */
        if (attackedValue <= 0f) attackedValue = hpMax;
        UpdateGage(attackedValue);
    }
    protected virtual void UpdateAnimStateFromAnimator()
    {
        if (anim == null) return;

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) AnimState = InsectAnimState.Idle;
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) AnimState = InsectAnimState.Attack;
        else AnimState = InsectAnimState.Dodge;
    }

    public void SetGage(RectTransform g)
    {
        gage = g;
        if (gage != null) gageMaxWidth = gage.sizeDelta.x;
        UpdateGage(gageMaxWidth);
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
