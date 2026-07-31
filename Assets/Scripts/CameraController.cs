using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Camera Settings")]
    public Transform defaultTarget; // Center (home view point)
    public float rotateSpeed = 0.1f; // per pixel rotation angle (sensitivity)
    public float zoomSpeed = 10f;
    public float defaultDistance = 30f;
    public float focusDistance = 8f;
    public float transitionDuration = 1f;

    private Transform _currentTarget;
    private float _distance;
    private float _yaw = 0f, _pitch = 30f;
    private Vector3 _lastMousePosition;
    private Coroutine _transitionRoutine;
    private GameObject _tempTargetObj;

    void Awake() => Instance = this;

    private void Start()
    {
        _currentTarget = defaultTarget;
        _distance = defaultDistance;
    }

    private void LateUpdate()
    {
        bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if(!isOverUI)
        {
            HandleRotationInput();
            HandleZoomInput();
        }

        HandleEscInput();

        if (_currentTarget == null) return;

        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0);
        Vector3 pos = _currentTarget.position - rot * Vector3.forward * _distance;

        transform.position = pos;
        transform.LookAt(_currentTarget);
    }

    void HandleRotationInput()
    {
        if (Input.GetMouseButtonDown(1)) // right click drag rotates
        {
            _lastMousePosition = Input.mousePosition; // right click start
        }
        if(Input.GetMouseButton(1))
        { 
            Vector3 delta = Input.mousePosition - _lastMousePosition;

            _yaw += delta.x * rotateSpeed;
            _pitch -= delta.y * rotateSpeed;
            _pitch = Mathf.Clamp(_pitch, 5f, 80f);

            _lastMousePosition = Input.mousePosition; // refresh for the next frame comparation
        }
    }

    void HandleZoomInput()
    {
        _distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        _distance = Mathf.Clamp(_distance, 5f, 40f);
    }

    void HandleEscInput()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToHomeView();
        }
    }

    public void FocusOnZone(Transform zoneTransform)
    {
        StartTransition(zoneTransform, focusDistance);
    }

    public void ReturnToHomeView()
    {
        StartTransition(defaultTarget, defaultDistance);
        DetailPanelUI.Instance.ClosePanel();
    }

    private void StartTransition(Transform newTarget, float newDistance)
    {
        if (_transitionRoutine != null)
            StopCoroutine(_transitionRoutine);

        // if any remaining temp object from previous transition, destroy it
        if (_tempTargetObj != null)
        {
            Destroy(_tempTargetObj);
            _tempTargetObj = null;
        }

        _transitionRoutine = StartCoroutine(TransitionTo(newTarget, newDistance));
    }

    private IEnumerator TransitionTo(Transform newTarget, float newDistance)
    {
        float elapsed = 0f;
        Vector3 startPos = _currentTarget != null ? _currentTarget.position : newTarget.position;
        float startDistance = _distance;

        // during transition, create temporary target object to interpolate position
        _tempTargetObj = new GameObject("TempCameraTarget");
        _currentTarget = _tempTargetObj.transform;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            // position interpolation
            _tempTargetObj.transform.position = Vector3.Lerp(startPos, newTarget.position, t);
            // distance interpolation
            _distance = Mathf.Lerp(startDistance, newDistance, t);
            yield return null;
        }

        Destroy(_tempTargetObj);
        _tempTargetObj = null;
        _currentTarget = newTarget;
        _distance = newDistance;
    }
}
