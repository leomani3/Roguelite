using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    [Header("Layout")]
    public int nbRoom;
    public Vector2Int mapSize;
    [Range(0.0f, 1.0f)]
    public float verticalityRatio;
    [Range(0.0f, 1.0f)]
    public float chanceDoubleVertical;
    [Range(0.0f, 1.0f)]
    public float randomessRatio;
    public int nbShopRoom;
    public int nbChestRoom;
    public int nbBossRoom;
    public int nbLevelUpRoom;

    [Space(10)]
    [Header("Placement des prefabs")]
    [Tooltip("roomSpace correspond à la distance entre deux room. aka la taille d'une room")]
    public float roomSpace;
    public List<GameObject> T;
    public List<GameObject> R;
    public List<GameObject> B;
    public List<GameObject> L;
    public List<GameObject> TR;
    public List<GameObject> TB;
    public List<GameObject> TL;
    public List<GameObject> RL;
    public List<GameObject> BL;
    public List<GameObject> RBL;
    public List<GameObject> TRB;
    public List<GameObject> TBL;
    public List<GameObject> TRL;
    public List<GameObject> TRBL;

    //2D array of the room layout
    private LayoutRoom[,] rooms;
    private List<LayoutRoom> spawnedRooms;
    private Vector2Int mapCenter;

    private int cpt;

    public void GenerateLayout()
    {
        Init();

        LayoutRoom firstRoom = new LayoutRoom(mapCenter, 0);
        SpawnRoom(firstRoom);

        int nbRoomToSpawn = Mathf.RoundToInt(nbRoom * (1 - randomessRatio));
        int nbRdmSoloRoomAdded = Mathf.RoundToInt(nbRoom * randomessRatio);

        //première boucle pour faire spawn le layout général
        while (spawnedRooms.Count < nbRoomToSpawn && cpt < 1000)
        {
            LayoutRoom chosenRoom = PickRandomRoom();

            float rdmBranch = Random.Range(0.0f, 1.0f);

            if (rdmBranch > 1 - verticalityRatio) //branch
            {
                ExpandVertically(chosenRoom, true);
            }
            else //expand normally
            {
                ExpandHorizontally(chosenRoom);
            }

            cpt++;
        }

        //seconde boucle pour mettre des room random par si par là
        cpt = 0;

        while (spawnedRooms.Count < nbRoomToSpawn + nbRdmSoloRoomAdded && cpt < 1000)
        {
            CreateRoom1Neighbor(true);

            cpt++;
        }

        DisplayLayout();
    }

    public void PutRoomOfInterest()
    {
        //choix de la room de boss
        LayoutRoom bossRoom = FindFurthestRoom();
        bossRoom.RoomType = 1;
        Debug.Log("Boss Room placée en : " + bossRoom.Position);
    }



    public void PlaceRoomPrefabs()
    {
        foreach (LayoutRoom room in spawnedRooms)
        {
            bool[] neighbors = ScanNeighborsAtPos(room.Position);
            string neighborComposition = "";

            if (neighbors[0] == true) neighborComposition += 'T';
            if (neighbors[1] == true) neighborComposition += 'R';
            if (neighbors[2] == true) neighborComposition += 'B';
            if (neighbors[3] == true) neighborComposition += 'L';

            GameObject chosenPrefab = null;
            switch (neighborComposition)
            {
                case "T":
                    chosenPrefab = T[Random.Range(0, T.Count)];
                    break;
                case "R":
                    chosenPrefab = R[Random.Range(0, R.Count)];
                    break;
                case "B":
                    chosenPrefab = B[Random.Range(0, B.Count)];
                    break;
                case "L":
                    chosenPrefab = L[Random.Range(0, L.Count)];
                    break;
                case "TR":
                    chosenPrefab = TR[Random.Range(0, TR.Count)];
                    break;
                case "TB":
                    chosenPrefab = TB[Random.Range(0, TB.Count)];
                    break;
                case "TL":
                    chosenPrefab = TL[Random.Range(0, TL.Count)];
                    break;
                case "RL":
                    chosenPrefab = RL[Random.Range(0, RL.Count)];
                    break;
                case "BL":
                    chosenPrefab = BL[Random.Range(0, BL.Count)];
                    break;
                case "TRB":
                    chosenPrefab = TRB [Random.Range(0, TRB.Count)];
                    break;
                case "TRL":
                    chosenPrefab = TRL[Random.Range(0, TRL.Count)];
                    break;
                case "TBL":
                    chosenPrefab = TBL[Random.Range(0, TBL.Count)];
                    break;
                case "RBL":
                    chosenPrefab = RBL[Random.Range(0, RBL.Count)];
                    break;
                case "TRBL":
                    chosenPrefab = TRBL[Random.Range(0, TRBL.Count)];
                    break;
                default:
                    Debug.Log("Le string est mal construit");
                    break;
            }
            Instantiate(chosenPrefab, new Vector3(room.Position.x * roomSpace, room.Position.y * roomSpace, 0), Quaternion.identity);

        }
    }








    /// <summary>
    /// parcours toutes les room et retourne la plus éloignée. Si il y en a plusieurs, retourne la première occurence.
    /// </summary>
    /// <returns></returns>
    public LayoutRoom FindFurthestRoom()
    {
        LayoutRoom furthestRoom = null;
        int currentMaxDistance = -1;

        foreach (LayoutRoom room in spawnedRooms)
        {
            if (furthestRoom == null || room.Distance > currentMaxDistance)
            {
                furthestRoom = room;
                currentMaxDistance = room.Distance;
            }
        }

        return furthestRoom;
    }



    public void Dijkstra()
    {
        List<LayoutRoom> roomsToExplore = new List<LayoutRoom>();

        LayoutRoom firstRoom = spawnedRooms[0];
        firstRoom.Distance = 0;
        roomsToExplore.Add(firstRoom);
        SetCircleValue(firstRoom.Position, firstRoom.Distance);

        int i = 0;
        while (roomsToExplore.Count > 0)
        {
            List<LayoutRoom> loopRoomsToExplore = new List<LayoutRoom>(roomsToExplore);
            foreach (LayoutRoom room in loopRoomsToExplore)
            {
                List<LayoutRoom> neighbors = GetNeighborsAtPos(room.Position);
                List<LayoutRoom> addedRooms = new List<LayoutRoom>();

                foreach (LayoutRoom neighbor in neighbors)
                {
                    if (neighbor.Distance == -1 || room.Distance + 1 < neighbor.Distance)
                    {
                        neighbor.Distance = room.Distance + 1;
                        addedRooms.Add(neighbor);
                        SetCircleValue(neighbor.Position, neighbor.Distance);
                    }
                }
                roomsToExplore.AddRange(addedRooms);
                roomsToExplore.Remove(room);
            }
            i++;
        }
    }


    public void Init()
    {
        cpt = 0;
        spawnedRooms = new List<LayoutRoom>();
        rooms = new LayoutRoom[mapSize.x, mapSize.y];
        mapCenter = new Vector2Int(Mathf.RoundToInt(mapSize.x / 2), Mathf.RoundToInt(mapSize.y / 2));
    }

    public void ExpandVertically(LayoutRoom room, bool allowDoubleSpawn)
    {
        //on détecte les free slots autours
        List<Vector2Int> possiblePos = GetEmptyValidSpotsAtPos(room.Position, false, true);

        //on fait spawn des rooms
        int rdm;

        if (possiblePos.Count == 1)
            rdm = Random.Range(0, possiblePos.Count + 1);
        else
            rdm = Random.Range(0, possiblePos.Count);


        for (int i = 0; i < rdm; i++)
        {
            LayoutRoom spawnedRoom = new LayoutRoom(possiblePos[Random.Range(0, possiblePos.Count)], 0);
            SpawnRoom(spawnedRoom);

            if (allowDoubleSpawn && Random.Range(0.0f, 1.0f) > 1 - chanceDoubleVertical)
            {
                ExpandVertically(spawnedRoom, false);
            }

        }
    }

    public void ExpandHorizontally(LayoutRoom room)
    {
        //on détecte les free slots autours
        List<Vector2Int> possiblePos = GetEmptyValidSpotsAtPos(room.Position, true, false);

        //on fait spawn des rooms
        int rdm;

        if (possiblePos.Count == 1)
            rdm = Random.Range(0, possiblePos.Count + 1);
        else
            rdm = Random.Range(0, possiblePos.Count);


        for (int i = 0; i < rdm; i++)
        {
            SpawnRoom(new LayoutRoom(possiblePos[Random.Range(0, possiblePos.Count)], 0));
        }
    }

    public void Expand(LayoutRoom room)
    {
        //on détecte les free slots autours
        List<Vector2Int> possiblePos = GetEmptyValidSpotsAtPos(room.Position, true, true);

        //on fait spawn des rooms
        int rdm;

        if (possiblePos.Count == 1)
            rdm = Random.Range(0, possiblePos.Count + 1);
        else
            rdm = Random.Range(0, possiblePos.Count);


        for (int i = 0; i < rdm; i++)
        {
           SpawnRoom(new LayoutRoom(possiblePos[Random.Range(0, possiblePos.Count)], 0));
        }
    }








    /// <summary> 
    /// returns a random room from the spawned rooms
    /// </summary>
    /// <returns></returns>
    public LayoutRoom PickRandomRoom()
    {
        return spawnedRooms[Random.Range(0, spawnedRooms.Count)];
    }


    public LayoutRoom PickLeastNeighborRoom()
    {
        List<LayoutRoom> sortedSpawnedRoom = spawnedRooms;
        sortedSpawnedRoom.Sort(SortByNeighborCount);

        return sortedSpawnedRoom[0];
    }

    public int SortByNeighborCount(LayoutRoom r1, LayoutRoom r2)
    {
        return r1.NbNeighbors.CompareTo(r2.NbNeighbors);
    }









    /// <summary>
    /// returns le list of rooms that has only one neighbor
    /// </summary>
    /// <returns></returns>
    public List<LayoutRoom> GetRooms1Neighbor()
    {
        List<LayoutRoom> rooms = new List<LayoutRoom>();

        foreach (LayoutRoom room in spawnedRooms)
        {
            if (room.NbNeighbors == 1) { rooms.Add(room); }
        }
        return rooms;
    }

    /// <summary>
    /// Create a room that has only one neighbor
    /// </summary>
    public void CreateRoom1Neighbor(bool canAlterate)
    {
        bool done = false;
        int cptInfinite = 0;
        int cptTries = 0;

        while (!done)
        {
            LayoutRoom chosenRoom = PickRandomRoom();
            //on préfère faire spawn une nouvelle room de façon à ne pas transformer une room qui a déjà qu'un seul voisin en une qui en a deux
            if (!canAlterate && cptTries < 100) 
            {
                if (chosenRoom.NbNeighbors > 1)
                {
                    List<Vector2Int> freeSlots = GetEmptyValidSpotsAtPos(chosenRoom.Position, true, true);
                    if (freeSlots.Count > 0)
                    {
                        Vector2Int spawnPosition = freeSlots[Random.Range(0, freeSlots.Count)];
                        if (CountNeighborsAtPos(spawnPosition) == 1)
                        {
                            SpawnRoom(new LayoutRoom(spawnPosition, 0));
                            done = true;
                        }
                    }
                }
                cptTries++;
            }
            else //Si aucune possibilité de spawn 1 room sans altérer celle qui n'ont déjà qu'un seul voisin, on spawn quand même une pour débloquer des possiblités
            {
                List<Vector2Int> freeSlots = GetEmptyValidSpotsAtPos(chosenRoom.Position, true, true);
                if (freeSlots.Count > 0)
                {
                    Vector2Int spawnPosition = freeSlots[Random.Range(0, freeSlots.Count)];
                    if (CountNeighborsAtPos(spawnPosition) == 1)
                    {
                        SpawnRoom(new LayoutRoom(spawnPosition, 0));
                        done = true;
                    }
                }
            }

            //protection anti boucle infinie de haute technologie
            cptInfinite++;
            if (cptInfinite > 1000)
            {
                Debug.Log("boucle infinie ");
                break;
            }
        }
        DisplayLayout();
    }



    /// <summary>
    /// Add the roon in parameters  to all the necessary lists and update all the rooms' neighbors
    /// </summary>
    /// <param name="room"></param>
    public void SpawnRoom(LayoutRoom room)
    {
        rooms[room.Position.x, room.Position.y] = room;
        spawnedRooms.Add(room);
        UpdateAllRoomsNeighbors();
        SetCircleValue(room.Position, room.Distance);
    }

    /// <summary>
    /// Return if the pos is in bounds of the 2D array
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool IsInBounds(Vector2Int pos)
    {
        return pos.x < mapSize.x && pos.x >= 0 && pos.y < mapSize.y && pos.y >= 0;
    }

    /// <summary>
    /// update NbNeighbors according to the neihbors aroud a room
    /// </summary>
    public void UpdateAllRoomsNeighbors()
    {
        foreach (LayoutRoom room in spawnedRooms)
        {
            room.UpdateNeighbors(spawnedRooms);
        }
    }

    /// <summary>
    /// return all the enpty positions around "pos"
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public List<Vector2Int> GetEmptyValidSpotsAtPos(Vector2Int pos, bool hor, bool vert)
    {
        List<Vector2Int> possiblePos = new List<Vector2Int>();

        Vector2Int up = pos + new Vector2Int(0, 1);
        Vector2Int right = pos + new Vector2Int(1, 0);
        Vector2Int down = pos + new Vector2Int(0, -1);
        Vector2Int left = pos + new Vector2Int(-1, 0);

        //on détecte les free slots autours
        if (hor)
        {
            if (IsInBounds(right) && rooms[right.x, right.y] == null) { possiblePos.Add(right); }
            if (IsInBounds(left) && rooms[left.x, left.y] == null) { possiblePos.Add(left); }
        }
        if (vert)
        {
            if (IsInBounds(up) && rooms[up.x, up.y] == null) { possiblePos.Add(up); }
            if (IsInBounds(down) && rooms[down.x, down.y] == null) { possiblePos.Add(down); }
        }

        return possiblePos;
    }

    /// <summary>
    /// return the number of room in the neighborhood of "pos"
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public int CountNeighborsAtPos(Vector2Int pos)
    {
        int nb = 0;

        Vector2Int up = pos + new Vector2Int(0, 1);
        Vector2Int right = pos + new Vector2Int(1, 0);
        Vector2Int down = pos + new Vector2Int(0, -1);
        Vector2Int left = pos + new Vector2Int(-1, 0);

        if (spawnedRooms.Exists(r => r.Position == up)) { nb++; }
        if (spawnedRooms.Exists(r => r.Position == right)) { nb++; }
        if (spawnedRooms.Exists(r => r.Position == down)) { nb++; }
        if (spawnedRooms.Exists(r => r.Position == left)) { nb++; }

        return nb;
    }

    public List<LayoutRoom> GetNeighborsAtPos(Vector2Int pos)
    {
        List<LayoutRoom> neighbors = new List<LayoutRoom>();

        Vector2Int up = pos + new Vector2Int(0, 1);
        Vector2Int right = pos + new Vector2Int(1, 0);
        Vector2Int down = pos + new Vector2Int(0, -1);
        Vector2Int left = pos + new Vector2Int(-1, 0);

        if (spawnedRooms.Exists(r => r.Position == up)) { neighbors.Add(spawnedRooms.Find(r => r.Position == up)); }
        if (spawnedRooms.Exists(r => r.Position == right)) { neighbors.Add(spawnedRooms.Find(r => r.Position == right)); }
        if (spawnedRooms.Exists(r => r.Position == down)) { neighbors.Add(spawnedRooms.Find(r => r.Position == down)); }
        if (spawnedRooms.Exists(r => r.Position == left)) { neighbors.Add(spawnedRooms.Find(r => r.Position == left)); }

        return neighbors;
    }

    /// <summary>
    /// retourne un tableau de booléen indiquant s'il y a des voisin en haut, en bas, à droite ou à gauche
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool[] ScanNeighborsAtPos(Vector2Int pos)
    {
        bool[] neighbors = new bool[4] { false, false, false, false};

        Vector2Int up = pos + new Vector2Int(0, 1);
        Vector2Int right = pos + new Vector2Int(1, 0);
        Vector2Int down = pos + new Vector2Int(0, -1);
        Vector2Int left = pos + new Vector2Int(-1, 0);

        if (spawnedRooms.Exists(r => r.Position == up)) { neighbors[0] = true; }
        if (spawnedRooms.Exists(r => r.Position == right)) { neighbors[1] = true; }
        if (spawnedRooms.Exists(r => r.Position == down)) { neighbors[2] = true; }
        if (spawnedRooms.Exists(r => r.Position == left)) { neighbors[3] = true; }

        return neighbors;
    }





    #region Debug
    public void DisplayLayout()
    {
        ClearLayout();

        for (int i = 0; i < rooms.GetLength(0); i++)
        {
            for (int j = 0; j < rooms.GetLength(1); j++)
            {
                if (rooms[i,j] == null)
                {
                    Instantiate(Resources.Load("DebugSquare"), new Vector3(i,j,0), Quaternion.identity);
                }
                else
                {
                    Instantiate(Resources.Load("DebugCircle"), new Vector3(i, j, 0), Quaternion.identity);
                }
            }
        }
    }

    public void ClearLayout()
    {
        GameObject[] circles = GameObject.FindGameObjectsWithTag("DebugCircle");

        foreach (GameObject go in circles)
        {
            DestroyImmediate(go);
        }

        GameObject[] squares = GameObject.FindGameObjectsWithTag("DebugSquare");

        foreach (GameObject go in squares)
        {
            DestroyImmediate(go);
        }
    }

    public void SetCircleValue(Vector2Int pos, int val)
    {
        GameObject[] circles = GameObject.FindGameObjectsWithTag("DebugCircle");

        foreach (GameObject go in circles)
        {
            if (go.transform.position == new Vector3(pos.x, pos.y, 0))
            {
                go.GetComponent<DebugCircle>().value = val;
            }
        }
    }
    #endregion
}
