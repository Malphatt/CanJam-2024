using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    private string _controlScheme;

    public GameObject NormalPlayer;
    [SerializeField]
    private GameObject _mirroredPlayer;
    [SerializeField]
    private PlayerCamera _playerCamera;

    private GameObject _camera;
    private GameObject _weapons;
    private Transform _muzzlePoint;

    private Vector3 _tempMuzzleStart = Vector3.zero;
    private Vector3 _tempMuzzleEnd = Vector3.zero;

    // Look
    // Controller
    private Vector2 _lookInput = Vector2.zero;

    // Mouse
    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    private readonly float _mouseSensitivity = 0.05f;
    private readonly float _lookSensitivity = 5.0f;

    // Motion
    private Rigidbody _rb;
    private Vector3 _velocity = Vector3.zero;

    private float _gravity = -120.0f;

    private readonly float _movementSmoothing = 0.05f;

    private float _walkSpeed = 25.0f;
    private float _sprintSpeed = 35.0f;
    private readonly float _transitionAcceleration = 0.2f;
    private float _targetVelocity = 0.0f;

    private Vector3 _moveDirection = Vector3.zero;

    private bool _isSprinting = false;

    // Jump
    private float _jumpForce = 35.0f;
    private bool _jumpedThisFrame = false;
    private bool _isJumping = false;

    // Jump Buffer
    private float _timeSinceJump = 0.0f;
    private readonly float _jumpBufferTime = 0.2f;
    private bool _jumpBuffered = false;
    private bool _heldJump = false;
    private float _timeSinceJumpBuffered = 0.0f;
    private readonly float _jumpBufferCooldown = 0.4f;

    // Ground Check
    private bool _isGrounded = false;

    [SerializeField]
    private LayerMask _groundLayer;

    private int _switchState = 1;

    // Weapons
    private bool _isFiring = false;
    private readonly float _fireRate = 0.1f;
    private float _timeSinceFired = 0.0f;

    private readonly float _rangedRange = 100.0f;
    private readonly float _meleeRange = 2.0f;
    private readonly float _meleeCooldown = 0.8f;
    private float _timeSinceMelee = 0.0f;

    [SerializeField]
    private LayerMask _enemyLayer;

    // Scripted Animation
    private bool _isSwitching = false;
    [SerializeField]
    private Image _BlackScreen;

    // Health
    public float CurrentHealth;
    private float _maxHealth = 100.0f;

    // Ultimate
    public int UltimateCharge = 0;
    private int _maxUltimateCharge = 10;

    // Animation
    private Animator _animator;
    private Animator _animator2;

    private AudioController _audioController;
    private BackgroundMusic _backgroundMusic;

    private HealthBar _healthBar;
    private UltBar _ultBar;

    //Trail
    [SerializeField]
    TrailRenderer BulletTrail;

    void Awake()
    {
        _camera = _playerCamera.Camera;
        _weapons = _playerCamera.Weapons;
        _muzzlePoint = _playerCamera.MuzzlePoint.transform;

        _animator = _playerCamera.Gun;
        _animator2 = _playerCamera.JamJar;

        _audioController = _playerCamera.AudioControl;
        _backgroundMusic = _playerCamera.BgMusic;

        _healthBar = _playerCamera.HealthBar;
        _ultBar = _playerCamera.UltBar;

        CurrentHealth = _maxHealth;

        _healthBar?.setMaxHealth(_maxHealth);
        _ultBar?.setMaxUlt(_maxUltimateCharge);

        _rb = NormalPlayer.GetComponent<Rigidbody>();

        _targetVelocity = _walkSpeed;

        rotationX = NormalPlayer.transform.rotation.eulerAngles.y;
        rotationY = _camera.transform.rotation.eulerAngles.x;

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set the black screen to be transparent
        _BlackScreen.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    void FixedUpdate()
    {
        // Move
        Move();

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

        // Jump
        if (_jumpedThisFrame)
            _jumpedThisFrame = false;

        // If the player has not jumped within the buffer time, reset the buffer
        if (Time.time - _timeSinceJump > _jumpBufferTime)
            _jumpBuffered = false;

        // Apply Gravity
        if (_rb.velocity.y > 0.0f)
            _rb.velocity += Vector3.up * (_gravity / 4) * Time.deltaTime;
        else
            _rb.velocity += Vector3.up * _gravity * Time.deltaTime;

        bool wasGrounded = _isGrounded; 
        _isGrounded = Physics.Raycast(NormalPlayer.transform.position, Vector3.down, 1.5f, _groundLayer);

        if (!wasGrounded && _isGrounded)
        {
            _audioController.Landed();
        }
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
            NormalPlayer.transform.rotation = Quaternion.Euler(0f, rotationX, 0f);
            // Rotate the camera based on the right stick input
            _camera.transform.localRotation = Quaternion.Euler(-rotationY, 0f, 0f);
        }
    }

    
    void Update()
    {
        _animator2.SetBool("isMelee", false);
        // Draw Melee & Ranged Ray
        Debug.DrawRay(NormalPlayer.transform.position, NormalPlayer.transform.forward * _meleeRange, Color.red);

        Debug.DrawRay(
            new Vector3(NormalPlayer.transform.position.x, NormalPlayer.transform.position.y + 0.5f, NormalPlayer.transform.position.z),
            new Vector3(NormalPlayer.transform.forward.x, _switchState * _camera.transform.forward.y, NormalPlayer.transform.forward.z)
                * _rangedRange,
            Color.blue
        );

        Debug.DrawRay(_tempMuzzleStart, _tempMuzzleEnd - _tempMuzzleStart, Color.yellow);

        // Move the mirrored player
        _mirroredPlayer.transform.position = new Vector3(
            NormalPlayer.transform.position.x,
            -NormalPlayer.transform.position.y,
            NormalPlayer.transform.position.z
        );

        // Rotate the mirrored player
        _mirroredPlayer.transform.rotation = Quaternion.Euler(
            NormalPlayer.transform.rotation.eulerAngles.x,
            NormalPlayer.transform.rotation.eulerAngles.y,
            NormalPlayer.transform.rotation.eulerAngles.z + 180.0f
        );

        // Move the camera
        if (!_isSwitching)
            _camera.transform.position = new Vector3(
                NormalPlayer.transform.position.x,
                _switchState * (NormalPlayer.transform.position.y + 0.5f),
                NormalPlayer.transform.position.z
            );

        // Rotate the camera's y-axis based on the player's y-axis
        _camera.transform.rotation = Quaternion.Euler(
            _camera.transform.rotation.eulerAngles.x,
            NormalPlayer.transform.rotation.eulerAngles.y,
            _camera.transform.rotation.eulerAngles.z
        );
        
    }

    public float TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        UpdateHealth();

        if (CurrentHealth <= 0.0f)
            Destroy(gameObject);

        return Mathf.Clamp(CurrentHealth, 0.0f, _maxHealth);
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

        Vector3 forward = NormalPlayer.transform.TransformDirection(Vector3.forward);
        Vector3 right = NormalPlayer.transform.TransformDirection(Vector3.right);
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
        _rb.AddForce(new Vector3(0, _jumpForce, 0), ForceMode.Impulse);
    }

    private void UseWeapon()
    {
        if (_isFiring && Time.time - _timeSinceFired > _fireRate)
        {
            _timeSinceFired = Time.time;
            Attack("Gun");
        }
        else if (!_isFiring)
            _animator.SetBool("Firing", false);
    }

    private void Attack(string weapon)
    {
        if (weapon == "Gun")
        {
            _audioController.FireMain();
            // Make a raycast from the camera's position to the camera's forward direction
            Ray ray = new Ray(
                new Vector3(
                    NormalPlayer.transform.position.x,
                    NormalPlayer.transform.position.y + 0.5f,
                    NormalPlayer.transform.position.z
                ),
                new Vector3(NormalPlayer.transform.forward.x, _camera.transform.forward.y, NormalPlayer.transform.forward.z)
            );
            RaycastHit hit;

            // If the raycast hits something
            if (Physics.Raycast(ray, out hit, _rangedRange))
            {
                _tempMuzzleStart = new Vector3(
                    _muzzlePoint.transform.position.x,
                    _muzzlePoint.transform.position.y,
                    _muzzlePoint.transform.position.z
                );
                TrailRenderer trail = Instantiate(BulletTrail, _tempMuzzleStart, Quaternion.identity);
                _tempMuzzleEnd = new Vector3(
                    hit.point.x,
                    hit.point.y,
                    hit.point.z
                );

                _animator.SetBool("Firing", true);

                // If the object hit has an Enemy component
                if (
                    hit.collider != null &&
                    hit.collider.GetComponent<Enemy>()
                    || hit.collider.transform.parent.GetComponent<Enemy>()
                )
                {
                    float enemyHealthRemaining;


                    // Call the TakeDamage function on the Enemy component
                    enemyHealthRemaining = (float) hit.collider.GetComponent<Enemy>()?.TakeDamage(10.0f);
                    enemyHealthRemaining = (float) hit.collider.transform.parent.GetComponent<Enemy>()?.TakeDamage(10.0f);

                    if (enemyHealthRemaining <= 0.0f)
                    {
                        
                        UltimateCharge = Mathf.Clamp(UltimateCharge + 1, 0, _maxUltimateCharge);
                        UpdateUltimate();
                        _audioController.Kill();
                    }
                }
            }
        }
        else if (weapon == "Melee")
        {

            _audioController.BeatIt();

            // Make a raycast from the player's position to the player's forward direction
            Ray ray = new Ray(NormalPlayer.transform.position, NormalPlayer.transform.forward);
            RaycastHit hit;

            _animator2.SetTrigger("Melee");
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
        if (_isSwitching) return;

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
            NormalPlayer.transform.rotation = Quaternion.Euler(0f, rotationX, 0f);
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

        if (context.phase == InputActionPhase.Started && !_isSwitching && _isGrounded && UltimateCharge == _maxUltimateCharge)
            StartCoroutine(StartSwitchAnimation());
        else if (context.phase == InputActionPhase.Started)
        {
            // Switch anim if not ready

        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
            Debug.Log("Reload");
    }

    public void OnControlsChanged(PlayerInput input) => _controlScheme = input.currentControlScheme;

    IEnumerator StartSwitchAnimation()
    {
        _audioController.FlipToMirror();
        _backgroundMusic.MainMusic();
        _animator.SetBool("Swap", _switchState == 1);
        _animator2.SetBool("Flipped", _switchState == 1);
        _isSwitching = true;

        float offset = 0.0f;

        // Shift the camera's y-axis gradually to the ground (NormalPlayer's position.y - 0.5f)
        while (offset < 1.0f)
        {
            offset += 0.025f;

            _camera.transform.position = new Vector3(
                NormalPlayer.transform.position.x,
                _switchState * (NormalPlayer.transform.position.y + 0.5f - offset),
                NormalPlayer.transform.position.z
            );

            // also squash the _weapons
            _weapons.transform.localScale = new Vector3(
                _weapons.transform.localScale.x,
                1.0f * _switchState - offset,
                _weapons.transform.localScale.z
            );

            _BlackScreen.color = new Color(
                0.0f,
                0.0f,
                0.0f,
                Mathf.Clamp(offset, 0, 1.0f)
            );

            yield return new WaitForSeconds(0.01f);
        }

        offset = 1.0f;

        // Teleport the camera to the ground (-NormalPlayer's position.y + 0.5f)
        //_camera.transform.position = new Vector3(
        //    NormalPlayer.transform.position.x,
        //    _switchState * (-NormalPlayer.transform.position.y + 0.5f),
        //    NormalPlayer.transform.position.z
        //);
        _weapons.transform.localScale = new Vector3(
            _weapons.transform.localScale.x,
            0.0f,
            _weapons.transform.localScale.z
        );

        _BlackScreen.color = new Color(
            0.0f,
            0.0f,
            0.0f,
            offset
        );

        // Animate the camera coming back up also
        while (offset > 0.0f)
        {
            offset -= 0.025f;

            _camera.transform.position = new Vector3(
                NormalPlayer.transform.position.x,
                _switchState * (-NormalPlayer.transform.position.y - 0.5f + offset),
                NormalPlayer.transform.position.z
            );

            _weapons.transform.localScale = new Vector3(
                _weapons.transform.localScale.x,
                1.0f * _switchState - offset,
                _weapons.transform.localScale.z
            );

            _BlackScreen.color = new Color(
                0.0f,
                0.0f,
                0.0f,
                Mathf.Clamp(offset, 0, 1.0f)
            );

            yield return new WaitForSeconds(0.01f);
        }

        // Switch the player's state
        _switchState *= -1;
        _isSwitching = false;

        _weapons.transform.localScale = new Vector3(
            _weapons.transform.localScale.x,
            1.0f * _switchState,
            _weapons.transform.localScale.z
        );

        _BlackScreen.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    public void UpdateHealth() => _healthBar?.SetHealth(CurrentHealth);
    private void UpdateUltimate() => _ultBar?.SetUlt(UltimateCharge);
}
