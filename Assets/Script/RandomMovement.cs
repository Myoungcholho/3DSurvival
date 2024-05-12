using System.Collections;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    private Vector3 destPos;
    private float speed = 3f;
    private float distance = 10f;
    private float waitTime = 5f;
    private LivingEntitiy entity;
    private Zombie zombie;

    private void Awake()
    {
        entity = GetComponent<LivingEntitiy>();
        zombie = GetComponent<Zombie>();
    }

    private void Start()
    {
        //if(entity != null)
        StartCoroutine(RandomPositionGenerator());
    }

    private void Update()
    {
        MoveToRandomPosition();
    }

    private void MoveToRandomPosition()
    {
        if (zombie.hasTarget)
            return;

        if (zombie.outOfRange)
            return;

        zombie.IsWalking = true;
        transform.position = Vector3.MoveTowards(transform.position, destPos, speed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(destPos.normalized);
    }

    // 5초마다 랜덤한 위치를 찾음
    private IEnumerator RandomPositionGenerator()
    {
        //while (!entity.dead)
        while(true)
        {
            float randomAngle = Random.Range(0f, Mathf.PI * 2f); // 0부터 360도 사이의 랜덤한 각도
            destPos = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
            destPos = destPos * distance;
            destPos.y = transform.position.y;

            yield return new WaitForSeconds(waitTime);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(transform.position, destPos, Color.red);
    }
}
