using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaCircleManager : MonoBehaviour
{
    
    #region Variables
    
    [Header("Main Components")]
    [SerializeField] GameObject stamina;
    public Image staminaCircle;
    public Image staminaBackground;
    AnimatronicController _animatronicController;
    
    [Header("Variables")]
    public float currentStamina = 1f;
    public float currentOpacity = 0;
    public bool canRun = true;
    #endregion
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _animatronicController = GetComponent<AnimatronicController>();
    }

    private void Start()
    {
        stamina.SetActive(false);
        StaminaCircleOpacity(currentOpacity); // Turns off the stamina bar by default
        StaminaCircleMaterialOpacity(1);
    }
    
    public void StaminaCircle()
    {
        
        if (_animatronicController.isSprinting)
        {
            StopAllCoroutines();
            StaminaDown();
        }
        else
        {
            StartCoroutine(StaminaUp());
        }
        
        StaminaCircleOpacity(currentOpacity);
    }

    private void StaminaDown()
    {
        currentOpacity = 1;
        stamina.SetActive(true);
        staminaCircle.fillAmount -= 0.67f * Time.fixedDeltaTime;
        if (staminaCircle.fillAmount <= 0) staminaCircle.fillAmount = 0;
    }

    private IEnumerator StaminaUp()
    {
        yield return new WaitForSeconds(1f);
        staminaCircle.fillAmount += 0.67f * Time.fixedDeltaTime;
        if (staminaCircle.fillAmount >= 1)
        {
            staminaCircle.fillAmount = 1;
            yield return new WaitForSeconds(2f);
            while (currentOpacity >= 0) // As long as the currentOpacity is higher than 0
            {
                currentOpacity -= 5f * Time.fixedDeltaTime; //Lower it (i wanna try doing it in another thread)
                yield return new WaitForSeconds(0.05f); // Every 0.05 seconds (because a while loop alone is too fast)
            }
            stamina.SetActive(false);// Once done, turn off the GameObject
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void StaminaCircleOpacity(float opacity) // Function to make accessing my stamina bar's opacity easier
    {
        staminaCircle.color = new Color(staminaCircle.color.r, staminaCircle.color.g, staminaCircle.color.b, opacity);
        staminaBackground.color = new Color(staminaBackground.color.r, staminaBackground.color.g, staminaBackground.color.b, opacity);
    }

    private void StaminaCircleMaterialOpacity(float opacity)
    {
        staminaCircle.material.color = new Color(1, 1, 1, opacity);
        staminaBackground.material.color = new Color(1, 1, 1, opacity);
    }
}
