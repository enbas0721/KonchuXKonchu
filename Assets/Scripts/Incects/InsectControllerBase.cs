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
    [SerializeField] protected float damagePerAttack = 20f;
    [SerializeField] protected float damagePerSpecialAttack = 50f;
    [SerializeField] protected float hpMax = 300f;

    private InsectControllerBase opponent;

    public InsectControllerBase Opponent => opponent;
    public InsectAnimState AnimState { get; private set; } = InsectAnimState.Idle;
    public float Hp01 => attackedValue;
    public bool IsDead { get; private set; } = false;


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
        attackedValue = hpMax;
    }

    protected void UpdateGage(float val)
    {
        if (gage == null) return;
        gage.sizeDelta = new Vector2(val, gage.sizeDelta.y);
    }

    public void TakeDamageDefault()
    {
        ApplyDamageDefault();
    }

    public void TakeDamage(float damageValue)
    {
        ApplyDamage(damageValue);
    }

    protected void ApplyDamageDefault()
    {
        ApplyDamage(damagePerAttack);
    }

    protected void ApplyDamage(float damageValue)
    {
        if (IsDead) return;

        attackedValue -= damageValue;

        if (attackedValue <= 0f)
        {
            attackedValue = 0f;
            IsDead = true;
        }

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

    public void SetAttackAnim()
    {
        anim.SetTrigger("attackOn");
    }

    public void SetDodgeAnim()
    {
        anim.SetTrigger("dodgeOn");
    }
    public void SetOpponent(InsectControllerBase opp)
    {
        opponent = opp;
    }
}
