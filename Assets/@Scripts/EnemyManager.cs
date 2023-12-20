using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
  public static EnemyManager Instance { get; private set; }

  [SerializeField] private Vector2 _timeRangeBetweenAttacks = new Vector2(1, 4);
  [SerializeField] private CombatController _player;
  
  private List<EnemyController> _enemiesInRange = new List<EnemyController>();
  private float _notAttackingTimer = 2f;
  private float _findTargetTimer = 0f;

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
        if (attackingEnemy != null)
        {
          attackingEnemy.ChangeState(EEnemyStates.Attack);
          _notAttackingTimer = Random.Range(_timeRangeBetweenAttacks.x, _timeRangeBetweenAttacks.y);
        }
      }
    }

    if (_findTargetTimer > 0.1f)
    {
      _findTargetTimer = 0f;
      var closestEnemy = GetClosestEnemyToPlayerDir();
      if (closestEnemy != null && closestEnemy != _player.TargetEnemy)
      {
        var prevEnemy = _player.TargetEnemy;
        _player.TargetEnemy = closestEnemy;
        
        if(_player.TargetEnemy != null)
          _player.TargetEnemy.MeshHighlighter.HighlightMesh(true);
        
        if(prevEnemy != null)
          prevEnemy.MeshHighlighter.HighlightMesh(false);
      }
    }
    
    _findTargetTimer += Time.deltaTime;
  }

  public void AddEnemyInRange(EnemyController enemy)
  {
    if(_enemiesInRange.Contains(enemy) == false)
      _enemiesInRange.Add(enemy);
  }
  public void RemoveEnemyInRange(EnemyController enemy)
  {
    _enemiesInRange.Remove(enemy);

    if (enemy == _player.TargetEnemy)
    {
      enemy.MeshHighlighter.HighlightMesh(false);
      _player.TargetEnemy = GetClosestEnemyToPlayerDir();
      
      if(_player.TargetEnemy != null)
        _player.TargetEnemy.MeshHighlighter.HighlightMesh(true);
    }
  }

  public EnemyController GetAttackingEnemy()
  {
    return _enemiesInRange.FirstOrDefault(e => e.IsInState(EEnemyStates.Attack));
  }

  private EnemyController SelectEnemyForAttack()
  {
    // 에너미 숫자가 기하급수적으로 많아지는 경우엔 최적화에 좋지 못함 (For-loop등으로 자체 구현 필요)
    return _enemiesInRange.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault(e => e.Target != null);
  }

  public EnemyController GetClosestEnemyToPlayerDir()
  {
    var targetingDir = _player.GetTargetingDir();
    float minDistance = Mathf.Infinity;
    EnemyController cloestEnemy = null;

    foreach (var enemy in _enemiesInRange)
    {
      var vecToEnemy = enemy.transform.position - _player.transform.position;
      vecToEnemy.y = 0f;
      
      var angle = Vector3.Angle(targetingDir, vecToEnemy);
      float distance = vecToEnemy.magnitude * Mathf.Sin(angle * Mathf.Deg2Rad);

      if (distance < minDistance)
      {
        minDistance = distance;
        cloestEnemy = enemy;
      }
    }

    return cloestEnemy;
  }
}
