using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshHighlighter : MonoBehaviour
{
  [SerializeField] private List<SkinnedMeshRenderer> _meshesToHighlight;
  [SerializeField] private Material _originalMaterial;
  [SerializeField] private Material _highlightMaterial;

  public void HighlightMesh(bool isHighlight)
  {
    foreach (var mesh in _meshesToHighlight)
      mesh.material = (isHighlight) ? _highlightMaterial : _originalMaterial;
  }
}
