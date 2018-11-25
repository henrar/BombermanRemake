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
        direction = ModifyMoveBasedOnSurrounding(direction, delta);

        KinematicCollision2D collision = MoveAndCollide(direction * this.moveModifier * delta);
        if (collision != null && collision.GetCollider().GetType() == typeof(Player)) {
            (collision.GetCollider() as Player).Die();
            return;
        }
    }

    private bool isAllowedToStep(Vector2 direction) {
        if (direction == Directions.directionUp) {
            Vector2 currentTile = this.currentPositionOnTileMap;
            Vector2 nextTile = currentTile + Directions.directionUp;
            if (this.tileMap.GetCellv(nextTile) == (int)TileType.TileType_Grass && this.tileMap.GetPositionOfTileCenter(currentTile).DistanceTo(nextTile) > 80.0f) {
                return true;
            }
        } else if (direction == Directions.directionDown) {
            Vector2 currentTile = this.currentPositionOnTileMap;
            Vector2 nextTile = currentTile + Directions.directionDown;
            if (this.tileMap.GetCellv(nextTile) == (int)TileType.TileType_Grass && this.tileMap.GetPositionOfTileCenter(currentTile).DistanceTo(nextTile) > 80.0f) {
                return true;
            }
        } else if (direction == Directions.directionLeft) {
            Vector2 currentTile = this.currentPositionOnTileMap;
            Vector2 nextTile = currentTile + Directions.directionLeft;
            if (this.tileMap.GetCellv(nextTile) == (int)TileType.TileType_Grass && this.tileMap.GetPositionOfTileCenter(currentTile).DistanceTo(nextTile) > 80.0f) {
                return true;
            }
        } else if (direction == Directions.directionRight) {
            Vector2 currentTile = this.currentPositionOnTileMap;
            Vector2 nextTile = currentTile + Directions.directionRight;
            if (this.tileMap.GetCellv(nextTile) == (int)TileType.TileType_Grass && this.tileMap.GetPositionOfTileCenter(currentTile).DistanceTo(nextTile) > 80.0f) {
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

    private Vector2 ModifyMoveBasedOnSurrounding(Vector2 originalMotion, float delta) {
        Vector2 newMotion = originalMotion;
        Vector2 currentTilePosition = this.currentPositionOnTileMap;
        Vector2 potentialTile = currentTilePosition + newMotion;
        Vector2 playerPosition = GetGlobalPosition();

        if (newMotion.x != 0
            && this.tileMap.GetCellv(currentTilePosition + Directions.directionLeft) != (int)TileType.TileType_Grass
            && this.tileMap.GetCellv(currentTilePosition + Directions.directionRight) != (int)TileType.TileType_Grass) {
            newMotion = Directions.noDirection;
        }

        if (newMotion.y != 0
            && this.tileMap.GetCellv(currentTilePosition + Directions.directionUp) != (int)TileType.TileType_Grass
            && this.tileMap.GetCellv(currentTilePosition + Directions.directionDown) != (int)TileType.TileType_Grass) {
            newMotion = Directions.noDirection;
        }

        if (newMotion == Directions.directionLeft) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile);
            newPos.x = playerPosition.x;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        if (newMotion == Directions.directionRight) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile);
            newPos.x = playerPosition.x;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        if (newMotion == Directions.directionUp) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile);
            newPos.y = playerPosition.y;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        if (newMotion == Directions.directionDown) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile);
            newPos.y = playerPosition.y;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        return newMotion;
    }

    private void UpdatePositionOnTileMap() {
        this.currentPositionOnTileMap = this.tileMap.WorldToMap(this.GetGlobalPosition());
        this.tileMap.enemyOnCell[this] = this.currentPositionOnTileMap;
    }
}
