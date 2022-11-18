using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WFC_GridCreation : MonoBehaviour
{


    /// <summary>
    /// Defines all of the possible tiles types. In this example it simply uses colours.
    /// </summary>
    public enum TileTypes
    {
        BLANK,
        GREEN,
        RED,
        BLACK,
        BLUE,
        YELLOW,
    }

    [SerializeField] private List<WFC_OnTileSO> tilesTypes;
    

    [SerializeField]private GameObject tilePrefab;


    [SerializeField]private int gridSizeX;
    [SerializeField]private int gridSizeY;
    
    private GameObject[] gridc;

    private float timer = 0.0f;
    [SerializeField]private bool useTimer;

   
    private void Start()
    {
        StartGrid();
    }

    /// <summary>
    /// Starts creating the grid.
    /// </summary>
    private void StartGrid()
    {
        GameObject[] grid = CreateGrid();
        SelectRandomTileAndTurn(grid);
        gridc = grid;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKeyDown("f") || (useTimer && timer > 0.02f))
        {
           StepWFC();
        }
    }

    /// <summary>
    /// Steps the WFC algorithm one step fowards by solving the highest entropy tile.
    /// Loops through all tiles and selects highest, then passes it to "SetPossibleTile"
    /// </summary>
    private void StepWFC()
    {
        timer = 0.0f;
        foreach (var x in gridc)
        {
            if(!x.GetComponent<WFC_OnTile>().ready) return;
        }

        float e = 0.0f;
        WFC_OnTile pickedTile = new WFC_OnTile();
        foreach (var x in gridc)
        {
            if (x.GetComponent<WFC_OnTile>().entropy > e && !x.GetComponent<WFC_OnTile>().solved)
            {
                e = x.GetComponent<WFC_OnTile>().entropy;
                x.GetComponent<WFC_OnTile>().ready = false;
                pickedTile = x.GetComponent<WFC_OnTile>();
            } 
        }
            
        if(pickedTile != null)SetToPossibleTile(pickedTile);
    }

    /// <summary>
    /// Creates the grid that the tiles sit on.
    /// </summary>
    /// <returns>Returns the created grid with a set of blank tiles</returns>
    private GameObject[] CreateGrid()
    {
        GameObject[] grid = new GameObject[gridSizeX*gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeX; y++)
            {
                grid[x + gridSizeY * y] = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                grid[x + gridSizeY * y].GetComponent<WFC_OnTile>().SetXY(x,y);
                grid[x + gridSizeY * y].GetComponent<WFC_OnTile>().SetTile(tilesTypes[0]);
                grid[x + gridSizeY * y].name = "Tile " + x + " | " + y;
                foreach (var tt in tilesTypes)
                {
                    if(tt.typeOfTile == TileTypes.BLANK) continue;
                    grid[x + gridSizeY * y].GetComponent<WFC_OnTile>().AddPossibleTile(tt.typeOfTile);
                }
            }
        }

        return grid;
    }

    /// <summary>
    /// To be used at the start, selects a tile randomly from the grid and sets it to solved, changing its appearance and TileType.
    /// </summary>
    /// <param name="grid"></param>
    private void SelectRandomTileAndTurn(GameObject[] grid)
    {
        int x = Random.Range(0, gridSizeX);
        int y = Random.Range(0, gridSizeY);
        WFC_OnTileSO turnedTileInfo = tilesTypes[Random.Range(1, tilesTypes.Count)];
        grid[x + gridSizeY * y].GetComponent<WFC_OnTile>().SetTile(turnedTileInfo);
        
        SetPossibleTiles(grid, x,y);
    }

    /// <summary>
    /// Takes in a tile and sets it to a one of the possible tiles that it can be.
    /// </summary>
    /// <param name="onTile">Tile to change</param>
    private void SetToPossibleTile(WFC_OnTile onTile)
    {
        WFC_OnTileSO tileInfo = new WFC_OnTileSO();
        tileInfo.typeOfTile = TileTypes.BLANK;

        SetPossibleTiles(gridc, onTile.xID, onTile.yID);
        
        foreach (var x in tilesTypes)
        {
            if (onTile.possibleTiles.Count <= 0) return;
            if (onTile.possibleTiles.Contains(x.typeOfTile))
            {
                if (tileInfo.typeOfTile == TileTypes.BLANK || Random.Range(0.0f, 1.0f) > .5f)
                {
                    tileInfo = x;
                }
            }
        }
        ResetReady();
        onTile.SetTile(tileInfo);
        SetPossibleTiles(gridc, onTile.xID,onTile.yID);
    }

    
    /// <summary>
    /// Checks to make sure that the recursive function has completed. Needs to be refactored to not need this.
    /// </summary>
    private void ResetReady()
    {
        foreach (var VARIABLE in gridc)
        {
            VARIABLE.GetComponent<WFC_OnTile>().ready = false;
        }
    }

    //Needs to be refactored, not happy with how this is working.
    
    /// <summary>
    /// Recursively goes through the grid and makes sure that each tile has only its possible tiles set
    /// 
    /// </summary>
    /// <param name="grid"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool SetPossibleTiles(GameObject[] grid, int x, int y)
    {
        WFC_OnTile parentTile = grid[x + gridSizeY * y].GetComponent<WFC_OnTile>();
        if (parentTile.ready) return true;
        parentTile.ready = true;
        bool west = false;
        bool south = false;
        bool east = false;
        bool north = false;
        //West
        if (x > 0)
        {
            List<TileTypes> tiles = parentTile.GetAllWestTiles();
            foreach (var tt in tilesTypes)
            {
                if(tiles.Contains(tt.typeOfTile)) continue;
                grid[x - 1 + gridSizeY * y].GetComponent<WFC_OnTile>().RemovePossibleTile(tt.typeOfTile);
            }
            
            west = SetPossibleTiles(grid, x-1, y);
        }
        
        //East
        if (x < gridSizeX-1)
        {
            List<TileTypes> tiles = parentTile.GetAllEastTiles();
            foreach (var tt in tilesTypes)
            {
                if(tiles.Contains(tt.typeOfTile)) continue;
                grid[x + 1 + gridSizeY * y].GetComponent<WFC_OnTile>().RemovePossibleTile(tt.typeOfTile);
            }
            east = SetPossibleTiles(grid, x+1, y);

        }
        
        //North
        if (y < gridSizeY-1)
        {
            List<TileTypes> tiles = parentTile.GetAllNorthTiles();
            foreach (var tt in tilesTypes)
            {
                if(tiles.Contains(tt.typeOfTile)) continue;
                grid[x + gridSizeY * (y + 1)].GetComponent<WFC_OnTile>().RemovePossibleTile(tt.typeOfTile);
            }
            north = SetPossibleTiles(grid, x, y+1);

        }
        
        //South
        if (y > 0)
        {
            List<TileTypes> tiles = parentTile.GetAllSouthTiles();
            foreach (var tt in tilesTypes)
            {
                if(tiles.Contains(tt.typeOfTile)) continue;
                grid[x + gridSizeY * (y - 1)].GetComponent<WFC_OnTile>().RemovePossibleTile(tt.typeOfTile);
            }
            south = SetPossibleTiles(grid, x, y-1);

        }

        return north && south && west && east;
    }
}
