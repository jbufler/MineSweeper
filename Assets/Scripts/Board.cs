using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    //Tilemap of the game with public getter and private setter function
    public Tilemap tilemap { get; private set; }
    
    //All the tiles
    public Tile tileUnknown;
    public Tile tileEmpty;
    public Tile tileMine;
    public Tile tileExploded;
    public Tile tileFlags;
    public Tile tileNum1;
    public Tile tileNum2;
    public Tile tileNum3;
    public Tile tileNum4;
    public Tile tileNum5;
    public Tile tileNum6;
    public Tile tileNum7;
    public Tile tileNum8;
    public Tile tileNum9;
    
    //Awake gets called automaticallly with creation
    public void Awake()
    {
        //if tilemap component exists assign it automatically if not it is Null
        //if later NullReference here is the issue
        tilemap = GetComponent<Tilemap>();
        
        //two dimensional array of states
        public void Draw(Cell[,] state)
        {
            //x
            int width = state.GetLength(0);
            //y
            int height = state.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Cell cell = state[x, y];
                    tilemap.SetTile(cell.position, GetTile(cell));
                }
            }
        }
    }

    private Tile GetTile(Cell cell)
    {
        if (cell.revealed)
        {
            //retun number or empty or bomb
            return null;
        }else if (cell.flagged)
        {
            //return flag
            return null;
        }
        else
        {
            //return basic unrevealed tile
            return null;
        }
    }
}
