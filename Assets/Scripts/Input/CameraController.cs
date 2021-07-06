using Input;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float speed;
    [SerializeField] private float screenBorderThickness;
    [SerializeField] private Vector2 screenLimitsMin;
    [SerializeField] private Vector2 screenLimitsMax;

    private Controls _controls;
    private Vector2 _previousInput;
    
    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);
        
        _controls = new Controls();

        _controls.Player.MoveCamera.performed += SetPreviousInput;
        _controls.Player.MoveCamera.canceled += SetPreviousInput;
        
        _controls.Enable();
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        _previousInput = ctx.ReadValue<Vector2>();
    }

    private void Update()
    {
        if (!hasAuthority) 
        {
            enabled = false;
            return;
        }

        if (!Application.isFocused)
        {
            return;
        }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        var pos = playerCameraTransform.position;

        if (_previousInput == Vector2.zero)
        {
            var cursorMovement = Vector3.zero;

            var cursorPosition = Mouse.current.position.ReadValue();

            if (cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            }else if (cursorPosition.y <= screenBorderThickness)
            {
                cursorMovement.z -= 1;
            }
            
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            pos += cursorMovement.normalized * (speed * Time.deltaTime);
        }
        else
        {
            pos += new Vector3(_previousInput.x, 0f, _previousInput.y) * (speed * Time.deltaTime);
        }

        pos.x = Mathf.Clamp(pos.x, screenLimitsMin.x, screenLimitsMax.x);
        pos.z = Mathf.Clamp(pos.z, screenLimitsMin.y, screenLimitsMax.y);

        playerCameraTransform.position = pos;
    }
}
