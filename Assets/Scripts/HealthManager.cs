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
        HealthCheck();
    }

    public void HealthCheck()
    {
        isAlive = (lifeAmount > 0) ? true : false;
        
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < lifeAmount)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
            
            if (i < maxLives)
                hearts[i].enabled = true;
            else
                hearts[i].enabled = false;
        }
    }
    
    public void TakeHealth(int amount)
    {
        lifeAmount -= amount;
    }
    
    
}
