using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile", menuName = "Tiles/Tile", order = 0)]
public class WFC_OnTileSO : ScriptableObject
{
    public string tileName;
    public string description;

    public WFC_GridCreation.TileTypes typeOfTile;
    
    //Debug Colors
    public Color color;
    
    public List<WFC_GridCreation.TileTypes> allowedNorthTiles;
    public List<WFC_GridCreation.TileTypes> allowedEastTiles;
    public List<WFC_GridCreation.TileTypes> allowedSouthTiles;
    public List<WFC_GridCreation.TileTypes> allowedWestTiles;
}
