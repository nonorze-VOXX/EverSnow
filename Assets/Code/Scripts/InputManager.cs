using UnityEngine;
using UnityEngine.Events;
public abstract class InputManager : Singleton<InputManager>
{
    public UnityEvent<Vector2> evtMoveAxis;
    public UnityEvent<bool> evtJump;
    public UnityEvent<bool> evtRun;
    public UnityEvent<bool> evtDialogClick; // Highlighted section 0
    public UnityEvent<Vector2> evtLook;
    public UnityEvent<bool> evtDodge;
    public UnityEvent<bool> evtLockOn;

    private void Update()
    {
        CalculateMove();
        CalculateJump();
        CalculateRun();
        CalculateDodge();
        CalculateDialogClick(); // Highlighted section 3
        PostProcessDpadAxis();
        CalculateLook();
        CalculateLockOn();
    }
    private void FixedUpdate()
    {}

    protected abstract void CalculateMove();
    protected abstract void CalculateJump();
    protected abstract void CalculateRun();
    protected abstract void CalculateDialogClick(); // Highlighted section 2
    protected abstract void PostProcessDpadAxis();
    protected abstract void CalculateLook();
    protected abstract void CalculateDodge();
    protected abstract void CalculateLockOn();
}
