using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour 
{ 
    public int width = 16;
    public int height = 16;
    public int mineCount = 32;

    private Board board;
    private Cell[,] state;
    private bool gameover;

    [SerializeField] private GameObject winCanvas;
    private void Awake()
    {
        board = GetComponentInChildren<Board>();
        winCanvas.gameObject.SetActive(false); 
    }

    private void Start()
    {
        //NewGame();
    }

    public void NewGame()
    {
        state = new Cell[width, height];

        GenerateCells();
        GenerateMines();
        GenerateNumbers();
        
        InitialReveal();
        
        Camera.main.transform.position = new Vector3(width / 2f, height / 2f, -10f);
        board.Draw(state);

    }

    private void GenerateCells()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = new Cell();
                cell.position = new Vector3Int(x, y, 0);
                cell.type = Cell.Type.Empty;
                state[x, y] = cell;
            }
        }
    }
    
    private void GenerateMines()
    {
        for (int i = 0; i < mineCount; i++)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);
            
            //if there is already a mine in the chosen field adjust position slightly until free field is found
            while (state[x, y].type == Cell.Type.Mine)
            {
                x++;

                if (x >= width)
                {
                    x = 0;
                    y++;

                    if (y >= height) {
                        y = 0;
                    }
                }
            }
            //place that mine
            state[x, y].type = Cell.Type.Mine;
        }
    }
    private void GenerateNumbers()
    {
        //once again loop through all fields
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];
            
                //if field is a mine we can ignore
                if (cell.type == Cell.Type.Mine) {
                    continue;
                }

                //count adjacent mines for every field
                cell.number = CountMines(x, y);

                //if there are adjacent mines assign number
                if (cell.number > 0) {
                    cell.type = Cell.Type.Number;
                }

                state[x, y] = cell;
            }
        }
    }

    private int CountMines(int cellX, int cellY)
    {
        int count = 0;
        
        //run through all adjacent squares to check for bombs skip own field
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0) {
                    continue;
                }

                int x = cellX + adjacentX;
                int y = cellY + adjacentY;
                
                //ceck if we are out of bounds
                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    continue;
                }
                
                if (state[x, y].type == Cell.Type.Mine) {
                    count++;
                }
            }
        }

        return count;
    }
  private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(0);
        }
        else if (!gameover)
        {
            if (Input.GetMouseButtonDown(1)) {
                Flag();
            } else if (Input.GetMouseButtonDown(0)) {
                Reveal();
            }
        }
    }

    private void Flag()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        // Cannot flag if already revealed
        if (cell.type == Cell.Type.Invalid || cell.revealed) {
            return;
        }

        cell.flagged = !cell.flagged;
        state[cellPosition.x, cellPosition.y] = cell;
        board.Draw(state);
    }

    private void Reveal()
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPosition = board.tilemap.WorldToCell(worldPosition);
        Cell cell = GetCell(cellPosition.x, cellPosition.y);

        if (cell.revealed && cell.type == Cell.Type.Number)
        {
            numberGamble(cellPosition.x,cellPosition.y);
        }
        
        // Cannot reveal if already revealed or while flagged
        if (cell.type == Cell.Type.Invalid || cell.revealed || cell.flagged) {
            return;
        }
        
        switch (cell.type)
        {
            case Cell.Type.Mine:
                Explode(cell);
                break;

            case Cell.Type.Empty:
                Flood(cell);
                CheckWinCondition();
                break;

            default:
                cell.revealed = true;
                state[cellPosition.x, cellPosition.y] = cell;
                CheckWinCondition();
                break;
        }

        board.Draw(state);
    }

    
    //function for revealing all adjacent squares if suffienctly enough are flagged in area
    private void numberGamble(int cellX, int cellY)
    {
        int flagCount = 0;
        int revealCount = 0;
        
        for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
        {
            for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
            {
                if (adjacentX == 0 && adjacentY == 0) {
                    continue;
                }

                int x = cellX + adjacentX;
                int y = cellY + adjacentY;
                
                //ceck if we are out of bounds
                if (x < 0 || x >= width || y < 0 || y >= height)
                {
                    revealCount++;
                    continue;
                }
                
                Cell cellTemp = GetCell(x,y);
                if (cellTemp.flagged) {
                    flagCount++;
                }
                if (cellTemp.revealed) {
                    revealCount++;
                }
            }
        }
        
        //if conditions right reveal all which are not yet revealed
        Cell cell = GetCell(cellX,cellY);
        
        if (flagCount == cell.number && revealCount < 8-cell.number)
        {
            for (int adjacentX = -1; adjacentX <= 1; adjacentX++)
            {
                for (int adjacentY = -1; adjacentY <= 1; adjacentY++)
                {
                    if (adjacentX == 0 && adjacentY == 0)
                    {
                        continue;
                    }

                    int x = cellX + adjacentX;
                    int y = cellY + adjacentY;

                    //ceck if we are out of bounds 
                    if (x < 0 || x >= width || y < 0 || y >= height)
                    {
                        continue;
                    }

                    Cell cellTemp = GetCell(x, y);

                    if (cellTemp.flagged)
                    {
                        continue;
                    }
                    
                    switch (cellTemp.type)
                    {
                        case Cell.Type.Mine:
                            Explode(cellTemp);
                            break;

                        case Cell.Type.Empty:
                            Flood(cellTemp);
                            CheckWinCondition();
                            break;

                        default:
                            cellTemp.revealed = true;
                            state[x, y] = cellTemp;
                            CheckWinCondition();
                            break;
                    }
                    board.Draw(state);
                }
            }
        }
    }
    
    //if we press in an empty cell we want to reveal all adjacent empty cells
    private void Flood(Cell cell)
    {
        // Recursive exit conditions
        if (cell.revealed) return;
        if (cell.type == Cell.Type.Mine || cell.type == Cell.Type.Invalid) return;

        // Reveal the cell
        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        // Keep flooding if the cell is empty, otherwise stop at numbers
        if (cell.type == Cell.Type.Empty)
        {
            Flood(GetCell(cell.position.x - 1, cell.position.y));
            Flood(GetCell(cell.position.x + 1, cell.position.y));
            Flood(GetCell(cell.position.x, cell.position.y - 1));
            Flood(GetCell(cell.position.x, cell.position.y + 1));
            
            Flood(GetCell(cell.position.x - 1, cell.position.y -1));
            Flood(GetCell(cell.position.x + 1, cell.position.y +1));
            Flood(GetCell(cell.position.x + 1, cell.position.y - 1));
            Flood(GetCell(cell.position.x - 1, cell.position.y + 1));
        }
    }

    private void InitialReveal()
    {
        int x = Random.Range(0, width);
        int y = Random.Range(0, height);
            
        //if there is already a mine in the chosen field adjust position slightly until free field is found
        while (state[x, y].type != Cell.Type.Empty)
        {
            x++;
            if (x >= width)
            {
                x = 0;
                y++;

                if (y >= height) {
                    y = 0;
                }
            }
        }
        Cell cell = GetCell(x,y);
        Flood(cell);
    }

    private void Explode(Cell cell)
    {
        Debug.Log("Game Over!");
        gameover = true;

        // Set the mine as exploded
        cell.exploded = true;
        cell.revealed = true;
        state[cell.position.x, cell.position.y] = cell;

        // Reveal all other mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.revealed = true;
                    state[x, y] = cell;
                }
            }
        }
        StartCoroutine(ExampleCoroutine());
    }

    private void CheckWinCondition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                // All non-mine cells must be revealed to have won
                if (cell.type != Cell.Type.Mine && !cell.revealed) {
                    return; // no win
                }
            }
        }

        Debug.Log("Winner!");
        
        gameover = true;

        // Flag all the mines
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cell cell = state[x, y];

                if (cell.type == Cell.Type.Mine)
                {
                    cell.flagged = true;
                    state[x, y] = cell;
                }
            }
        }
        winCanvas.gameObject.SetActive(true);
        StartCoroutine(ExampleCoroutine());
        
    }
    IEnumerator ExampleCoroutine()
    {
        //Print the time of when the function is first called.

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(5);

        SceneManager.LoadScene(0);
    }
    private Cell GetCell(int x, int y)
    {
        if (IsValid(x, y)) {
            return state[x, y];
        } 
        else {
            return new Cell();
        }
    }

    private bool IsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

}