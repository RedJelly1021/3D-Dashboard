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

    [Header("Panning")]
    public float panSpeed = 0.02f; // per pixel panning distance
    public float dragThreshold = 5f; // minimum drag distance to start panning
    public bool IsDragging { get; private set; }

    private Vector3 _panOffset = Vector3.zero;
    private Vector3 _lastPanMouse, _panStartMouse;
    private bool _panArmed; // start left click outside UI?

    void Awake() => Instance = this;

    private void Start()
    {
        _currentTarget = defaultTarget;
        _distance = defaultDistance;
    }

    private void LateUpdate()
    {
        bool isOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        HandlePanInput(isOverUI);

        if(!isOverUI)
        {
            HandleRotationInput();
            HandleZoomInput();
        }

        HandleEscInput();

        if (_currentTarget == null) return;

        Vector3 pivot = _currentTarget.position + _panOffset;
        Quaternion rot = Quaternion.Euler(_pitch, _yaw, 0);

        transform.position = pivot - rot * Vector3.forward * _distance;
        transform.LookAt(pivot);
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

    void HandlePanInput(bool isOverUI)
    {
        if (Input.GetMouseButtonDown(0))
        {
            _panArmed = !isOverUI; // only arm if not over UI
            IsDragging = false; // reset dragging state
            _panStartMouse = _lastPanMouse = Input.mousePosition;
        }

        if (!_panArmed) return;

        if (Input.GetMouseButton(0))
        {
            if (!IsDragging && (Input.mousePosition - _panStartMouse).magnitude > dragThreshold)
                IsDragging = true;

            if (IsDragging)
            {
                Vector3 delta = Input.mousePosition - _lastPanMouse;

                // move based on floor(xz) plane : camera's right + forward horizontal direction
                Vector3 right = transform.right;
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

                // move more when the zoom is far away, proportional to distance
                float scale = panSpeed * (_distance / defaultDistance); 
                _panOffset -= (right * delta.x + forward * delta.y) * scale;
            }
            _lastPanMouse = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _panArmed = false;
        }
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
        Vector3 startPos = (_currentTarget != null ? _currentTarget.position : newTarget.position) + _panOffset;
        _panOffset = Vector3.zero; // reset pan offset during transition
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
