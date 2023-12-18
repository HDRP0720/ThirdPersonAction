using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
  public static EnemyManager Instance { get; private set; }

  [SerializeField] private Vector2 _timeRangeBetweenAttacks = new Vector2(1, 4);
  
  private List<EnemyController> _enemiesInRange = new List<EnemyController>();
  private float _notAttackingTimer = 2f;

  private void Awake()
  {
    Instance = this;
  }
  private void Update()
  {
    if (_enemiesInRange.Count == 0) return;
    
    if (!_enemiesInRange.Any(e => e.IsInState(EEnemyStates.Attack)))
    {
      if (_notAttackingTimer > 0)
        _notAttackingTimer -= Time.deltaTime;

      if (_notAttackingTimer <= 0)
      {
        var attackingEnemy = SelectEnemyForAttack();
        attackingEnemy.ChangeState(EEnemyStates.Attack);
        _notAttackingTimer = Random.Range(_timeRangeBetweenAttacks.x, _timeRangeBetweenAttacks.y);
      }
    }
  }

  public void AddEnemyInRange(EnemyController enemy)
  {
    if(_enemiesInRange.Contains(enemy) == false)
      _enemiesInRange.Add(enemy);
  }
  public void RemoveEnemyInRange(EnemyController enemy)
  {
    _enemiesInRange.Remove(enemy);
  }

  public EnemyController GetAttackingEnemy()
  {
    return _enemiesInRange.FirstOrDefault(e => e.IsInState(EEnemyStates.Attack));
  }

  private EnemyController SelectEnemyForAttack()
  {
    // 에너미 숫자가 기하급수적으로 많아지는 경우엔 최적화에 좋지 못함 (For-loop등으로 자체 구현 필요)
    return _enemiesInRange.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault();
  }
}
