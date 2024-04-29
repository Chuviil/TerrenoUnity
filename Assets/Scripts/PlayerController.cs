using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidBody;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float movementSpeed = 5f;
    private PlayerInputActions _playerInputActions;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Jump.performed += JumpOnPerformed;
    }

    private void FixedUpdate()
    {
        Vector2 inputVector = _playerInputActions.Player.Movement.ReadValue<Vector2>();
        _rigidBody.AddForce(new Vector3(inputVector.x, 0, inputVector.y) * movementSpeed, ForceMode.Force);
    }

    private void JumpOnPerformed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}