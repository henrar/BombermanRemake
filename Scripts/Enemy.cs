using Godot;
using System;

public class Enemy : KinematicBody2D {
    private readonly int moveModifier = 200;

    public override void _Ready() {

    }

    public override void _Process(float delta) {
        updatePositionOnTileMap();
    }

    public void kill() {
        this.QueueFree();
    }
   
    private void updatePositionOnTileMap() {
        TileMap map = GetTree().GetRoot().GetNode("World/Nav/TileMap") as TileMap;
        Vector2 position = map.WorldToMap(this.GetPosition());
        map.enemyOnCell[this] = position;
    }
}
