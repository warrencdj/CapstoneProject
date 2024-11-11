using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class RoomCreator : MonoBehaviour
{
    public int roomWidth, roomLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public Material material;
    [Range(0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerModifier;
    [Range(0, 2)]
    public int roomOffset;
    public GameObject wallVertical, wallHorizontal;
    public GameObject playerPrefab; // Reference to the player prefab
    public GameObject monsterPrefab; // Reference to the monster prefab
    public float minimumMonsterDistance = 5f; // Minimum distance from the player for monster spawning

    public GameObject[] roomAssets; // Array of asset prefabs to randomly place in rooms
    public int maxAssetsPerRoom = 3; // Maximum number of assets to spawn per room
    public float assetSpawnOffset = 1f; // Offset to avoid wall collisions when spawning assets
    public float floorHeight = 0.1f; // Adjust based on floor thickness

    private Vector3 playerSpawnPosition;

    private List<Vector3Int> possibleDoorVerticalPosition;
    private List<Vector3Int> possibleDoorHorizontalPosition;
    private List<Vector3Int> possibleWallHorizontalPosition;
    private List<Vector3Int> possibleWallVerticalPosition;

    void Start()
    {
        CreateRoom();
    }

    private void CreateRoom()
    {
        RoomGenerator generator = new RoomGenerator(roomWidth, roomLength);
        var listOfRooms = generator.CalculateRooms(maxIterations, roomWidthMin, roomLengthMin, roomBottomCornerModifier, roomTopCornerModifier, roomOffset, corridorWidth);

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;

        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();

        // Generate the room mesh, walls, and spawn assets for each room
        foreach (var room in listOfRooms)
        {
            // Create the mesh for this room
            CreateMesh(room.BottomLeftAreaCorner, room.TopRightAreaCorner);

            // Spawn assets in the current room
            List<Node> singleRoomList = new List<Node> { room }; // Wrap the room in a list
            SpawnAssetsInRooms(singleRoomList); // Spawn assets for the current room only
        }

        CreateWalls(wallParent);
        BuildNavMesh();

        // Spawn player and monsters
        playerSpawnPosition = SpawnPlayerInRandomRoom(listOfRooms);
        SpawnMonstersInOtherRooms(listOfRooms);
    }


    private void BuildNavMesh()
    {
        NavMeshSurface[] surfaces = FindObjectsOfType<NavMeshSurface>();

        if (surfaces.Length == 0)
        {
            NavMeshSurface surface = gameObject.AddComponent<NavMeshSurface>();
            surface.agentTypeID = 0; // Optional: Set agent type ID as needed
            surface.BuildNavMesh(); // Build the NavMesh after room generation
        }
        else
        {
            foreach (var surface in surfaces)
            {
                surface.BuildNavMesh(); // Rebuild existing surfaces if any
            }
        }
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        // Create room floor mesh
        Vector3[] vertices = GetRoomVertices(bottomLeftCorner, topRightCorner);
        Vector2[] uvs = GetRoomUVs(vertices);
        int[] triangles = GetRoomTriangles();

        Mesh mesh = new Mesh { vertices = vertices, uv = uvs, triangles = triangles };
        GameObject roomFloor = CreateRoomFloorObject(mesh, bottomLeftCorner, topRightCorner);

        // Add NavMesh surface to room floor and build
        NavMeshSurface navMeshSurface = roomFloor.AddComponent<NavMeshSurface>();
        navMeshSurface.BuildNavMesh();

        // Add wall positions to the list
        AddWallsToPositionLists(vertices, bottomLeftCorner, topRightCorner);
    }

    private Vector3[] GetRoomVertices(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        return new Vector3[] { topLeftV, topRightV, bottomLeftV, bottomRightV };
    }

    private Vector2[] GetRoomUVs(Vector3[] vertices)
    {
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }
        return uvs;
    }

    private int[] GetRoomTriangles()
    {
        return new int[] { 0, 1, 2, 2, 1, 3 };
    }

    private GameObject CreateRoomFloorObject(Mesh mesh, Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        GameObject roomFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        roomFloor.transform.position = Vector3.zero;
        roomFloor.transform.localScale = Vector3.one;
        roomFloor.GetComponent<MeshFilter>().mesh = mesh;
        roomFloor.GetComponent<MeshRenderer>().material = material;

        // Add BoxCollider with appropriate size
        BoxCollider boxCollider = roomFloor.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(topRightCorner.x - bottomLeftCorner.x, 0.1f, topRightCorner.y - bottomLeftCorner.y);
        boxCollider.center = new Vector3((bottomLeftCorner.x + topRightCorner.x) / 2, 0.05f, (bottomLeftCorner.y + topRightCorner.y) / 2);

        return roomFloor;
    }

    private void AddWallsToPositionLists(Vector3[] vertices, Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        for (int row = (int)bottomLeftCorner.x; row < (int)topRightCorner.x; row++)
        {
            AddWallPositionToList(new Vector3(row, 0, bottomLeftCorner.y), possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
            AddWallPositionToList(new Vector3(row, 0, topRightCorner.y), possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int col = (int)bottomLeftCorner.y; col < (int)topRightCorner.y; col++)
        {
            AddWallPositionToList(new Vector3(bottomLeftCorner.x, 0, col), possibleWallVerticalPosition, possibleDoorVerticalPosition);
            AddWallPositionToList(new Vector3(topRightCorner.x, 0, col), possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private Vector3 SpawnPlayerInRandomRoom(List<Node> listOfRooms)
    {
        List<RoomNode> roomNodes = GetRoomNodes(listOfRooms);

        if (roomNodes.Count > 0)
        {
            RoomNode randomRoom = roomNodes[UnityEngine.Random.Range(0, roomNodes.Count)];
            Vector3 spawnPosition = GetRoomCenter(randomRoom);

            GameObject playerInstance = GameObject.FindGameObjectWithTag("Player");
            if (playerInstance != null)
            {
                playerInstance.transform.position = spawnPosition;
            }
            else
            {
                Debug.LogWarning("Player not found! Ensure player is tagged as 'Player' and exists in the scene.");
            }

            return spawnPosition;
        }
        else
        {
            Debug.LogWarning("No RoomNodes available to spawn the player.");
            return Vector3.zero; // Default position if no room nodes available
        }
    }

    private List<RoomNode> GetRoomNodes(List<Node> listOfRooms)
    {
        List<RoomNode> roomNodes = new List<RoomNode>();
        foreach (Node node in listOfRooms)
        {
            if (node is RoomNode roomNode)
            {
                roomNodes.Add(roomNode);
            }
        }
        return roomNodes;
    }

    private Vector3 GetRoomCenter(RoomNode room)
    {
        return new Vector3(
            (room.BottomLeftAreaCorner.x + room.TopRightAreaCorner.x) / 2,
            1.0f, // Adjust based on player height
            (room.BottomLeftAreaCorner.y + room.TopRightAreaCorner.y) / 2
        );
    }

    private void SpawnMonstersInOtherRooms(List<Node> listOfRooms)
    {
        List<RoomNode> roomNodes = GetRoomNodes(listOfRooms);

        foreach (RoomNode room in roomNodes)
        {
            if (Vector3.Distance(playerSpawnPosition, GetRoomCenter(room)) >= minimumMonsterDistance)
            {
                SpawnMonster(room);
            }
        }
    }

    private void SpawnMonster(RoomNode room)
    {
        Vector3 spawnPosition = GetRoomCenter(room);
        Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
    }

    private void SpawnAssetsInRooms(List<Node> listOfRooms)
    {
        List<RoomNode> roomNodes = GetRoomNodes(listOfRooms);

        foreach (RoomNode room in roomNodes)
        {
            Vector3 roomCenter = GetRoomCenter(room);
            SpawnAssetsInRoom(room, roomCenter);
        }
    }

    private void SpawnAssetsInRoom(RoomNode room, Vector3 roomCenter)
    {
        // Generate random points within the room's area
        List<Vector3> spawnPoints = GetRandomSpawnPoints(room, maxAssetsPerRoom);

        foreach (Vector3 point in spawnPoints)
        {
            // Adjust the Y position to be on top of the floor
            Vector3 spawnPosition = new Vector3(point.x, floorHeight, point.z);

            // Randomly choose an asset to spawn
            GameObject assetPrefab = roomAssets[UnityEngine.Random.Range(0, roomAssets.Length)];
            Instantiate(assetPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private List<Vector3> GetRandomSpawnPoints(RoomNode room, int numberOfPoints)
    {
        List<Vector3> spawnPoints = new List<Vector3>();
        Vector2 bottomLeft = room.BottomLeftAreaCorner;
        Vector2 topRight = room.TopRightAreaCorner;

        for (int i = 0; i < numberOfPoints; i++)
        {
            float xPos = UnityEngine.Random.Range(bottomLeft.x + assetSpawnOffset, topRight.x - assetSpawnOffset);
            float zPos = UnityEngine.Random.Range(bottomLeft.y + assetSpawnOffset, topRight.y - assetSpawnOffset);
            spawnPoints.Add(new Vector3(xPos, 0, zPos));
        }

        return spawnPoints;
    }


}
