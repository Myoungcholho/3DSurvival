using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Zombie
{
    [SerializeField]
    private float distanceRadius = 0.1f;

    private Vector3 destPos;
    private float speed = 3f;
    private float distance = 3f;
    private float waitTime = 5f;

    private void MoveToRandomPosition()
    {
        if (hasTarget)
            return;

        if (outOfRange)
            return;

        if (IsAttack)
            return;

        if (IsGuard)
            return;

        if (isCritical2)
            return;

        Vector3 direction = destPos - transform.position;
        // �Ÿ��� ������ Walk ����
        if (Vector3.Distance(transform.position, destPos) < distanceRadius)
        {
            IsWalking = false;
            animator.SetBool("IsWalking", IsWalking);
        }
        else
        {
            IsWalking = true;
            animator.SetBool("IsWalking", IsWalking);
        }

        if (direction == Vector3.zero)
            return;

        Quaternion angle = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = angle;
        transform.position = Vector3.MoveTowards(transform.position, destPos, speed * Time.deltaTime);
    }

    // 5�ʸ��� ������ ��ġ�� ã��
    private IEnumerator RandomPositionGenerator()
    {
        while(true)
        {
            float randomAngle = UnityEngine.Random.Range(0f, Mathf.PI * 2f); // 0���� 360�� ������ ������ ����
            destPos = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
            destPos = destPos * distance;
            destPos.y = transform.position.y;

            yield return new WaitForSeconds(waitTime);
        }
    }
}