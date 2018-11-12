using Godot;
using System;

public enum TileType {
    Tile_Empty = 0,
    Tile_Destructible = 1,
    Tile_Wall = 2,
}

public class MapTile : Node {
    public TileType type;
    public int posX;
    public int posY;

    public override void _Ready() {

    }

    public override void _Process(float delta) {

    }
}
