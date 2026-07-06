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

    Animator playerAnimator;
    SpriteRenderer spriteRenderer;
    StaminaManager _staminaManager;
    HealthManager _healthManager;
    
    [FormerlySerializedAs("MoveSpeed")]
    [Header("Player Status:")]
    [SerializeField] private float moveSpeed = 0;
    [SerializeField] private float defaultSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private bool playerControl = true;
    [SerializeField] bool runPressed = false;
    public bool isRunning = false;
    
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
        if(playerControl)
            _movementinput = context.ReadValue<Vector2>(); // Uses Vector2 to give values, Z is 1 on the y-axis, D is 1 on the x-axis, and the opposite for the rest
    }
    
    public void SprintInput(InputAction.CallbackContext context)
    {
        if(playerControl)
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
        _healthManager.TakeHealth(1);
    }

    public void Animation()
    {
        playerAnimator.SetBool("isRunning", isRunning);
        if (_movementinput.magnitude > 0)
        {
            playerAnimator.SetBool("isWalking", true);
            playerAnimator.SetFloat("InputX", _movementinput.x);
            playerAnimator.SetFloat("InputY", _movementinput.y);
            playerAnimator.SetFloat("LastInputX", _movementinput.x);
            playerAnimator.SetFloat("LastInputY", _movementinput.y);
        }
        else playerAnimator.SetBool("isWalking", false);
    }

    public void PlayerControllable()
    {
        playerControl = true;
        
        if (!_healthManager.isAlive) playerControl = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerHit();
    }
}
