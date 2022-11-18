using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WFC_OnTile : MonoBehaviour
{
    [SerializeField] private WFC_OnTileSO tileInformation;
    public List<WFC_GridCreation.TileTypes> possibleTiles;

    public int xID { get; private set; }
    public int yID { get; private set; }
    public float entropy { get; private set; }

    public bool solved = false;
    public bool ready = false;


    /// <summary>
    /// Sets the xID and yID based on the input of X and Y
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetXY(int x, int y)
    {
        xID = x;
        yID = y;
    }

    /// <summary>
    /// Sets this tile to the WFC_OnTileSO that is passed in, then it is counted as solved.
    /// </summary>
    /// <param name="tileInfo"></param>
    public void SetTile(WFC_OnTileSO tileInfo)
    {
        tileInformation = tileInfo;
        GetComponent<MeshRenderer>().material.color = tileInformation.color;
        if(tileInfo.typeOfTile != WFC_GridCreation.TileTypes.BLANK) solved = true;
        if(solved) possibleTiles.Clear();
    }

    /// <summary>
    /// Removes a possible tile from the possible tile list.
    /// </summary>
    /// <param name="possTile"></param>
    public void RemovePossibleTile(WFC_GridCreation.TileTypes possTile)
    {
        possibleTiles.Remove(possTile);
        entropy = 1f - Mathf.InverseLerp(0, 5, possibleTiles.Count);
    }
    
    /// <summary>
    /// Adds the passed TileTpe to the list of possible tiles that this tile can be
    /// </summary>
    /// <param name="possTile"></param>
    public void AddPossibleTile(WFC_GridCreation.TileTypes possTile)
    {
        possibleTiles.Add(possTile);
    }

    public List<WFC_GridCreation.TileTypes> GetAllEastTiles()
    {
        
        return tileInformation.allowedEastTiles;
    }
    public List<WFC_GridCreation.TileTypes> GetAllSouthTiles()
    {
        
        return tileInformation.allowedSouthTiles;
    }
    public List<WFC_GridCreation.TileTypes> GetAllWestTiles()
    {
        
        return tileInformation.allowedWestTiles;
    }
    public List<WFC_GridCreation.TileTypes> GetAllNorthTiles()
    {
        
        return tileInformation.allowedNorthTiles;
    }

}
