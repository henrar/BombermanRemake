using Godot;
using System;

public static class Directions {
    public static readonly Vector2 directionUp = new Vector2(0, -1);
    public static readonly Vector2 directionDown = new Vector2(0, 1);
    public static readonly Vector2 directionRight = new Vector2(1, 0);
    public static readonly Vector2 directionLeft = new Vector2(-1, 0);

    private static Random random = new Random();

    public static readonly Godot.Array<Vector2> directionsArray = new Godot.Array<Vector2> {
        directionUp, directionDown, directionRight, directionLeft
    };

    public static Vector2 GetRandomDirection() {
        int index = random.Next(0, directionsArray.Count);
        return directionsArray[index];
    }
}

