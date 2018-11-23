using Godot;
using System;
using System.Collections.Generic;

public class Enemy : KinematicBody2D {
    public Vector2 currentPositionOnTileMap;
    private readonly int moveModifier = 10;
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
        UpdatePositionOnTileMap();
    }

    public override void _PhysicsProcess(float delta) {
        if (this.reachedDestination) {
            this.currentDestination = this.tileMap.GetRandomGrassCell();
            Console.WriteLine("Setting destination at " + this.currentDestination);
            UpdatePath();
            this.reachedDestination = false;
        }

        UpdatePositionOnTileMap();

        if (this.path != null && this.path.Count > 1) {
            float distance = GetPosition().DistanceTo(this.path[0]);
            if (distance > 2) {
                Vector2 motion = this.path[0] - GetPosition();
                motion = motion / distance;
                KinematicCollision2D collision = MoveAndCollide(motion * this.moveModifier);
                if (collision != null && collision.GetCollider().GetType() == typeof(Player)) {
                    (collision.GetCollider() as Player).Die();
                    return;
                }
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

    private void UpdatePath() {
        Vector2[] path = this.navigation.GetSimplePath(this.GetPosition(), this.tileMap.MapToWorld(this.currentDestination), false);
        this.path = new List<Vector2>(path);
    }

    public void Die() {
        this.QueueFree();
    }

    private void UpdatePositionOnTileMap() {
        this.currentPositionOnTileMap = this.tileMap.WorldToMap(this.GetGlobalPosition());
        this.tileMap.enemyOnCell[this] = this.currentPositionOnTileMap;
    }
}
