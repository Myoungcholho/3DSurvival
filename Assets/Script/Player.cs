using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

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

    // 플레이어 콜라이더
    private new Collider collider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        SetUp();
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

        UpdateMoving();
        UpdateDrawing();
        UpdateAttacking();
    }

    private void SetUp()
    {
        health = startingHealth;
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

    public virtual void OnDamage(float damage,Vector3 hitPoint,Vector3 hitNormal)
    {

    }

    public override void Die()
    {
        base.Die();
        animator.SetTrigger("IsDead");
        collider.enabled = false;
    }
}
