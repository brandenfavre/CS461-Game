/* ORIGINAL CODE BY https://github.com/BadNidalee
https://www.youtube.com/watch?v=QaryeJsjrI8 explaining how the code works. */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    public string nextScene;
    [SerializeField]
    private Tile groundTile;
    [SerializeField]
    private Tile pitTile;
    [SerializeField]
    private Tile topWallTile;
    [SerializeField]
    private Tile botWallTile;
    [SerializeField]
    private Tile leftWallTile;
    [SerializeField]
    private Tile rightWallTile;
    [SerializeField]
    private Tile botLeftWallTile;
    [SerializeField]
    private Tile botRightWallTile;
    [SerializeField]
    private Tile topLeftWallTile;
    [SerializeField]
    private Tile topRightWallTile;
    [SerializeField]
    private Tile exitTile;
    [SerializeField]
    private Tile[] decorations;
    [SerializeField]
    private Tilemap groundMap;
    [SerializeField]
    private Tilemap decorationMap;
    [SerializeField]
    private Tilemap pitMap;
    [SerializeField]
    private Tilemap wallMap;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject NPC;
    [SerializeField]
    private GameObject[] enemies;
    private int enemyRange;
    [SerializeField]
    private ExitLevel exit;
    [SerializeField]
    private int deviationRate = 10;
    [SerializeField]
    private int roomRate = 15;
    [SerializeField]
    private int maxRouteLength;
    [SerializeField]
    private int maxRoutes = 20;
    [SerializeField]
    private int enemySpawnChance = 1;
    [SerializeField]
    private int decorationSpawnChance = 10;

    private int routeCount = 0;

    private void Start()
    {
        int x = 0;
        int y = 0;
        int routeLength = 0;
        enemyRange = enemies.Length;
        GenerateSquare(x, y, 1);
        Vector2Int previousPos = new Vector2Int(x, y);
        y += 3;
        GenerateSquare(x, y, 1);
        NewRoute(x, y, routeLength, previousPos);

        FillWalls();


        Vector3Int playerSpawnPoint = FindLowestPoint() + Vector3Int.up + Vector3Int.right;
        Instantiate(player, groundMap.GetCellCenterWorld(playerSpawnPoint), Quaternion.identity);
        Instantiate(NPC, groundMap.GetCellCenterWorld(playerSpawnPoint), Quaternion.identity);
        player.transform.position = playerSpawnPoint;
        NPC.transform.position = playerSpawnPoint; // Should be offset but eh

        Vector3Int exitPoint = FindFarthestPoint(playerSpawnPoint);
        exitPoint = new Vector3Int(exitPoint.x, exitPoint.y + 1, exitPoint.z);
        Instantiate(exit, groundMap.GetCellCenterWorld(exitPoint), Quaternion.identity);
        exit.nextLevel = nextScene; // Doesn't work for some reason?
        wallMap.SetTile(exitPoint, exitTile);
    }

    private void FillWalls()
    {
        BoundsInt bounds = groundMap.cellBounds;
        for (int xMap = bounds.xMin - 10; xMap <= bounds.xMax + 10; xMap++)
        {
            for (int yMap = bounds.yMin - 10; yMap <= bounds.yMax + 10; yMap++)
            {
                Vector3Int pos = new Vector3Int(xMap, yMap, 0);
                Vector3Int posBelow = new Vector3Int(xMap, yMap - 1, 0);
                Vector3Int posAbove = new Vector3Int(xMap, yMap + 1, 0);
                Vector3Int posLeft = new Vector3Int(xMap - 1, yMap, 0);
                Vector3Int posRight = new Vector3Int(xMap + 1, yMap, 0);
                Vector3Int posUL = new Vector3Int(xMap - 1, yMap + 1, 0); // Upper left
                Vector3Int posUR = new Vector3Int(xMap + 1, yMap + 1, 0); // Upper right
                Vector3Int posBL = new Vector3Int(xMap - 1, yMap - 1, 0); // Bottom left
                Vector3Int posBR = new Vector3Int(xMap + 1, yMap - 1, 0); // Bottom right
                TileBase tile = groundMap.GetTile(pos);
                TileBase wall = wallMap.GetTile(pos); ////////////maybe remove
                TileBase tileBelow = groundMap.GetTile(posBelow);
                TileBase tileAbove = groundMap.GetTile(posAbove);
                TileBase tileLeft = groundMap.GetTile(posLeft);
                TileBase tileRight = groundMap.GetTile(posRight);
                TileBase tileUL = groundMap.GetTile(posUL);
                TileBase tileUR = groundMap.GetTile(posUR);
                TileBase tileBL = groundMap.GetTile(posBL);
                TileBase tileBR = groundMap.GetTile(posBR);

                if (tile == null)
                {
                    pitMap.SetTile(pos, pitTile); // fill void
                   if (tileBelow != null) // place tile at pos, above
                    {
                        wallMap.SetTile(pos, topWallTile);
                    }
                    else if (tileLeft != null) // place tile at pos, right
                    {
                        pitMap.SetTile(pos, groundTile); // set groundtile under wallTile
                        if (tileAbove != null)
                        {
                            wallMap.SetTile(pos, topLeftWallTile);
                        }
                        else
                        {
                            wallMap.SetTile(pos, rightWallTile);
                        }
                    }
                    else if (tileRight != null) // place tile at pos, left
                    {
                        pitMap.SetTile(pos, groundTile);
                        if (tileAbove != null)
                        {
                            wallMap.SetTile(pos, topRightWallTile);
                        }
                        else
                        {
                            wallMap.SetTile(pos, leftWallTile);
                        }
                    }
                    else if (tileAbove != null) // place tile at pos, below
                    {
                        //pitMap.SetTile(pos, groundTile);
                        wallMap.SetTile(pos, botWallTile);
                    }
                }
                else
                {
                    pitMap.SetTile(pos, pitTile);
                }
                if (tileBelow == null && tileBL == null && tileLeft == null && tileUL == null &&
                        tileAbove == null && tileUR == null && tileRight == null && tileBR != null) // top left wall
                {
                    wallMap.SetTile(pos, topLeftWallTile);
                }
                else if (tileBelow == null && tileBL != null && tileLeft == null && tileUL == null &&
                    tileAbove == null && tileUR == null && tileRight == null && tileBR == null) // top right wall
                {
                    wallMap.SetTile(pos, topRightWallTile);
                }
                else if (tileBelow == null && tileBL == null && tileLeft == null && tileUL == null &&
                    tileAbove == null && tileUR != null && tileRight == null && tileBR == null) // bottom left wall
                {
                    wallMap.SetTile(pos, botLeftWallTile);
                }
                else if (tileBelow == null && tileBL == null && tileLeft == null && tileUL != null &&
                    tileAbove == null && tileUR == null && tileRight == null && tileBR == null) // bottom right wall
                {
                    wallMap.SetTile(pos, botRightWallTile);
                }
                else if (tileRight != null && tileBR != null && tileBelow != null 
                    && tileAbove == null && tileLeft == null && tileUL == null && tile == null) // inside room bottom right walls
                {
                    wallMap.SetTile(pos, botRightWallTile);                    // could possibly move this and bottom left walls
                    pitMap.SetTile(pos, groundTile);                          // to where top right/left walls are being initialized
                }                                                             // not same if statement, but their respective ones
                else if (tileLeft != null && tileBL != null && tileBelow != null
                    && tileRight == null && tileAbove == null && tileUR == null && tile == null) // inside room bottom left walls
                {
                    wallMap.SetTile(pos, botLeftWallTile);
                    pitMap.SetTile(pos, groundTile);
                }

            }
        }
    }

    private void NewRoute(int x, int y, int routeLength, Vector2Int previousPos)
    {
        if (routeCount < maxRoutes)
        {
            routeCount++;
            while (++routeLength < maxRouteLength)
            {
                //Initialize
                bool routeUsed = false;
                int xOffset = x - previousPos.x; //0
                int yOffset = y - previousPos.y; //3
                int roomSize = 1; //Hallway size
                // Generate room
                if (Random.Range(1, 100) <= roomRate)
                    roomSize = Random.Range(3, 6);
                previousPos = new Vector2Int(x, y);

                //Go Straight
                if (Random.Range(1, 100) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateSquare(previousPos.x + xOffset, previousPos.y + yOffset, roomSize);
                        NewRoute(previousPos.x + xOffset, previousPos.y + yOffset, Random.Range(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        x = previousPos.x + xOffset;
                        y = previousPos.y + yOffset;
                        GenerateSquare(x, y, roomSize);
                        routeUsed = true;
                    }
                }

                //Go left
                if (Random.Range(1, 100) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateSquare(previousPos.x - yOffset, previousPos.y + xOffset, roomSize);
                        NewRoute(previousPos.x - yOffset, previousPos.y + xOffset, Random.Range(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        y = previousPos.y + xOffset;
                        x = previousPos.x - yOffset;
                        GenerateSquare(x, y, roomSize);
                        routeUsed = true;
                    }
                }
                //Go right
                if (Random.Range(1, 100) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateSquare(previousPos.x + yOffset, previousPos.y - xOffset, roomSize);
                        NewRoute(previousPos.x + yOffset, previousPos.y - xOffset, Random.Range(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        y = previousPos.y - xOffset;
                        x = previousPos.x + yOffset;
                        GenerateSquare(x, y, roomSize);
                        routeUsed = true;
                    }
                }

                if (!routeUsed)
                {
                    x = previousPos.x + xOffset;
                    y = previousPos.y + yOffset;
                    GenerateSquare(x, y, roomSize);
                }
            }
        }
    }

    private void GenerateSquare(int x, int y, int radius)
    {
        bool isRoom = false;
        if (radius > 3)
        {
            isRoom = true;
        }
        for (int tileX = x - radius; tileX <= x + radius; tileX++)
        {
            for (int tileY = y - radius; tileY <= y + radius; tileY++)
            {
                Vector3Int tilePos = new Vector3Int(tileX, tileY, 0);
                groundMap.SetTile(tilePos, groundTile);
                // Spawn decorations
                if (Random.Range(1,101) <= decorationSpawnChance) // x% chance to spawn decoration on this tile
                {
                    int adjustedX = tileX * 2 + 1;
                    int adjustedY = tileY * 2 + 1;
                    Vector3Int decorationPosition = new Vector3Int(adjustedX, adjustedY, 0);
                    decorationMap.SetTile(decorationPosition, decorations[Random.Range(0, decorations.Length)]);
                }
                // Spawn enemies
                if (isRoom && Random.Range(1, 101) <= enemySpawnChance) // x% chance to spawn monster on this tile
                {
                    Instantiate(enemies[Random.Range(0, enemyRange)], groundMap.GetCellCenterWorld(tilePos), Quaternion.identity);
                    // Convert grid position to world space coordinates
                }
            }
        }
    }

    private Vector3Int FindLowestPoint()
    {
        BoundsInt bounds = groundMap.cellBounds;
        int lowestY = int.MaxValue;
        Vector3Int lowestPoint = Vector3Int.zero;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = groundMap.GetTile(tilePos);

                // Check if the tile is not null and its y-coordinate is lower than the current lowest
                if (tile != null && tilePos.y < lowestY)
                {
                    lowestY = tilePos.y;
                    lowestPoint = tilePos;
                }
            }
        }
        return lowestPoint;
    }
    private Vector3Int FindFarthestPoint(Vector3Int fromPoint)
    {
        BoundsInt bounds = groundMap.cellBounds;
        float maxDistance = 0;
        Vector3Int farthestPoint = Vector3Int.zero;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = groundMap.GetTile(tilePos);

                // Check if the tile is not null
                if (tile != null)
                {
                    // Calculate distance from the spawn point
                    float distance = Vector3Int.Distance(fromPoint, tilePos);

                    // Update farthest point if this tile is farther away
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        farthestPoint = tilePos;
                    }
                }
            }
        }

        return farthestPoint;
    }
}
