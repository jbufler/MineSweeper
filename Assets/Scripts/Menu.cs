using System;
using System.Collections;
using System.Collections.Generic;using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Game game;
    public Slider widthSlider;
    public Slider heightSlider;
    public Slider minesSlider;
    public int tempWidth = 16;
    public int tempHeight = 16;
    public int tempMines = 5;

    public void Start()
    {
        widthSlider.onValueChanged.AddListener(UpdateTempWidth);
        heightSlider.onValueChanged.AddListener(UpdateTempHeight);
        minesSlider.onValueChanged.AddListener(UpdateTempMines);
    }

    public void UpdateTempWidth(float newValue)
    {
        tempWidth = (int) newValue;
    }
    
    public void UpdateTempHeight(float newValue)
    {
        tempHeight = (int) newValue;
    }
    
    public void UpdateTempMines(float newValue)
    {
        tempMines = (int) newValue;
    }


    public void StartGame()
    {
        game.height = 16;
        game.width = 16;
        game.mineCount = 32;
        game.NewGame();
    }

    
    public void settings()
    {
        game.height = tempHeight;
        game.width = tempWidth;
        game.mineCount = tempMines;                                                  
        game.NewGame();
    }
}
