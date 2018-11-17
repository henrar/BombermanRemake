using Godot;
using System;
using System.Collections.Generic;

public class Enemy : KinematicBody2D {
    public Vector2 currentPositionOnTileMap;
    private readonly int moveModifier = 200;
    private Vector2 currentDestination;
    private bool reachedDestination;
    private TileMap tileMap;
    private List<Vector2> path;
    private Navigation2D navigation;

    public override void _Ready() {
        this.currentDestination = new Vector2();
        this.reachedDestination = true;
        this.tileMap = GetTree().GetRoot().GetNode("World/Nav/TileMap") as TileMap;
        this.navigation = GetTree().GetRoot().GetNode("World/Nav") as Navigation2D;
        updatePositionOnTileMap();
    }

    public override void _Process(float delta) {
        if (this.reachedDestination) {
            this.currentDestination = this.tileMap.getRandomGrassCell();
            Console.WriteLine("Setting destination at " + this.currentDestination);
            updatePath();
            this.reachedDestination = false;
        }

        updatePositionOnTileMap();

        if (this.path != null && this.path.Count > 1) {
            float distance = GetPosition().DistanceTo(this.path[0]);
            if (distance > 2) {
                SetPosition(GetPosition().LinearInterpolate(this.path[0], (this.moveModifier * delta) / distance));
            } else {
                this.path.RemoveAt(0);
            }
        } else {
            this.reachedDestination = true;
        }

        if (this.path == null || this.path.Count == 0 || this.currentDestination == this.tileMap.WorldToMap(this.GetPosition())) {
            this.reachedDestination = true;
        }
    }

    private void updatePath() {
        Vector2[] path = this.navigation.GetSimplePath(this.GetPosition(), this.tileMap.MapToWorld(this.currentDestination), false);
        this.path = new List<Vector2>(path);
    }

    public void kill() {
        this.QueueFree();
    }

    private void updatePositionOnTileMap() {
        this.currentPositionOnTileMap = this.tileMap.WorldToMap(this.GetPosition());
        this.tileMap.enemyOnCell[this] = this.currentPositionOnTileMap;
    }
}
