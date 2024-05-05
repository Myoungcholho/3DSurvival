using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public partial class Player : MonoBehaviour
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

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        UpdateMoving();
        UpdateDrawing();
        UpdateAttacking();

    }

    private void UpdateMoving()
    {
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

    private void OnGUI()
    {
        GUI.color = Color.red;
        //GUI.Label(new Rect(10, 10, 100, 20), $"horzontal : {horizontal}");
        //GUI.Label(new Rect(10, 20, 100, 20), $"vertical : {vertical}");
    }
}
