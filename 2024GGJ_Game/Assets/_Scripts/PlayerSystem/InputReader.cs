using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    private PlayerInput playerInput;
    bool Interact;
    bool Slap;
    bool Pause;
    Vector2 MoveDirection;
    [SerializeField] PlayerController controller;
    [SerializeField] PlayerController otherController;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        var players = GameManager.Instance.GetPlayers();
        var index = playerInput.playerIndex;
        if(index == 0) { GameManager.Instance.SetDeviceIndex(index); otherController = players[index + 1]; }
        controller = players[index];
    }
    #region Interact
    public void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (controller == null) return;
        controller.Interact(context);
        Debug.Log("Interact performed");
    }
    #endregion Interact
    #region Slap
    public void OnSlapPerformed(InputAction.CallbackContext context)
    {
        if (controller == null) return;
        controller.Slap(context);
        Debug.Log("Slap performed"); Slap = true;
    }
    #endregion Slap
    #region Movement
    public void OnMovementAction(InputAction.CallbackContext context)
    {
        if (controller == null) return;
        controller.Move(context);
        Debug.Log("move performed"); MoveDirection = context.ReadValue<Vector2>();
    }
    #endregion Movement
    #region Pause
    public void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (controller == null) return;
        controller.Pause(context);
        Debug.Log("Pause performed"); Pause = true;
    }
    #endregion Pause


    #region alt Interact
    public void OnAltInteractPerformed(InputAction.CallbackContext context)
    {
        if (controller == null) return;
        if (GameManager.Instance.deviceIndex > 0 || otherController == null ) return;
        otherController.Interact(context);
        Debug.Log("Interact performed");
    }
    #endregion alt Interact
    #region alt Slap
    public void OnAltSlapPerformed(InputAction.CallbackContext context)
    {
        if (controller == null) return;
        if (GameManager.Instance.deviceIndex > 0 || otherController == null) return;
        otherController.Slap(context);
        Debug.Log("Slap performed"); Slap = true;
    }
    #endregion alt Slap
    #region alt Movement
    public void OnAltMovementAction(InputAction.CallbackContext context)
    {
        if (controller == null) return;
        if (GameManager.Instance.deviceIndex > 0 || otherController == null) return;
        otherController.Move(context);
        Debug.Log("move performed"); MoveDirection = context.ReadValue<Vector2>();
    }
    #endregion alt Movement
    #region alt Pause
    public void OnAltPausePerformed(InputAction.CallbackContext context)
    {
        if (controller == null) return;
        if (GameManager.Instance.deviceIndex > 0 || otherController == null) return;
        otherController.Pause(context);
        Debug.Log("Pause performed"); Pause = true;
    }
    #endregion alt Pause



}
