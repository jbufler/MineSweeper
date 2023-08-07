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
    
    //Awake gets called automaticallly with creation
    public void Awake()
    {
        //if tilemap component exists assign it automatically if not it is Null
        //if later NullReference here is the issue
        tilemap = GetComponent<Tilemap>();
    }

    //two dimensional array of states
        public void Draw(Cell[,] state)
        {
            int width = state.GetLength(0);
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

    

    private Tile GetTile(Cell cell)
    {
        if (cell.revealed)
        {
            //retun number or empty or bomb
            return GetRevealedTile(cell);
            
        }else if (cell.flagged)
        {
            //return flag
            return tileFlags;
        }
        else
        {
            //return basic unrevealed tile
            return tileUnknown;
        }
    }
    
    //return which revealed tile is returned if it is a number move on to number tile function
    private Tile GetRevealedTile(Cell cell)
    {
        switch (cell.type)
        {
            case Cell.Type.Empty: return tileEmpty;
            case Cell.Type.Mine: return cell.exploded ? tileExploded : tileMine;
            case Cell.Type.Number: return GetNumberTile(cell);
            default: return null;
        }
    }
    
    //check and return the number
    private Tile GetNumberTile(Cell cell)
    {
        switch (cell.number)
        {
            case 1: return tileNum1;
            case 2: return tileNum2;
            case 3: return tileNum3;
            case 4: return tileNum4;
            case 5: return tileNum5;
            case 6: return tileNum6;
            case 7: return tileNum7;
            case 8: return tileNum8;
            default: return null;
        }
    }
}
