using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : InputManager
{
    private InputAction art;
    private Vector2 axis;
    private InputAction block;
    private InputAction cameraSet;
    private bool dialogClick;
    private InputAction dodge;
    private InputAction interact;
    private bool isMoving;
    private bool isRunning;
    private InputAction jump;
    private InputAction look;
    private InputAction move;
    private InputAction next;
    private InputAction nextWeapon;
    private InputAction normalAttack;
    private InputAction previous;
    private InputAction run;
    private InputAction strongAttack;
    private InputAction useItem;
    private InputAction walk;
    
    private GameObject lockOnTarget;


    protected override void Init()
    {
        move = InputSystem.actions.FindAction("Move");
        look = InputSystem.actions.FindAction("Look");
        run = InputSystem.actions.FindAction("Run");
        run.canceled += _ => isRunning = false;
        jump = InputSystem.actions.FindAction("Jump");
        normalAttack = InputSystem.actions.FindAction("NormalAttack");
        strongAttack = InputSystem.actions.FindAction("StrongAttack");
        interact = InputSystem.actions.FindAction("Interact");
        previous = InputSystem.actions.FindAction("Previous");
        next = InputSystem.actions.FindAction("Next");
        block = InputSystem.actions.FindAction("Block");
        dodge = InputSystem.actions.FindAction("Dodge");
        cameraSet = InputSystem.actions.FindAction("CameraSet");
        useItem = InputSystem.actions.FindAction("UseItem");
        art = InputSystem.actions.FindAction("Art");
        walk = InputSystem.actions.FindAction("Walk");
    }

    protected override void CalculateMove()
    {
        axis = move.ReadValue<Vector2>();
        evtMoveAxis?.Invoke(axis);
    }

    protected override void CalculateJump()
    {
        if (run.ReadValue<float>() == 0 || move.ReadValue<Vector2>().magnitude < 0.1f) return;
        evtJump?.Invoke(jump.WasPressedThisFrame());
    }

    protected override void CalculateRun()
    {
        if (move.ReadValue<Vector2>().magnitude < 0.1f) return;
        if (run.WasPerformedThisFrame()) isRunning = true;

        evtRun?.Invoke(isRunning);
    }

    protected override void CalculateDialogClick()
    {
        dialogClick = Mouse.current.leftButton.wasPressedThisFrame;
        evtDialogClick?.Invoke(dialogClick);
    }

    protected override void PostProcessDpadAxis()
    {
    }

    protected override void CalculateLook()
    {
        var lookValue = look.ReadValue<Vector2>();
        evtLook?.Invoke(lookValue);
    }

    protected override void CalculateDodge()
    {
        if (move.ReadValue<Vector2>().magnitude < 0.1f) return;
        evtDodge?.Invoke(dodge.WasPerformedThisFrame());
    }

    protected override void CalculateLockOn()
    {
        evtLockOn?.Invoke(cameraSet.WasPressedThisFrame());
    }
}