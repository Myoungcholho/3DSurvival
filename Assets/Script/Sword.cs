using UnityEngine;

public class Sword : MonoBehaviour
{
    private new Collider collider;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        End_Collision();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (transform.root.gameObject == other.gameObject)
            return;

        // �浹 ������ ���� ���� ���� ����


        print(other.gameObject.name);
    }

    public void Begin_Collision()
    {
        collider.enabled = true;
    }

    public void End_Collision()
    {
        collider.enabled = false;
    }
}