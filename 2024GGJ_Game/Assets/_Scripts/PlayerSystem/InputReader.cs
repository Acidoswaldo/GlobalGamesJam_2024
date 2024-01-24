using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance;
    InputActions _inputs;
    [SerializeField] Camera _mainCamera;
    PlayerController _player;

    public static bool Interact;
    public static bool Slap;
    public static bool Pause;
    public static Vector2 MoveDirection;
    [SerializeField] Vector2 _moveDirection;


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != null) { Destroy(gameObject); }
        _inputs = new InputActions();
        _player = FindObjectOfType<PlayerController>();

    }

    private void Start()
    {
        if (_mainCamera == null) _mainCamera = Camera.main;
    }

    private void Update()
    {
        _moveDirection = MoveDirection;
    }
    private void OnEnable()
    {
        _inputs.Enable();
        _inputs.Player.Movement.performed += OnMovementPerformed;
        _inputs.Player.Movement.canceled += OnMovementCanceled;

        _inputs.Player.Slap.performed += OnSlapPerformed;
        _inputs.Player.Slap.canceled += OnSlapCanceled;

        _inputs.Player.Interact.performed += OnInteractPerformed;
        _inputs.Player.Interact.canceled += OnInteractCanceled;

        _inputs.Player.Pause.performed += OnPausePerformed;
        _inputs.Player.Pause.canceled += OnPauseCanceled;
    }

    private void OnDisable()
    {
        _inputs.Player.Movement.performed -= OnMovementPerformed;
        _inputs.Player.Movement.canceled -= OnMovementCanceled;

        _inputs.Player.Slap.performed -= OnSlapPerformed;
        _inputs.Player.Slap.canceled -= OnSlapCanceled;

        _inputs.Player.Interact.performed -= OnInteractPerformed;
        _inputs.Player.Interact.canceled -= OnInteractCanceled;

        _inputs.Player.Pause.performed -= OnPausePerformed;
        _inputs.Player.Pause.canceled -= OnPauseCanceled;
        _inputs.Disable();
    }

    #region Interact
    private void OnInteractPerformed(InputAction.CallbackContext context) { Interact = true; }
    private void OnInteractCanceled(InputAction.CallbackContext context) { Interact = false; }
    #endregion Interact

    #region Slap
    private void OnSlapCanceled(InputAction.CallbackContext context) { Slap = false; }

    private void OnSlapPerformed(InputAction.CallbackContext context) { Slap = true; }
    #endregion Slap

    #region Movement
    private void OnMovementCanceled(InputAction.CallbackContext context) { MoveDirection = Vector2.zero; }
    private void OnMovementPerformed(InputAction.CallbackContext context) { MoveDirection = context.ReadValue<Vector2>(); }
    #endregion Movement

    #region Pause
    private void OnPauseCanceled(InputAction.CallbackContext context)
    {

    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        Pause = true;
    }
    #endregion Pause

}
