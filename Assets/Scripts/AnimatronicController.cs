using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class AnimatronicController : MonoBehaviour
{
    #region Variables

    [Header("Player Main Components:")] // Makes a header, looks like a title above all the variables in Unity
    [SerializeField]
    Rigidbody2D rb;

    Animator playerAnimator;
    SpriteRenderer spriteRenderer;
    HealthManager _healthManager;
    StaminaCircleManager _staminaCircleManager;

    [FormerlySerializedAs("MoveSpeed")] [Header("Player Status:")] [SerializeField]
    private float moveSpeed;
    [SerializeField] float defaultSpeed = 5f;
    [SerializeField] float sprintSpeed = 9f;
    [SerializeField] float sprintTime = 2f;
    [SerializeField] float sprintWait = 3f;
    [SerializeField] bool playerControl = true;
    [SerializeField] bool shiftPressed;

    public bool isSprinting;
    private bool isMoving = false;
    private bool shiftLock = false;
    private bool shiftUnlock = true;
    private Vector2 directionLock;
    private Vector2 _movementinput;
    private Vector2 movement;

    #endregion

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _healthManager = GetComponent<HealthManager>();
        _staminaCircleManager = GetComponent<StaminaCircleManager>();
    }
    
    private void FixedUpdate() // Updates a fixed amount of time, most times better than using time.deltaTime
    {
        Movement();
        Animation();
        _staminaCircleManager.StaminaCircle();
    }

    #region General Movement

    public void MoveInput(InputAction.CallbackContext context) // Check for an input, ZQSD, and gives the right value depending on which it is
    {
        if (playerControl) // Player has to have control
            _movementinput =
                context.ReadValue<Vector2>(); // Uses Vector2 to give values, Z is 1 on the y-axis, D is 1 on the x-axis, and the opposite for the rest
    }

    public void SprintInput(InputAction.CallbackContext context)
    {
        if (playerControl) // Player has to have control
            shiftPressed = context.ReadValueAsButton(); // Sends true if left shift is pressed, otherwise false
    }

    public void Movement()
    {
        isMoving = false;
        isSprinting = false;
        if (rb.linearVelocity.sqrMagnitude > 0) isMoving = true;

        if (shiftPressed && shiftUnlock && isMoving)
            StartCoroutine(LockSprint());

        if (shiftLock)
        {
            moveSpeed = sprintSpeed;
            isSprinting = true;
        }
        else moveSpeed = defaultSpeed;
        
        movement = shiftLock ? directionLock : _movementinput;
        
        rb.linearVelocity = movement * moveSpeed; // The player's velocity if the player is moving  
    }
    
    #endregion
    
    public IEnumerator LockSprint()
    {
        shiftLock = true;
        shiftUnlock = false;
        directionLock = _movementinput;
        yield return new WaitForSeconds(sprintTime);
        shiftLock = false;
        yield return new WaitForSeconds(sprintWait);
        shiftUnlock = true;
    }
    
    
    public void Animation()
    {
        if (_movementinput.magnitude > 0 && !shiftLock)
        {
            playerAnimator.SetBool("isWalking", true); // If isWalking alone is true, then we use the walk animation
            playerAnimator.SetFloat("InputX", _movementinput.x); // Checks for inputX for animation
            playerAnimator.SetFloat("InputY", _movementinput.y); // Checks for inputX for animation
            playerAnimator.SetFloat("LastInputX", _movementinput.x); // Checks for inputX and makes it the last input for idle animation
            playerAnimator.SetFloat("LastInputY", _movementinput.y); // Checks for inputX and makes it the last input for idle animation
        }
        else if (shiftLock)
        {
            playerAnimator.SetBool("isWalking", true); // If isWalking alone is true, then we use the walk animation
            playerAnimator.SetFloat("InputX", directionLock.x); // Checks for inputX for animation
            playerAnimator.SetFloat("InputY", directionLock.y); // Checks for inputX for animation
        }

        else playerAnimator.SetBool("isWalking", false);
    }
}