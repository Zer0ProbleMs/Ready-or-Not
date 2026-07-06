using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaManager : MonoBehaviour
{
    #region Variables
    
    [Header("Main Components")]
    [SerializeField] GameObject stamina;
    public Image staminaBar;
    public Image staminaBackground;
    PlayerController _playerController;
    
    [Header("Variables")]
    public float maxStamina = 100;
    public float currentStamina = 100f;
    public float staminaDown = 40f;
    public float staminaUp = 5f;
    public float currentOpacity = 0;
    public bool canRun = true;
    
    #endregion

    public void Awake()
    {
        _playerController = GetComponent<PlayerController>();
    }

    public void Start()
    {
        stamina.SetActive(false);
        StaminaBarOpacity(currentOpacity);
        StaminaBarMaterialOpacity(1);
    }
    
    public void StaminaBar()
    {
        Debug.Log(staminaBar.material.color); Debug.Log(staminaBackground.material.color);
        StaminaBarMaterialOpacity(1);
        StaminaBarOpacity(currentOpacity);
        
        if (_playerController.isRunning) // Checks if the player is moving, wants to run and has stamina
        {
            StopAllCoroutines(); // Stops the stamina from recharging
            currentOpacity = 1;
            stamina.SetActive(true);
            StaminaBarOpacity(currentOpacity);
            currentStamina -= staminaDown * Time.fixedDeltaTime; // Lowers stamina by a fixed rate
            staminaBar.fillAmount = currentStamina / maxStamina;
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
                    currentOpacity -= 5f * Time.fixedDeltaTime; //Lower it (i wanna try doing it in another thread)
                    yield return new WaitForSeconds(0.05f); // Every 0.05 seconds (because a while loop alone is too fast)
                }
                stamina.SetActive(false);// Once done, turn off the GameObject
            }
            yield return new WaitForSeconds(0.1f); // Try replacing by Time.deltaTime?
        }
    }

    public void StaminaBarOpacity(float Opacity)
    {
        staminaBar.color = new Color(staminaBar.color.r, staminaBar.color.g, staminaBar.color.b, Opacity);
        staminaBackground.color = new Color(staminaBackground.color.r, staminaBackground.color.g, staminaBackground.color.b, Opacity);
    }

    public void StaminaBarMaterialOpacity(float Opacity)
    {
        staminaBar.material.color = new Color(1, 1,
            1, Opacity);
        staminaBackground.material.color = new Color(1, 1,
            1, Opacity);
    }
}
