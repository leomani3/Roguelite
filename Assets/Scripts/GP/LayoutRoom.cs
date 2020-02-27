using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this class' goal is to store data while making the room layout of the Level (first step of the level generation)
/// </summary>
public class LayoutRoom
{
    private Vector2Int position; //Grid position of trhe room (not world position)
    private int roomType; //0 room normal, 1 boss room
    private int distance;
    private int nbNeighbors = 0;

    #region constructors
    public LayoutRoom(Vector2Int pos, int RoomType)
    {
        position = pos;
        roomType = RoomType;
        distance = -1;
    }

    public LayoutRoom() { }
    #endregion

    #region getter setters
    public Vector2Int Position
    {
        get { return position; }
    }

    public int RoomType
    {
        get { return roomType; }
        set { roomType = value; }
    }

    public int Distance
    {
        get { return distance; }
        set { distance = value; }
    }

    public int NbNeighbors
    {
        get { return nbNeighbors; }
        set { nbNeighbors = value; }
    }
    #endregion


    public void UpdateNeighbors(List<LayoutRoom> spawnedRooms)
    {
        int nb = 0;
        
        if (spawnedRooms.Exists(r => r.Position == position + Vector2Int.up))
            nb++;
        if (spawnedRooms.Exists(r => r.Position == position + Vector2Int.right))
            nb++;
        if (spawnedRooms.Exists(r => r.Position == position + Vector2Int.down))
            nb++;
        if (spawnedRooms.Exists(r => r.Position == position + Vector2Int.left))
            nb++;
            
        nbNeighbors = nb;
    }
}
