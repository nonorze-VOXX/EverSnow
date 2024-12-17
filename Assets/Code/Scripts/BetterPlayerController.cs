using UnityEngine;
public class BetterPlayerController : MonoBehaviour
{
    private static readonly int Speed = Animator.StringToHash("speed");
    private static readonly int DirX = Animator.StringToHash("DirX");
    private static readonly int DirY = Animator.StringToHash("DirY");

    public float velocity = 10f;
    public float runningSpeed = 20f;
    [SerializeField] private Camera cam;
    private readonly float rotationSpeed = 5f;
    private Animator anim;
    private bool isIdle;
    private bool isLockOn;
    private bool isSprinting;
    private bool isWalking;
    private float lastVelocity;
    private Vector2 lookVec;
    private Vector2 movingVec;
    private Rigidbody rigid;
    private STATE state = STATE.IDLE;
    private bool triggerEnter;
    private ThirdPersonCamera camSoul;
    private GameObject lockOnTarget;
    private AnimatorStateInfo StateInfo
    {
        get => anim.GetCurrentAnimatorStateInfo(0);
    }

    private bool IsGround
    {
        get => Physics.Raycast(transform.position, Vector3.down, 0.5f);
    }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = gameObject.transform.GetComponentInChildren<Animator>();
        GoToState(STATE.IDLE);
        PlayerInputManager.Instance.evtMoveAxis.AddListener(Move);
        PlayerInputManager.Instance.evtJump.AddListener(Jump);
        PlayerInputManager.Instance.evtRun.AddListener(Run);
        PlayerInputManager.Instance.evtLook.AddListener(Look);
        PlayerInputManager.Instance.evtDodge.AddListener(Dodge);
        camSoul = cam.GetComponent<ThirdPersonCamera>();
        camSoul.evtLock.AddListener(LockOn);
        camSoul.evtUnlock.AddListener(UnlockOn);
    }

    private void FixedUpdate()
    {
        var newVelocity = 0.0f;
        switch (state)
        {
            case STATE.IDLE:
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("idle", 0.1f);
                    triggerEnter = false;
                }

                if (movingVec.magnitude > 0.1f) GoToState(STATE.LOCOMOTION);
                break;

            case STATE.LOCOMOTION:
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime(isLockOn ? "lockon_locomotion" : "locomotion", 0.1f);
                    triggerEnter = false;
                }

                if (movingVec.magnitude <= 0.1f) GoToState(STATE.IDLE);
                if (!IsGround) GoToState(STATE.FALL);


                newVelocity = movingVec.magnitude * (isSprinting ? runningSpeed : velocity);
                if (isLockOn)
                {
                    anim.SetFloat(DirX, movingVec.x);
                    anim.SetFloat(DirY, movingVec.y);
                }
                else
                    anim.SetFloat(Speed, movingVec.magnitude * 1.0f + (isSprinting ? 1.0f : 0.0f));
                break;

            case STATE.JUMP:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("jump", 0.1f);
                    triggerEnter = false;
                    rigid.AddForce(Vector3.up * 4, ForceMode.Impulse); // Apply vertical jump force
                }

                // Maintain horizontal movement direction

                if (StateInfo.normalizedTime > 0.8f && StateInfo.IsName("jump")) GoToState(STATE.FALL);
                break;

            case STATE.FALL:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("fall", 0.1f);
                    triggerEnter = false;
                }

                // Keep the falling movement consistent with horizontal velocity
                break;

            case STATE.ROLL:
                newVelocity = lastVelocity;
                if (triggerEnter)
                {
                    anim.CrossFadeInFixedTime("roll_forward", 0.1f);
                    triggerEnter = false;
                }

                if (StateInfo.normalizedTime > 0.9f && StateInfo.IsName("roll_forward"))
                {
                    if (movingVec.magnitude <= 0.1f)
                        GoToState(STATE.IDLE);
                    else GoToState(STATE.LOCOMOTION);
                }

                // Maintain rolling direction and speed
                break;
        }

        newVelocity = Mathf.Lerp(lastVelocity, newVelocity, Time.deltaTime);
        // print($"lastVelocity: {lastVelocity}, newVelocity: {newVelocity}");
        MoveOn(movingVec, newVelocity);
        lastVelocity = newVelocity;
    }


    public void OnCollisionEnter(Collision collision)
    {
        if (state == STATE.JUMP)
        {
            if (movingVec.magnitude <= 0.1f)
                GoToState(STATE.IDLE);
            else GoToState(STATE.LOCOMOTION);
        }
        else if (state == STATE.FALL)
        {
            GoToState(STATE.ROLL);
        }
        //anim.SetBool("isGround", true);
    }


    private void MoveOn(Vector2 movingInputs, float currentVelocity)
    {
        if (currentVelocity == 0.0f) return;
        var cameraForward = cam.transform.forward;
        cameraForward.y = 0; // 移除垂直分量，保持水平運動
        cameraForward.Normalize();

        var cameraRight = cam.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        // 將輸入向量轉換為世界座標的方向向量
        var direction = cameraForward * movingInputs.y + cameraRight * movingInputs.x;

        // 計算目標旋轉
        if (direction != Vector3.zero)
        {
            var targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // 計算並應用運動速度
        var newVelocity = direction * currentVelocity;
        newVelocity.y = rigid.linearVelocity.y; // 保持垂直速度
        rigid.linearVelocity = newVelocity;
    }

    private void GoToState(STATE newState)
    {
        state = newState;
        triggerEnter = true;
    }

    private void Move(Vector2 vac)
    {
        movingVec = vac;
    }

    private void Look(Vector2 vac)
    {
        lookVec = vac;
    }

    private void Run(bool isRun)
    {
        isSprinting = isRun;
    }

    public void Jump(bool _isThrust)
    {
        if (lastVelocity < Mathf.Lerp(velocity, runningSpeed, 0.5f)) return;
        if (_isThrust && IsGround)
            if (state == STATE.IDLE || state == STATE.LOCOMOTION)
                GoToState(STATE.JUMP);
    }

    private void Dodge(bool isDodge)
    {
        if (!isDodge) return;
        if (state != STATE.ROLL)
            GoToState(STATE.ROLL);
    }

    private void LockOn(GameObject target)
    {
        lockOnTarget = target;
        isLockOn = true;
        if (state == STATE.LOCOMOTION)
            anim.CrossFadeInFixedTime("lockon_locomotion", 0.1f);
    }
    private void UnlockOn()
    {
        isLockOn = false;
        if (state == STATE.LOCOMOTION)
            anim.CrossFadeInFixedTime("locomotion", 0.1f);
    }
    

    private enum STATE
    {
        LOCOMOTION,
        IDLE,
        JUMP,
        FALL,
        ROLL
    }
}
