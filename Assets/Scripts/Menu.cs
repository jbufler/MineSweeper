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
    private int tempWidth = 16;
    private int tempHeight = 16;
    private int tempMines = 5;

    public void Start()
    {
        widthSlider.onValueChanged.AddListener(UpdateTempWidth);
        heightSlider.onValueChanged.AddListener(UpdateTempHeight);
        minesSlider.onValueChanged.AddListener(UpdateTempMines);
    }

    private void UpdateTempWidth(float newValue)
    {
        tempWidth = (int) newValue;
    }
    
    private void UpdateTempHeight(float newValue)
    {
        tempHeight = (int) newValue;
    }
    
    private void UpdateTempMines(float newValue)
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
