public enum AttackState
{
    BasicAttack,   // 기본 공격
    StunAttack,    // 스턴 공격
    HeavyAttack,   // 강한 공격
    RangedAttack,  // 원거리 공격
    MagicAttack,   // 마법 공격
    Healing        // 치유
}

public enum CharacterState
{
    Normal,         // 기본 상태
    Stunned,        // 기절 상태
    Poisoned,       // 중독 상태
    Burning,        // 화상 상태
    Frozen,         // 얼린 상태
    Paralyzed,      // 마비 상태
    Healing         // 치유 상태
}