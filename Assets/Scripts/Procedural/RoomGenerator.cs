using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomGenerator
{

    List<RoomNode> allSpaceNodes = new List<RoomNode> ();
    private int roomWidth;
    private int roomLength;

    public RoomGenerator(int roomWidth, int roomLength)
    {
        this.roomWidth = roomWidth;
        this.roomLength = roomLength;
    }

    public List<Node> CalculateRooms(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, int corridorWidth)
    {
        BinarySpacePartitioner bsp = new BinarySpacePartitioner(roomWidth, roomLength);
        allSpaceNodes = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);

        RoomGen roomGen = new RoomGen(maxIterations, roomLengthMin, roomWidthMin);
        List<RoomNode> roomList = roomGen.GenrateRoomsInGivenSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerModifier, roomOffset);

        CorridorsGenerator corridorsGenerator = new CorridorsGenerator();
        var corridorList = corridorsGenerator.CreateCorridor(allSpaceNodes, corridorWidth);

        return new List<Node>(roomList).Concat(corridorList).ToList();
    }


}
