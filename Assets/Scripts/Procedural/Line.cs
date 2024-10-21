using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{

    Orientation orientation;
    Vector2Int coodinates;

    public Line(Orientation orientation, Vector2Int coodinates)
    {
        this.orientation = orientation;
        this.coodinates = coodinates;
    }

    public Orientation Orientation {  get => orientation; set => orientation = value; }
    public Vector2Int Coodinates { get => coodinates; set => coodinates = value; }

}

public enum Orientation
{
    Horizontal = 0,
    Vertical = 1
}
