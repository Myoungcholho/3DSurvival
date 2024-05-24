using UnityEngine;

public interface IDamageable
{
    void OnDamage(Vector3 hitPoint, Vector3 hitNormal, GameObject attacker, DoActionData doActionData);
}
