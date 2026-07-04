using System;
using UnityEngine;

public class LogScript : MonoBehaviour
{
    [SerializeField] private GameObject Player;

    private void Awake()
    {
        PlayerController p = Player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
