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
    private float _lookSensitivity = 1.0f;
    private float _lookAcceleration = 0.1f;

    // Motion
    [SerializeField]
    private Rigidbody _rb;

    private float _walkSpeed = 10.0f;
    private float _sprintSpeed = 20.0f;
    private float _transitionAcceleration = 0.1f;
    private float _targetVelocity = 0.0f;

    private Vector3 _moveDirection = Vector3.zero;

    private bool _isJumping = false;
    private bool _isGrounded = false;

    void Start()
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
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 _newMoveDirection = forward * _moveDirection.z + right * _moveDirection.x;

        // Move the player
        _rb.velocity = Vector3.Lerp(
            _rb.velocity,
            _newMoveDirection.normalized * _targetVelocity,
            _transitionAcceleration
        );

        // If using a controller, rotate the player based on the right stick input
        if (_controlScheme == "Controller")
        {
            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y + _lookInput.x * _lookSensitivity,
                transform.rotation.eulerAngles.z
            );

            // Rotate the camera based on the right stick input
            _camera.transform.localRotation = Quaternion.Euler(
                Mathf.Clamp(_camera.transform.localRotation.eulerAngles.x - _lookInput.y * _lookSensitivity, -90.0f, 90.0f),
                _camera.transform.localRotation.eulerAngles.y,
                _camera.transform.localRotation.eulerAngles.z
            );
        }
    }

    void Update()
    {
        // Move the mirrored player
        _mirroredPlayer.transform.position = new Vector3(
            transform.position.x,
            -transform.position.y,
            transform.position.z
        );

        // Rotate the mirrored player
        _mirroredPlayer.transform.rotation = transform.rotation;
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
            _isJumping = true;
        if (context.phase == InputActionPhase.Canceled)
            _isJumping = false;
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

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Debug.Log("Fire");
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Debug.Log("Melee");
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && _moveDirection.z > 0.5f)
            _targetVelocity = _sprintSpeed;
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Debug.Log("Switch");
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Debug.Log("Reload");
    }

    public void OnControlsChanged(PlayerInput input)
    {
        _controlScheme = input.currentControlScheme;
    }
}
