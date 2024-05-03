using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;

    private Vector2 _inputVector;
    private Vector3 _movementDirection;

    private Rigidbody _rb;
    private PlayerInputActions _playerInputActions;

    private bool _grounded;
    private bool _readyToJump = true;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Jump.performed += Jump;
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;
    }

    private void Update()
    {
        _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayer);

        GetInput();
        SpeedControl();

        if (_grounded)
            _rb.drag = groundDrag;
        else
            _rb.drag = 0f;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void GetInput()
    {
        _inputVector = _playerInputActions.Player.Movement.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        _movementDirection = orientation.forward * _inputVector.y + orientation.right * _inputVector.x;

        if (_grounded)
            _rb.AddForce(_movementDirection.normalized * (movementSpeed * 10f), ForceMode.Force);
        else if (!_grounded)
            _rb.AddForce(_movementDirection.normalized * (movementSpeed * 10f * airMultiplier), ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVelocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);

        if (flatVelocity.magnitude > movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
            _rb.velocity = new Vector3(limitedVelocity.x, _rb.velocity.y, limitedVelocity.z);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (_readyToJump && _grounded)
        {
            _readyToJump = false;
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        _readyToJump = true;
    }
}