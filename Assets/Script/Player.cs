using System.Collections.Generic;
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

    [SerializeField]
    private GameObject shieldPrefab;
    private GameObject shieldDestination;   // 방패 Instantiate 정보
    private Transform shieldTransform;

    // 패링시 생길 Effect 정보
    [SerializeField]
    DoActionData parringData;

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
        shieldTransform = transform.FindChildByName("ShieldAttachPoint");
        if (swordPrefab != null)
        {
            swordDestination = Instantiate<GameObject>(swordPrefab, holsterTransform);
            sword = swordDestination.GetComponent<Sword>();
        }
        if(shieldPrefab != null)
        {
            shieldDestination = Instantiate<GameObject>(shieldPrefab, shieldTransform);
        }
    }

    private void Update()
    {
        if (GameManager.instance.isGameover)
            return;

        if (bHitting)
            return;
        if (bParring)
            return;

        UpdateMoving();
        UpdateDrawing();
        UpdateAttacking();
        UpdateGuarding();
    }

    private void UpdateMoving()
    {
        if (bAttacking)
            return;
        if (bGuarding)
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

    // 플레이어 피격 시 호출
    public override void OnDamage(Vector3 hitPoint,Vector3 hitNormal, GameObject attacker, DoActionData data)
    {
        Debug.Log("OnDamage " + this.gameObject.name);

        if(bParryExist)
        {
            // 여기부터
            // 패링 성공 시 적 기절 애니메이션 상태로 이동할 수 있게끔
            // 적 기절 애니메이션 중은 Critical 상태로 할 것
            Debug.Log("패링 성공");
            ReflectDamage(attacker);
            return;
        }
        FrameComponent.Instance.Delay(data.StopFrame);
        Debug.Log("Delay : " + data.StopFrame);

        // 만약 패링 타이밍 중 데미지를 받았다면

        if (data.HitParticle != null)
        {
            GameObject obj = Instantiate(data.HitParticle, transform, false);
            obj.transform.localPosition = data.HitParticlePositionOffset;
            obj.transform.localScale = data.HitParticleScaleOffset;
        }

        if (bBlocking)
        {
            Debug.Log("Player OnDamage 가드-> 데미지 무효");
            return;
        }

        base.OnDamage(hitPoint, hitNormal, attacker, data);


        healthBar.value = health;
        if (dead)
            return;

        animator.SetTrigger("Hitting");
        bHitting = true;
        bParring = false;
    }

    private void ReflectDamage(GameObject attacker)
    {
        LivingEntitiy living = attacker.GetComponent<LivingEntitiy>();
        if(living == null)
        {
            Debug.Log("ReflectDamage() null");
            return;
        }
        living.OnDamage(Vector3.zero, Vector3.zero, this.gameObject, parringData);
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
