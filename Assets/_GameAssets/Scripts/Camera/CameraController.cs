using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using Utils.UI;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraController : MyBox.Singleton<CameraController>
{
    [SerializeField] private GameObject m_uiBackgroundImage;
    
    [Header("Pan Settings")]
    [SerializeField] private float panSpeed = 0.5f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float zoomSmoothness = 10f;
    [SerializeField] private float minCameraDistance = 5f;
    [SerializeField] private float maxCameraDistance = 50f;
    [SerializeField] private float m_defaultZooom = 20f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Boundary Settings")]
    [SerializeField] private float boundaryRadius = 100f;

    [TitleGroup("Anim Settings")]
    [SerializeField] private float m_resetAnimDuration = 1f;
    [SerializeField] private float m_resetAnimZoom = 100f;
    [SerializeField] private AnimationCurve m_resetAnimCurve;

    #region Variables
    private bool isControlling = true;

    private CinemachineCamera virtualCamera;
    private CinemachineComponentBase componentBase;
    private CinemachinePositionComposer composer;

    private Transform followTarget;
    private Vector3 lastMousePosition;
    private float targetCameraDistance;
    #endregion

    protected void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
        componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        composer = componentBase as CinemachinePositionComposer;

        if (virtualCamera.Follow == null)
            Debug.LogWarning("CameraController: CinemachineCamera has no Follow target assigned!");

        followTarget = virtualCamera.Follow;
        
        #if !UNITY_EDITOR
        zoomSpeed *= 2;
        rotationSpeed *= 5;
        panSpeed *= 4;
        #endif
        
        NewStartAnim();
    }

    private void Update()
    {
        if (!isControlling) return;
        HandlePanning();
        HandleZoom();
        HandleRotation();
    }

    public void SetControl(bool enable)
    {
        isControlling = enable;

        if (enable)
        {
            lastMousePosition = Input.mousePosition;
        }
    }

    private void HandlePanning()
    {
        if (followTarget == null) return;

        // Middle mouse pan
        if (Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
        
            Vector3 right = transform.right;
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

            Vector3 move = (-right * delta.x - forward * delta.y) * panSpeed * Time.deltaTime * PlayerPrefs.GetFloat("CameraSensitivity", 0.5f);
            Vector3 newPosition = followTarget.position + move;

            newPosition = ApplyBoundaryConstraint(newPosition);
            followTarget.position = newPosition;
            lastMousePosition = Input.mousePosition;
        }

        // WASD pan (physical key position, cross-layout)
        Vector3 right2 = transform.right;
        Vector3 forward2 = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        Vector3 keyboardInput = Vector3.zero;
        if (Keyboard.current[Key.W].isPressed) keyboardInput += forward2;
        if (Keyboard.current[Key.S].isPressed) keyboardInput -= forward2;
        if (Keyboard.current[Key.A].isPressed) keyboardInput -= right2;
        if (Keyboard.current[Key.D].isPressed) keyboardInput += right2;

        if (keyboardInput != Vector3.zero)
        {
            Vector3 keyboardMove = keyboardInput * panSpeed * 10f * Time.deltaTime * PlayerPrefs.GetFloat("CameraSensitivity", 0.5f);
            Vector3 newPosition = ApplyBoundaryConstraint(followTarget.position + keyboardMove);
            followTarget.position = newPosition;
        }
    }

    private Vector3 ApplyBoundaryConstraint(Vector3 position)
    {
        // Calculate distance from world center (ignoring Y axis for horizontal boundary)
        Vector3 flatPosition = new Vector3(position.x, 0f, position.z);
        Vector3 flatCenter = Vector3.zero;
        float distanceFromCenter = Vector3.Distance(flatPosition, flatCenter);

        // If within boundary, return as-is
        if (distanceFromCenter <= boundaryRadius)
        {
            return position;
        }

        // Hard clamp: constrain to boundary
        Vector3 directionFromCenter = (flatPosition - flatCenter).normalized;
        Vector3 clampedFlatPosition = flatCenter + directionFromCenter * boundaryRadius;
        
        // Preserve original Y position
        return new Vector3(clampedFlatPosition.x, position.y, clampedFlatPosition.z);
    }

    private void HandleZoom()
    {
        if (composer == null) return;

        if (UIHandler.Instance != null && UIHandler.Instance.IsOverUI) return;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            targetCameraDistance -= scroll * zoomSpeed;
            targetCameraDistance = Mathf.Clamp(targetCameraDistance, minCameraDistance, maxCameraDistance);
        }
        
        composer.CameraDistance = Mathf.Lerp(
            composer.CameraDistance, 
            targetCameraDistance, 
            zoomSmoothness * Time.deltaTime
        );
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1)) // Right mouse held
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            float rotationY = delta.x * rotationSpeed * Time.deltaTime * PlayerPrefs.GetFloat("CameraSensitivity", 0.5f);

            transform.Rotate(Vector3.up, rotationY, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }


    public void ResetAnim()
    {
        StartCoroutine(ResetAnimCR());
    }

    private IEnumerator ResetAnimCR()
    {
        float elapsed = 0f;
        float initialDistance = composer.CameraDistance;
        while (elapsed < m_resetAnimDuration)
        {
            float t = elapsed / m_resetAnimDuration;
            float curveValue = m_resetAnimCurve.Evaluate(t);
            
            composer.CameraDistance = Mathf.Lerp(initialDistance, m_resetAnimZoom, curveValue);
            targetCameraDistance = composer.CameraDistance; // Keep target in sync

            elapsed += Time.deltaTime;
            yield return null;
        }

        SetUIBackgroundImageActive(true);
    }
    
    public void SetUIBackgroundImageActive(bool active)
    {
        m_uiBackgroundImage.SetActive(active);
    }

    public void NewStartAnim()
    {
        virtualCamera.Target.TrackingTarget.position = Vector3.zero;
        StartCoroutine(NewStartAnimCR());
    }

    private IEnumerator NewStartAnimCR()
    {
        SetUIBackgroundImageActive(false);
        
        float elapsed = 0f;

        while (elapsed < m_resetAnimDuration)
        {
            float t = elapsed / m_resetAnimDuration;
            float curveValue = m_resetAnimCurve.Evaluate(t);

            composer.CameraDistance = Mathf.Lerp(m_resetAnimZoom, m_defaultZooom, curveValue);
            targetCameraDistance = composer.CameraDistance; // Keep target in sync

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // Public method for runtime boundary adjustment
    public void SetBoundaryRadius(float radius)
    {
        boundaryRadius = Mathf.Max(0f, radius);
    }

    // Visualization in Scene view
    private void OnDrawGizmosSelected()
    {
        // Draw boundary circle
        Gizmos.color = Color.yellow;
        DrawCircle(Vector3.zero, boundaryRadius, 64);

        // Draw current follow target position if available
        if (followTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(followTarget.position, 2f);
            
            // Draw line to center
            Gizmos.color = Color.cyan * 0.5f;
            Vector3 flatTarget = new Vector3(followTarget.position.x, 0f, followTarget.position.z);
            Gizmos.DrawLine(Vector3.zero, flatTarget);
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0f, 0f);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}