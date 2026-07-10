using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    #region Variables
    
    [Header("Player Main Components:")] // Makes a header, looks like a title above all the variables in Unity
    [SerializeField] Rigidbody2D rb;
    
    Animator playerAnimator;
    SpriteRenderer spriteRenderer;
    StaminaManager _staminaManager;
    HealthManager _healthManager;
    
    [FormerlySerializedAs("MoveSpeed")]
    [Header("Player Status:")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float defaultSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private bool playerControl = true;
    [SerializeField] bool runPressed;
    public bool isRunning;
    
    private Vector2 _movementinput;
    
    #endregion

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _staminaManager = GetComponent<StaminaManager>();
        _healthManager = GetComponent<HealthManager>();
    }
    
    
    private void FixedUpdate() // Updates a fixed amount of time, most times better than using time.deltaTime
    {
        PlayerControllable();
        Movement();
        _staminaManager.StaminaBar();
        Animation();
    }

    #region General Movement
    
    public void MoveInput(InputAction.CallbackContext context) // Check for an input, ZQSD, and gives the right value depending on which it is
    {
        if(playerControl) // Player has to have control
            _movementinput = context.ReadValue<Vector2>(); // Uses Vector2 to give values, Z is 1 on the y-axis, D is 1 on the x-axis, and the opposite for the rest
    }
    
    public void SprintInput(InputAction.CallbackContext context)
    {
        if(playerControl) // Player has to have control
            runPressed = context.ReadValueAsButton(); // Sends true if left shift is pressed, otherwise false
    }

    public void Movement()
    {
        isRunning = false;
        
        if (runPressed && rb.linearVelocity.magnitude > 0 && _staminaManager.canRun)
        {
            isRunning = true;
            moveSpeed = sprintSpeed; // If the player is holding shift, then the movement is faster
        } 
        
        else
            moveSpeed = defaultSpeed; // Otherwise the player moves at default speed

        rb.linearVelocity = _movementinput.normalized * moveSpeed; // The player's velocity if the player is moving
        
        /*
        if (_movementinput.x > 0)
            spriteRenderer.flipX = true;
        else if (_movementinput.x < 0)
            spriteRenderer.flipX = false; // Dead code that can be used if the player doesn't have two sided animations
            */
    }
    
    #endregion

    public void PlayerHit()
    {
        _healthManager.TakeHealth(1); // If the player is hit, it will take one heart off
    }

    public void Animation()
    {
        playerAnimator.SetBool("isRunning", isRunning); //If isRunning is true, then we use the run animation
        if (_movementinput.magnitude > 0)
        {
            playerAnimator.SetBool("isWalking", true); // If isWalking alone is true, then we use the walk animation
            playerAnimator.SetFloat("InputX", _movementinput.x); // Checks for inputX for animation
            playerAnimator.SetFloat("InputY", _movementinput.y); // Checks for inputX for animation
            playerAnimator.SetFloat("LastInputX", _movementinput.x); // Checks for inputX and makes it the last input for idle animation
            playerAnimator.SetFloat("LastInputY", _movementinput.y); // Checks for inputX and makes it the last input for idle animation
        }
        else playerAnimator.SetBool("isWalking", false);
    }

    public void PlayerControllable()
    {
        playerControl = true; // Player has control by default
        
        if (!_healthManager.isAlive) playerControl = false; // Unless he isn't alive
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerHit(); // When entering any collision, takes one heart off (for test purposes)
    }
}
