using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Tooltip("최대 줌 아웃 크기")]
    public float zoomMax = 15;
    [Tooltip("최대 줌 인 크기")]
    public float zoomMin = 7;
    private float zoomTarget = 7.0f;
    public float smooth = 2.0f;
    private Vector3 offset;
    private Transform target;
    private Camera minimapCamera;
    private Player_Input_Actions uiActions;

    private void Awake()
    {
        minimapCamera = GetComponent<Camera>();
        uiActions = new Player_Input_Actions();
    }

    private void Start()
    {
        zoomTarget = zoomMin;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * smooth);
        transform.rotation = Quaternion.Euler(90, target.eulerAngles.y, 0);
        minimapCamera.orthographicSize = Mathf.Lerp(minimapCamera.orthographicSize, zoomTarget, Time.deltaTime);
    }

    private void OnEnable()
    {
        uiActions.UI.Enable();

        uiActions.UI.Minimap_ZoomIn.performed += OnZoomIn;
        uiActions.UI.Minimap_ZoomOut.performed += OnZoomOut;
    }

    private void OnDisable()
    {
        uiActions.UI.Minimap_ZoomIn.performed -= OnZoomIn;
        uiActions.UI.Minimap_ZoomOut.performed -= OnZoomOut;

        uiActions.UI.Disable();
    }

    public void Initialize(Player player)
    {
        // 플레이어가 0, 0, 0이어서 별다른 계산 안함
        offset = transform.position;
        target = player.transform;
        transform.position = target.position + offset;

        player.onSpawn += () =>
        {
            transform.position = target.position + offset;
            transform.rotation = Quaternion.Euler(90, target.eulerAngles.y, 0);
        };
    }

    private void OnZoomIn(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        zoomTarget -= 1.0f;
        zoomTarget = Mathf.Clamp(zoomTarget, zoomMin, zoomMax);
    }

    private void OnZoomOut(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        zoomTarget += 1.0f;
        zoomTarget = Mathf.Clamp(zoomTarget, zoomMin, zoomMax);
    }
}
