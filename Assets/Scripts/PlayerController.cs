using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables
    
    [Header("Player Main Components:")] // Makes a header, looks like a title above all the variables in Unity
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Image staminaBar;
    [SerializeField] Image staminaBackground;
    [SerializeField] GameObject stamina;

    Animator playerAnimator;
    SpriteRenderer spriteRenderer;
    
    [FormerlySerializedAs("MoveSpeed")]
    [Header("Player Status:")]
    [SerializeField] private float moveSpeed = 0;
    [SerializeField] private float defaultSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float currentStamina = 100f;
    [SerializeField] private bool runPressed = false;
    [SerializeField] private bool canRun = true;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool playerControl = true;
    
    private float maxStamina = 100;
    private float staminaDown = 40f;
    private float staminaUp = 5f;
    private Vector2 _movementinput;
    private float currentOpacity;
    private bool _context;
    
    #endregion

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        currentOpacity = 0;
        stamina.SetActive(false);
    }

    private void Update() // Updates every frame
    {
        
    }

    private void FixedUpdate() // Updates a fixed amount of time, most times better than using time.deltaTime
    {
        Movement();
        StaminaBar();
        Animation();
    }

    #region General Movement
    
    public void MoveInput(InputAction.CallbackContext context) // Check for an input, ZQSD, and gives the right value depending on which it is
    {
        
        _context = context.canceled;
        _movementinput = context.ReadValue<Vector2>(); // Uses Vector2 to give values, Z is 1 on the y-axis, D is 1 on the x-axis, and the opposite for the rest
    }
    
    public void SprintInput(InputAction.CallbackContext context)
    {
        runPressed = context.ReadValueAsButton(); // Sends true if left shift is pressed, otherwise false
    }

    public void Movement()
    {
        if (isRunning)
            moveSpeed = sprintSpeed; // If the player is holding shift, then the movement is faster
        
        else
            moveSpeed = defaultSpeed; // Otherwise the player moves at default speed
        
        if (!playerControl) // Checks if the player is allowed to move or not
            rb.linearVelocity = new Vector2(0, 0); // Stops the player from moving
        else
            rb.linearVelocity = _movementinput.normalized * moveSpeed; // The player's velocity if the player is moving
        
        if (_movementinput.x > 0)
            spriteRenderer.flipX = true;
        else if (_movementinput.x < 0)
            spriteRenderer.flipX = false;
    }

    public void StaminaBar()
    {
        staminaBar.color = new Color(staminaBar.color.r, staminaBar.color.g, staminaBar.color.b, currentOpacity);
        staminaBackground.color = new Color(staminaBackground.color.r, staminaBackground.color.g, staminaBackground.color.b, currentOpacity);
        isRunning = false;
        
        if (runPressed && rb.linearVelocity.magnitude > 0 && canRun) // Checks if the player is moving, wants to run and has stamina
        {
            StopAllCoroutines(); // Stops the stamina from recharging
            currentOpacity = 1;
            stamina.SetActive(true);
            currentStamina -= staminaDown * Time.fixedDeltaTime; // Lowers stamina by a fixed rate
            staminaBar.fillAmount = currentStamina / maxStamina;
            isRunning = true;
        }
        else
            StartCoroutine(RechargeStaminaBar()); // If the player isn't running, then we start the recharge stamina coroutine

        if (currentStamina <= 0) // If the stamina reaches the minimum, then it stops
        {
            currentStamina = 0;
            canRun = false; // Can't run at all if the stamina is all the way down
        }
        
        
    }

    private IEnumerator RechargeStaminaBar()
    {
        yield return new WaitForSeconds(2.5f); // Starts the following code after 2.5 seconds

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaUp * Time.fixedDeltaTime; // Recharges stamina
            staminaBar.fillAmount = currentStamina / maxStamina;
            if (currentStamina >= maxStamina)   // If stamina reaches the max, then it stops
            {
                currentStamina = maxStamina;
                canRun = true; // Can run again once the stamina is filled back
                yield return new WaitForSeconds(2f);
                while (currentOpacity >= 0) // As long as the currentOpacity is higher than 0
                {
                    currentOpacity -= 5f * Time.fixedDeltaTime; //Lower it
                    yield return new WaitForSeconds(0.05f); // Every 0.05 seconds (because a while loop alone is too fast)
                }
                stamina.SetActive(false); // Once done, turn off the GameObject
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    #endregion

    public void Animation()
    {
        if (_movementinput.magnitude > 0)
        {
            
            playerAnimator.SetBool("isWalking", true);
            playerAnimator.SetBool("isRunning", isRunning);
            playerAnimator.SetFloat("InputX", _movementinput.x);
            playerAnimator.SetFloat("InputY", _movementinput.y);
            playerAnimator.SetFloat("LastInputX", _movementinput.x);
            playerAnimator.SetFloat("LastInputY", _movementinput.y);
        }
        else playerAnimator.SetBool("isWalking", false);
        

    }
}
