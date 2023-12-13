using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat System/Create a new attack")]
public class AttackData : ScriptableObject
{
  [field: SerializeField] public string AnimName { get; private set; }        // 공격 애니메이션의 이름
  [field: SerializeField] public EAttackHitbox HitboxToUse { get; private set; } 
  [field: SerializeField] public float ImpactStartTime { get; private set; }  // 공격 애니메이션에서 실제 공격 동작이 시작하는 순간 (전체 시간 대비 백분율)
  [field: SerializeField] public float ImpactEndTime { get; private set; }    // 공격 애니메이션에서 실제 공격 동작이 끝난 순간 (전체 시간 대비 백분율)
}

public enum EAttackHitbox {LeftHand, RightHand, LeftFoot, RightFoot, Weapon }
