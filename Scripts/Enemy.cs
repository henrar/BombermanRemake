using Godot;
using System;
using System.Collections.Generic;

public class Enemy : KinematicBody2D {
    public Vector2 currentPositionOnTileMap;
    private readonly int moveModifier = 180;
    private TileMap tileMap;
    private List<Vector2> path;
    private Navigation2D navigation;
    private Vector2 previousDirection;

    public override void _Ready() {
        this.tileMap = GetTree().GetRoot().GetNode("World/Nav/TileMap") as TileMap;
        this.navigation = GetTree().GetRoot().GetNode("World/Nav") as Navigation2D;
        this.previousDirection = new Vector2(0, 0);
        UpdatePositionOnTileMap();
    }

    public override void _PhysicsProcess(float delta) {
        UpdatePositionOnTileMap();

        Vector2 direction = GetRandomDirection();
        KinematicCollision2D collision = MoveAndCollide(direction * this.moveModifier * delta);
        if (collision != null && collision.GetCollider().GetType() == typeof(Player)) {
            (collision.GetCollider() as Player).Die();
            return;
        }
    }

    private bool isAllowedToStep(Vector2 direction) {
        if (direction == Directions.directionUp) {
            Vector2 tile = this.currentPositionOnTileMap;
            tile.y -= 1;
            if (this.tileMap.GetCellv(tile) == (int)TileType.TileType_Grass) {
                return true;
            }
        } else if (direction == Directions.directionDown) {
            Vector2 tile = this.currentPositionOnTileMap;
            tile.y += 1;
            if (this.tileMap.GetCellv(tile) == (int)TileType.TileType_Grass) {
                return true;
            }
        } else if (direction == Directions.directionLeft) {
            Vector2 tile = this.currentPositionOnTileMap;
            tile.x -= 1;
            if (this.tileMap.GetCellv(tile) == (int)TileType.TileType_Grass) {
                return true;
            }
        } else if (direction == Directions.directionRight) {
            Vector2 tile = this.currentPositionOnTileMap;
            tile.x += 1;
            if (this.tileMap.GetCellv(tile) == (int)TileType.TileType_Grass) {
                return true;
            }
        }
        return false;
    }

    private Vector2 GetRandomDirection() {
        bool directionFound = false;
        Vector2 direction = this.previousDirection;
        while (!directionFound) {
            if (direction != Directions.noDirection) {
                if (isAllowedToStep(direction)) {
                    directionFound = true;
                } else if(direction == Directions.noDirection) {
                    direction = Directions.GetRandomDirection();
                    directionFound = false;
                } else {
                    direction = Directions.GetRandomDirection();
                }
            } else {
                direction = Directions.GetRandomDirection();
                directionFound = true;
            }
        }

        this.previousDirection = direction;

        return direction;
    }

    public void Die() {
        this.QueueFree();
    }

    private void UpdatePositionOnTileMap() {
        this.currentPositionOnTileMap = this.tileMap.WorldToMap(this.GetGlobalPosition());
        this.tileMap.enemyOnCell[this] = this.currentPositionOnTileMap;
    }
}
