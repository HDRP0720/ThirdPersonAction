using UnityEngine;

public class VisionSensor : MonoBehaviour
{
  [SerializeField] private EnemyController _enemy;

  private void Awake()
  {
    _enemy.VisionSensor = this;
  }
  private void OnTriggerEnter(Collider other)
  {
    var fighter = other.GetComponent<MeeleCombat>();
    if (fighter != null)
    {
      _enemy.TargetsInRange.Add(fighter);
      EnemyManager.Instance.AddEnemyInRange(_enemy);
    }
  }
  private void OnTriggerExit(Collider other)
  {
    var fighter = other.GetComponent<MeeleCombat>();
    if (fighter != null)
    {
      _enemy.TargetsInRange.Remove(fighter);
      EnemyManager.Instance.RemoveEnemyInRange(_enemy);
    }
  }
}
