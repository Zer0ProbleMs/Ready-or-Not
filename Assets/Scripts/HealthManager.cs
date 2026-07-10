using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using Image = UnityEngine.UI.Image;

public class HealthManager : MonoBehaviour
{
    [Header("Main Components")]
    [SerializeField] Sprite emptyHeart;
    [SerializeField] Sprite fullHeart;
    public Image[] hearts;
    
    [Header("Variables")]
    [SerializeField] int lifeAmount = 3;
    [SerializeField] int maxLives = 3;
    public bool isAlive;
    
    
    // Update is called once per frame
    void FixedUpdate()
    {
        HealthCheck(); // Checks the health over and over during FixedUpdate
    }

    public void HealthCheck()
    {
        isAlive = (lifeAmount > 0) ? true : false; // If the player has at least one heart, he is alive and has control
        
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < lifeAmount) // Checks for each heart, if it should be full or empty
                hearts[i].sprite = fullHeart; // Makes the heart full
            else
                hearts[i].sprite = emptyHeart; // Makes the heart empty
            
            if (i < maxLives) // Allows to makes as many hearts a necessary
                hearts[i].enabled = true; // If i goes above the amount of heart wanted (without going above max), then it stops
            else
                hearts[i].enabled = false;
        }
    }
    
    public void TakeHealth(int amount)
    {
        lifeAmount -= amount; // Takes off one heart when function is called
    }
    
}
