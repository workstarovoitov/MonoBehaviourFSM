using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour, IUnitController
{
    private Controls inputActions;
    private UnitMain uMain;

    private void OnEnable()
    {
        if (uMain != null && uMain.uState != null)
        {
            uMain.uState.OnMoveInputUpdate += UpdateMoveInput;
        }
        else
        {
            DisableController();
            enabled = false;
            return;
        }

        EnableController();
    }

    private void OnDisable()
    {
        DisableController();
        if (uMain != null && uMain.uState != null)
        {
            uMain.uState.OnMoveInputUpdate -= UpdateMoveInput;
        }
    }

    public void Initialize(UnitMain unitMain)
    {
        uMain = unitMain;
        uMain.uState.OnMoveInputUpdate += UpdateMoveInput;

        enabled = true;
    }

    /// <summary>
    /// Enables the input controller and subscribes to input events.
    /// </summary>
    public void EnableController()
    {
        if (inputActions == null)
        {
            inputActions = new Controls();
        }
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.Run.started += OnRun;
        inputActions.Player.Run.canceled += OnRunCanceled;
        inputActions.Player.Crouch.started += OnCrouch;
        inputActions.Player.Crouch.canceled += OnCrouchCanceled;
        inputActions.Player.Jump.started += OnJump;
        inputActions.Player.Jump.canceled += OnJumpCanceled;
    }

    /// <summary>
    /// Disables the input controller and unsubscribes from input events.
    /// </summary>
    public void DisableController()
    {
        if (inputActions == null) return;

        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.Run.started -= OnRun;
        inputActions.Player.Run.canceled -= OnRunCanceled;
        inputActions.Player.Crouch.started -= OnCrouch;
        inputActions.Player.Crouch.canceled -= OnCrouchCanceled;
        inputActions.Player.Jump.started -= OnJump;
        inputActions.Player.Jump.canceled -= OnJumpCanceled;

        inputActions.Player.Disable();
    }

    /// <summary>
    /// Updates the move input value in the unit state.
    /// </summary>
    private void UpdateMoveInput()
    {
        if (inputActions != null && inputActions.Player.enabled)
        {
            Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
            uMain.uState.MoveInput = moveInput;
        }
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 move = ctx.ReadValue<Vector2>();
        uMain.uState.MoveInput = move;
        uMain.uState.CurrentStateBase?.OnMove(move);
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        uMain.uState.MoveInput = Vector2.zero;
        uMain.uState.CurrentStateBase?.OnMove(Vector2.zero);
    }

    private void OnRun(InputAction.CallbackContext ctx)
    {
        uMain.uState.IsRunPerformed = true;
        uMain.uState.CurrentStateBase?.OnRun(true);
    }

    private void OnRunCanceled(InputAction.CallbackContext ctx)
    {
        uMain.uState.IsRunPerformed = false;
        uMain.uState.CurrentStateBase?.OnRun(false);
    }

    private void OnCrouch(InputAction.CallbackContext ctx)
    {
        uMain.uState.IsCrouchPerformed = true;
        uMain.uState.CurrentStateBase?.OnCrouch(true);
    }

    private void OnCrouchCanceled(InputAction.CallbackContext ctx)
    {
        uMain.uState.IsCrouchPerformed = false;
        uMain.uState.CurrentStateBase?.OnCrouch(false);
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        uMain.uState.IsJumpPerformed = true;
        uMain.uState.CurrentStateBase?.OnJump();
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        uMain.uState.IsJumpPerformed = false;
        uMain.uState.CurrentStateBase?.OnJumpCanceled();
    }
}
