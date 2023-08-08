using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private Game game;

    public void startGame()
    {
        game.NewGame();
    }

    public void settings()
    {
        
    }
}
