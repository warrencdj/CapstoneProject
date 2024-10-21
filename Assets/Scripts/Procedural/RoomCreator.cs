using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Vector3 playerSpawnPosition;

    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;

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

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }

        CreateWalls(wallParent);

        // Spawn the player in a random room
        playerSpawnPosition = SpawnPlayerInRandomRoom(listOfRooms);

        // Spawn monsters in other rooms
        SpawnMonstersInOtherRooms(listOfRooms);
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
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[] { topLeftV, topRightV, bottomLeftV, bottomRightV };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject roomFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        roomFloor.transform.position = Vector3.zero;
        roomFloor.transform.localScale = Vector3.one;
        roomFloor.GetComponent<MeshFilter>().mesh = mesh;
        roomFloor.GetComponent<MeshRenderer>().material = material;

        // Add a BoxCollider to the room floor
        BoxCollider boxCollider = roomFloor.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(topRightCorner.x - bottomLeftCorner.x, 0.1f, topRightCorner.y - bottomLeftCorner.y); // Set height to 0.1f
        boxCollider.center = new Vector3((bottomLeftCorner.x + topRightCorner.x) / 2, 0.05f, (bottomLeftCorner.y + topRightCorner.y) / 2); // Center slightly above the ground

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
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
        // Filter for RoomNode types only
        List<RoomNode> roomNodes = new List<RoomNode>();
        foreach (Node node in listOfRooms)
        {
            if (node is RoomNode roomNode) // Safe casting
            {
                roomNodes.Add(roomNode);
            }
        }

        if (roomNodes.Count > 0)
        {
            // Randomly select a room node
            RoomNode randomRoom = roomNodes[UnityEngine.Random.Range(0, roomNodes.Count)];

            // Calculate spawn position (using the center of the room)
            Vector3 spawnPosition = new Vector3(
                (randomRoom.BottomLeftAreaCorner.x + randomRoom.TopRightAreaCorner.x) / 2,
                1.0f, // Adjust this value based on your player model's height
                (randomRoom.BottomLeftAreaCorner.y + randomRoom.TopRightAreaCorner.y) / 2
            );

            // Move the existing player to the spawn position
            GameObject playerInstance = GameObject.FindGameObjectWithTag("Player");
            if (playerInstance != null)
            {
                playerInstance.transform.position = spawnPosition; // Move the player to the spawn position
            }
            else
            {
                Debug.LogWarning("Player not found! Make sure the player is tagged as 'Player' and exists in the scene.");
            }

            return spawnPosition; // Return the player's spawn position
        }
        else
        {
            Debug.LogWarning("No RoomNodes available to spawn the player.");
            return Vector3.zero; // Return a default position if no rooms are available
        }
    }

    private void SpawnMonstersInOtherRooms(List<Node> listOfRooms)
    {
        foreach (var room in listOfRooms)
        {
            // Calculate the room boundaries
            Vector2 bottomLeft = room.BottomLeftAreaCorner;
            Vector2 topRight = room.TopRightAreaCorner;

            // Randomly generate a position within the room's boundaries
            Vector3 randomPosition = new Vector3(
                UnityEngine.Random.Range(bottomLeft.x, topRight.x),
                1.0f, // Adjust height if necessary
                UnityEngine.Random.Range(bottomLeft.y, topRight.y)
            );

            // Check if the distance from the player is greater than the minimum distance
            if (Vector3.Distance(randomPosition, playerSpawnPosition) > minimumMonsterDistance)
            {
                GameObject monsterInstance = Instantiate(monsterPrefab, randomPosition, Quaternion.identity);

                // Get the Enemy component and set the player reference
                Enemy enemy = monsterInstance.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.player = GameObject.FindGameObjectWithTag("Player").transform; // Assuming your player has a "Player" tag
                }
            }
        }
    }
}
