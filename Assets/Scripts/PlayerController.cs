using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private string _controlScheme;

    [SerializeField]
    private GameObject _mirroredPlayer;

    [SerializeField]
    private GameObject _camera;

    // Look
    // Controller
    private Vector2 _lookInput = Vector2.zero;

    // Mouse
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private float _mouseSensitivity = 0.05f;
    private float _lookSensitivity = 5.0f;

    // Motion
    private Rigidbody _rb;
    private Vector3 _velocity = Vector3.zero;

    private float _gravity = -40.0f;

    private float _movementSmoothing = 0.05f;

    private float _walkSpeed = 10.0f;
    private float _sprintSpeed = 20.0f;
    private float _transitionAcceleration = 0.1f;
    private float _targetVelocity = 0.0f;

    private Vector3 _moveDirection = Vector3.zero;

    private bool _isSprinting = false;

    // Jump
    private float _jumpForce = 25.0f;
    private bool _jumpedThisFrame = false;
    private bool _isJumping = false;

    // Jump Buffer
    private float _timeSinceJump = 0.0f;
    private float _jumpBufferTime = 0.2f;
    private bool _jumpBuffered = false;
    private bool _heldJump = false;
    private float _timeSinceJumpBuffered = 0.0f;
    private float _jumpBufferCooldown = 0.4f;

    // Ground Check
    private bool _isGrounded = false;

    [SerializeField]
    private LayerMask _groundLayer;

    private int _switchState = 1;

    // Weapons
    private bool _isFiring = false;
    private float _fireRate = 0.1f;
    private float _timeSinceFired = 0.0f;

    private float _rangedRange = 100.0f;
    private float _meleeRange = 2.0f;
    private float _meleeCooldown = 0.8f;
    private float _timeSinceMelee = 0.0f;

    [SerializeField]
    private LayerMask _enemyLayer;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _targetVelocity = _walkSpeed;

        rotationX = transform.rotation.eulerAngles.y;
        rotationY = _camera.transform.rotation.eulerAngles.x;

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        // Move
        Move();

        // Jump
        if (_jumpedThisFrame)
            _jumpedThisFrame = false;

        bool wasGrounded = _isGrounded;
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, _groundLayer);

        // Sprint
        if (_isSprinting && _moveDirection.z > 0.5f)
            _targetVelocity = _sprintSpeed;

        // Weapons
        UseWeapon();

        // If using a controller, rotate the player based on the right stick input
        if (_controlScheme == "Controller")
        {
            rotationX += _lookInput.x * _lookSensitivity;
            rotationY += _lookInput.y * _lookSensitivity;
            rotationY = Mathf.Clamp(rotationY, -90.0f, 90.0f);

            // Rotate the player based on the right stick input
            transform.rotation = Quaternion.Euler(0f, rotationX, 0f);
            // Rotate the camera based on the right stick input
            _camera.transform.localRotation = Quaternion.Euler(-rotationY, 0f, 0f);
        }
    }

    void Update()
    {
        // Draw Melee & Ranged Ray
        Debug.DrawRay(transform.position, transform.forward * _meleeRange, Color.red);

        Debug.DrawRay(
            new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z),
            new Vector3(transform.forward.x, _camera.transform.forward.y, transform.forward.z)
                * _rangedRange,
            Color.blue
        );

        // If the jump key stops being held and the player is currently moving upwards, stop the jump
        if (_heldJump && _rb.velocity.y > 0)
        {
            Vector3 targetVelocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            Vector3.SmoothDamp(_rb.velocity, targetVelocity, ref _velocity, _movementSmoothing);
        }

        // If the player has jumped within the buffer time, jump again
        if (
            _isGrounded
            && !_jumpedThisFrame
            && _jumpBuffered
            && Time.time - _timeSinceJump < _jumpBufferTime
            && Time.time - _timeSinceJumpBuffered > _jumpBufferCooldown
        )
        {
            _timeSinceJumpBuffered = Time.time;
            Jump();
        }

        // If the player has not jumped within the buffer time, reset the buffer
        if (Time.time - _timeSinceJump > _jumpBufferTime)
            _jumpBuffered = false;

        // Apply Gravity
        _rb.AddForce(Vector3.up * _gravity, ForceMode.Acceleration);

        // Move the mirrored player
        _mirroredPlayer.transform.position = new Vector3(
            transform.position.x,
            -transform.position.y,
            transform.position.z
        );

        // Rotate the mirrored player
        _mirroredPlayer.transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            transform.rotation.eulerAngles.z + 180.0f
        );

        // Move the camera
        _camera.transform.position = new Vector3(
            transform.position.x,
            _switchState * (transform.position.y + 0.5f),
            transform.position.z
        );

        // Rotate the camera's y-axis based on the player's y-axis
        _camera.transform.rotation = Quaternion.Euler(
            _camera.transform.rotation.eulerAngles.x,
            transform.rotation.eulerAngles.y,
            _camera.transform.rotation.eulerAngles.z
        );
    }

    private void Move()
    {
        if (_jumpedThisFrame)
        {
            _timeSinceJump = Time.time;
            _jumpBuffered = true;
        }

        if (_heldJump && _isJumping)
            _heldJump = false;

        if (_isGrounded && _jumpedThisFrame && !_jumpBuffered)
            Jump();

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 _newMoveDirection = forward * _moveDirection.z + right * _moveDirection.x;

        // Move the player
        _rb.velocity = Vector3.Lerp(
            _rb.velocity,
            _newMoveDirection.normalized * _targetVelocity,
            _transitionAcceleration
        );
    }

    private void Jump()
    {
        // Set vertical velocity to 0 before jumping
        _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        // Add a vertical force to the player
        _isGrounded = false;
        _jumpBuffered = false;
        _heldJump = true;
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
    }

    private void UseWeapon()
    {
        if (_isFiring && Time.time - _timeSinceFired > _fireRate)
        {
            _timeSinceFired = Time.time;
            Attack("Gun");
        }
    }

    private void Attack(string weapon)
    {
        if (weapon == "Gun")
        {
            // Make a raycast from the camera's position to the camera's forward direction
            Ray ray = new Ray(
                new Vector3(
                    transform.position.x,
                    transform.position.y + 0.5f,
                    transform.position.z
                ),
                new Vector3(transform.forward.x, _camera.transform.forward.y, transform.forward.z)
            );
            RaycastHit hit;

            // If the raycast hits something
            if (Physics.Raycast(ray, out hit, _rangedRange, _enemyLayer))
            {
                // If the object hit has an Enemy component
                if (
                    hit.collider.GetComponent<Enemy>()
                    || hit.collider.transform.parent.GetComponent<Enemy>()
                )
                {
                    // Call the TakeDamage function on the Enemy component
                    hit.collider.GetComponent<Enemy>()?.TakeDamage(10.0f);
                    hit.collider.transform.parent.GetComponent<Enemy>()?.TakeDamage(10.0f);
                }
            }
        }
        else if (weapon == "Melee")
        {
            // Make a raycast from the player's position to the player's forward direction
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            // If the raycast hits something
            if (Physics.Raycast(ray, out hit, _meleeRange, _enemyLayer))
            {
                // If the object hit has an Enemy component
                if (hit.collider.GetComponent<Enemy>() || hit.collider.transform.parent.GetComponent<Enemy>())
                {
                    // Call the TakeDamage function on the Enemy component
                    hit.collider.GetComponent<Enemy>()?.TakeDamage(50.0f);
                    hit.collider.transform.parent.GetComponent<Enemy>()?.TakeDamage(50.0f);
                }
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        _moveDirection = new Vector3(input.x, 0, input.y);

        if (_moveDirection.z <= 0.5f)
            _targetVelocity = _walkSpeed;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            _jumpedThisFrame = true;

        _isJumping = context.ReadValueAsButton();
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (_controlScheme == "Controller")
            _lookInput = context.ReadValue<Vector2>();

        if (_controlScheme == "Keyboard & Mouse")
        {
            // Rotate the player based on the mouse input
            rotationX += context.ReadValue<Vector2>().x * _mouseSensitivity;
            rotationY += context.ReadValue<Vector2>().y * _mouseSensitivity;
            rotationY = Mathf.Clamp(rotationY, -90.0f, 90.0f);

            // Rotate the player based on the mouse input
            transform.rotation = Quaternion.Euler(0f, rotationX, 0f);
            // Rotate the camera based on the mouse input
            _camera.transform.localRotation = Quaternion.Euler(-rotationY, 0f, 0f);
        }
    }

    public void OnFire(InputAction.CallbackContext context) =>
        _isFiring = context.ReadValueAsButton();

    public void OnMelee(InputAction.CallbackContext context)
    {
        if (
            context.phase == InputActionPhase.Started
            && !_isFiring
            && Time.time - _timeSinceMelee > _meleeCooldown
        )
        {
            _timeSinceMelee = Time.time;

            Attack("Melee");
        }
    }

    public void OnSprint(InputAction.CallbackContext context) =>
        _isSprinting = context.ReadValueAsButton();

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            _switchState *= -1;
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Debug.Log("Reload");
    }

    public void OnControlsChanged(PlayerInput input) => _controlScheme = input.currentControlScheme;
}
