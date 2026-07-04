using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    [Header("Player Main Components:")] // Makes a header, looks like a title above all the variables in Unity
    [SerializeField] Rigidbody2D rb;
    
    [FormerlySerializedAs("MoveSpeed")]
    [Header("Player Status:")]
    [SerializeField] private float moveSpeed = 0;
    [SerializeField] private float defaultSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float currentStamina = 100f;
    [SerializeField] private bool isRunning = false;
    
    private float maxStamina = 100;
    private float staminaDown = 40f;
    private float staminaUp = 5f;
    private Vector2 _movementinput;

    private void Awake()
    {
        
    }

    private void Update() // Updates every frame
    {
        
    }

    private void FixedUpdate() // Updates a fixed amount of time, most times better than using time.deltaTime
    {
        Movement();
        StaminaBar();
    }

    #region General Movement
    
    public void MoveInput(InputAction.CallbackContext context) // Check for an input, ZQSD, and gives the right value depending on which it is
    {
        _movementinput = context.ReadValue<Vector2>(); // Uses Vector2 to give values, Z is 1 on the y-axis, D is 1 on the x-axis, and the opposite for the rest
        _movementinput = _movementinput.normalized;
    }
    
    public void SprintInput(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton(); // Sends true if left shift is pressed, otherwise false
    }

    public void Movement()
    {
        if (isRunning && currentStamina > 0)
            moveSpeed = sprintSpeed; // If the player is holding shift, then the movement is faster
        
        else
            moveSpeed = defaultSpeed; // Otherwise the player moves at default speed
        
        rb.linearVelocity = _movementinput * moveSpeed; // The player's velocity
    }

    public void StaminaBar()
    {
        if (isRunning && currentStamina > 0 && rb.linearVelocity.magnitude > 0)
        {
            StopAllCoroutines();
            currentStamina -= staminaDown * Time.fixedDeltaTime;
        }

        else
            StartCoroutine(RechargeStaminaBar());
        
        if (currentStamina <= 0) currentStamina = 0; // If the stamina reaches the minimum, then it stops
    }

    private IEnumerator RechargeStaminaBar()
    {
        yield return new WaitForSeconds(2.5f);

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaUp * Time.fixedDeltaTime;
            if (currentStamina >= maxStamina) currentStamina = maxStamina; // If stamina reaches the max, then it stops
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    #endregion
}
