using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
  [SerializeField] private List<Neighbour> _neighbours;

  private void Awake()
  {
    var twoWayNeighbours = _neighbours.Where(n => n.isTwoWay);
    foreach (var neighbour in twoWayNeighbours)
    {
      if(neighbour.point != null)
        neighbour.point.CreateConnection(this, -neighbour.direction, neighbour.connectionType, neighbour.isTwoWay);
    }
  }

  private void CreateConnection(ClimbPoint point, Vector2 direction, EConnectionType connectionType, bool isTwoWay = true)
  {
    var neighbour = new Neighbour()
    {
      point = point,
      direction = direction,
      connectionType = connectionType,
      isTwoWay = isTwoWay
    };
    
    _neighbours.Add(neighbour);
  }

  private void OnDrawGizmos()
  {
    Debug.DrawRay(transform.position, transform.forward, Color.blue);
    
    foreach (var neighbour in _neighbours)
    {
      if (neighbour.point != null)
      {
        Debug.DrawLine(transform.position, neighbour.point.transform.position, neighbour.isTwoWay ? Color.green : Color.gray);
      }
    }
  }
}

[System.Serializable]
public class Neighbour
{
  public ClimbPoint point;
  public Vector2 direction;
  public EConnectionType connectionType;
  public bool isTwoWay = true;
}

public enum EConnectionType { Jump, Move }
