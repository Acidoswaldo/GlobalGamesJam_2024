using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance;
    PlayerInputs _inputs;

    public static bool Interact;
    public static bool Slap;
    public static bool Pause;
    public static Vector2 MoveDirection;
    [SerializeField] Vector2 _moveDirection;


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != null) { Destroy(gameObject); }
        _inputs = new PlayerInputs();
    }
    private void Update()
    {
        _moveDirection = MoveDirection;
    }
    private void OnEnable()
    {
        Debug.Log("enabling inputs");
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
        Debug.Log("Disabling inputs");
    }

    #region Interact
    private void OnInteractPerformed(InputAction.CallbackContext context) { Debug.Log("Interact performed"); Interact = true; }
    private void OnInteractCanceled(InputAction.CallbackContext context) { Interact = false; }
    #endregion Interact
    #region Slap
    private void OnSlapCanceled(InputAction.CallbackContext context) { Slap = false; }

    private void OnSlapPerformed(InputAction.CallbackContext context) { Debug.Log("Slap performed"); Slap = true; }
    #endregion Slap
    #region Movement
    private void OnMovementCanceled(InputAction.CallbackContext context) { MoveDirection = Vector2.zero; }
    private void OnMovementPerformed(InputAction.CallbackContext context) { Debug.Log("move performed"); MoveDirection = context.ReadValue<Vector2>(); }
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
