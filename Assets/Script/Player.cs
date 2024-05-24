using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public partial class Player : LivingEntitiy
{
    [SerializeField]
    private float walkSpeed = 2.0f;
    [SerializeField]
    private float runSpeed = 4.0f;
    private float speed;
    private float horizontal;
    private float vertical;
    private bool bRun;
    Vector3 direction;

    [SerializeField]
    private GameObject swordPrefab;
    private GameObject swordDestination; // 무기 Instantiate 정보
    private Sword sword; // 무기의 스크립트
    private Transform holsterTransform;
    private Transform handTransform;

    private Animator animator;
    private bool bHitting;

    // 플레이어 콜라이더
    private new Collider collider;

    // UI HP bar
    public Slider healthBar;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        healthBar.gameObject.SetActive(true);
        healthBar.maxValue = startingHealth;
        healthBar.value = health;
    }

    private void Start()
    {
        holsterTransform = transform.FindChildByName("Holster");
        handTransform = transform.FindChildByName("HolderWeapon");
        if (swordPrefab != null)
        {
            swordDestination = Instantiate<GameObject>(swordPrefab, holsterTransform);
            sword = swordDestination.GetComponent<Sword>();
        }
    }

    private void Update()
    {
        if (GameManager.instance.isGameover)
            return;

        if (bHitting)
            return;

        UpdateMoving();
        UpdateDrawing();
        UpdateAttacking();
    }

    private void UpdateMoving()
    {
        if (bAttacking)
            return;


        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        bool bRun = Input.GetButton("Run");
        speed = bRun ? runSpeed : walkSpeed;
        direction = (Vector3.forward * vertical) + (Vector3.right * horizontal);
        direction = direction.normalized * speed;
        transform.Translate(direction * Time.deltaTime);
        animator.SetFloat("SpeedX", direction.x);
        animator.SetFloat("SpeedZ", direction.z);
    }

    public override void OnDamage(Vector3 hitPoint,Vector3 hitNormal, GameObject attacker, DoActionData doActionData)
    {
        base.OnDamage(hitPoint, hitNormal, attacker, doActionData);
        healthBar.value = health;
        if (dead)
            return;

        animator.SetTrigger("Hitting");
        bHitting = true;
        
    }

    private void End_Hitting()
    {
        bHitting = false;
    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("IsDead");
        collider.enabled = false;
    }
}
