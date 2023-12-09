using UnityEngine;

public class CameraController : MonoBehaviour
{
  [SerializeField] private Transform _followTarget;         // 카메라 주시 대상
  [SerializeField] private float _distance = 5f;            // 캐릭터-카메라 거리 조절
  [SerializeField] private float _rotationSpeed = 2f;       // 카메라 회전 속도
  [SerializeField] private float _minVerticalAngle = -20;   // x축 회전 최소 각도
  [SerializeField] private float _maxVerticalAngle = 45;    // x축 회전 최대 각도

  [SerializeField] private float _zoomSpeed = 1.0f;
  [SerializeField] private float _minZoomDistance = 1.5f;
  [SerializeField] private float _maxZoomDistance = 8.0f;

  [SerializeField] private Vector2 _framingOffset;          // 캐릭터-카메라 높이 조절 (피봇)

  [SerializeField] private bool _invertX;                   // x축 회전 방향 반전
  [SerializeField] private bool _invertY;                   // y축 회전 방향 반전

  private float _rotationX;
  private float _rotationY;

  private float _invertXVal;
  private float _invertYVal;
  
  // Property
  public Quaternion GetPlanarRotation => Quaternion.Euler(0, _rotationY, 0);

  private void Start()
  {
    Cursor.visible = false;
    Cursor.lockState = CursorLockMode.Locked;
  }

  private void Update()
  {
    _invertXVal = (_invertX) ? -1 : 1;
    _invertYVal = (_invertY) ? -1 : 1;
    
    _rotationX += Input.GetAxis("Camera Y") * _rotationSpeed * _invertXVal;
    _rotationX = Mathf.Clamp(_rotationX, _minVerticalAngle, _maxVerticalAngle);
    
    _rotationY += Input.GetAxis("Camera X") * _rotationSpeed * _invertYVal;

    var targetRotation = Quaternion.Euler(_rotationX, _rotationY, 0);

    var focusPosition = _followTarget.position + new Vector3(_framingOffset.x, _framingOffset.y);
    
    transform.position = focusPosition - targetRotation * new Vector3(0, 0, _distance);
    transform.rotation = targetRotation;
    
    //zoom
    _distance += Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;
    _distance = Mathf.Clamp(_distance, _minZoomDistance, _maxZoomDistance);
  }
}
