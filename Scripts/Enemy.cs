using Godot;
using System;
using System.Collections.Generic;

public enum EnemyType {
    EnemyType_Balloon = 0,
    EnemyType_Mushroom = 1, //+20% speed
    EnemyType_Barrel = 2, //+33% speed
    EnemyType_Ghost = 3, // -15% speed, moves through bricks
    EnemyType_Coin = 4, //+45%, moves through bricks
}

public class Enemy : KinematicBody2D {
    public Vector2 currentPositionOnTileMap;
    private readonly int moveModifier = 180;
    private TileMap tileMap;
    private Vector2 previousDirection;
    private Sprite sprite;
    private CollisionPolygon2D collision;

    public override void _Ready() {
        this.tileMap = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
        this.previousDirection = new Vector2(0, 0);

        SetPosition(new Vector2(0, 0));

        Transform2D mapTransfrom = this.tileMap.GetGlobalTransform();
        SetGlobalPosition(this.tileMap.GetPositionOfTileCenter(this.currentPositionOnTileMap) + mapTransfrom.Origin);

        this.sprite = new Sprite();
        ImageTexture tex = new ImageTexture();
        tex.Load("res://Assets/icon.png");
        this.sprite.SetTexture(tex);
        this.sprite.SetScale(new Vector2(0.5f, 0.5f));
        AddChild(this.sprite);

        Vector2[] indices = {
            new Vector2(-30, -30),
            new Vector2(-30, 30),
            new Vector2(30, 30),
            new Vector2(30,-30)
        };

        this.collision = new CollisionPolygon2D();
        this.collision.SetPolygon(indices);
        AddChild(this.collision);

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

    private bool ShouldStepOnTile(Vector2 currentTile, Vector2 nextTile) {
        Transform2D mapTransform = this.tileMap.GetGlobalTransform();

        if (this.tileMap.GetCellv(nextTile) != (int)TileType.TileType_Floor && GetGlobalPosition().DistanceTo(this.tileMap.GetPositionOfTileCenter(nextTile) + mapTransform.Origin) < 90.0f) {
            return false;
        }

        return ((this.tileMap.GetCellv(nextTile) == (int)TileType.TileType_Floor)
            || (this.tileMap.GetCellv(nextTile) != (int)TileType.TileType_Floor && GetGlobalPosition().DistanceTo(this.tileMap.GetPositionOfTileCenter(nextTile) + mapTransform.Origin) >= 90.0f))
            && this.tileMap.FindEnemyOnTile(nextTile) == null
            && this.tileMap.droppedBombPosition != nextTile;
    }

    private bool IsAllowedToStep(Vector2 direction) {
        Vector2 currentTile = this.currentPositionOnTileMap;

        if (direction == Directions.directionUp) {
            Vector2 nextTile = currentTile + Directions.directionUp;
            if (ShouldStepOnTile(currentTile, nextTile)) {
                return true;
            }
        } else if (direction == Directions.directionDown) {
            Vector2 nextTile = currentTile + Directions.directionDown;
            if (ShouldStepOnTile(currentTile, nextTile)) {
                return true;
            }
        } else if (direction == Directions.directionLeft) {
            Vector2 nextTile = currentTile + Directions.directionLeft;
            if (ShouldStepOnTile(currentTile, nextTile)) {
                return true;
            }
        } else if (direction == Directions.directionRight) {
            Vector2 nextTile = currentTile + Directions.directionRight;
            if (ShouldStepOnTile(currentTile, nextTile)) {
                return true;
            }
        }
        return false;
    }

    private Vector2 GetRandomDirection() {
        bool directionFound = false;
        Vector2 direction = this.previousDirection;
        if (!IsAllowedToStep(Directions.directionLeft)
            && !IsAllowedToStep(Directions.directionRight)
            && !IsAllowedToStep(Directions.directionUp)
            && !IsAllowedToStep(Directions.directionDown)) {
            return Directions.noDirection;
        }
        while (!directionFound) {
            if (direction != Directions.noDirection) {
                if (IsAllowedToStep(direction)) {
                    directionFound = true;
                } else if (direction == Directions.noDirection) {
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

        Transform2D mapTransform = this.tileMap.GetGlobalTransform();

        Vector2 playerPosition = GetGlobalPosition();

        if (newMotion.x != 0.0f
            && this.tileMap.GetCellv(currentTilePosition + Directions.directionLeft) != (int)TileType.TileType_Floor
            && this.tileMap.GetCellv(currentTilePosition + Directions.directionRight) != (int)TileType.TileType_Floor) {
            newMotion = Directions.noDirection;
        }

        if (newMotion.y != 0.0f
            && this.tileMap.GetCellv(currentTilePosition + Directions.directionUp) != (int)TileType.TileType_Floor
            && this.tileMap.GetCellv(currentTilePosition + Directions.directionDown) != (int)TileType.TileType_Floor) {
            newMotion = Directions.noDirection;
        }

        if (newMotion == Directions.directionLeft) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile) + mapTransform.Origin;
            newPos.x = playerPosition.x;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        if (newMotion == Directions.directionRight) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile) + mapTransform.Origin;
            newPos.x = playerPosition.x;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        if (newMotion == Directions.directionUp) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile) + mapTransform.Origin;
            newPos.y = playerPosition.y;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        if (newMotion == Directions.directionDown) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile) + mapTransform.Origin;
            newPos.y = playerPosition.y;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        return newMotion;
    }

    private void UpdatePositionOnTileMap() {
        Transform2D mapTransform = this.tileMap.GetGlobalTransform();
        this.currentPositionOnTileMap = this.tileMap.WorldToMap(this.GetGlobalPosition() - mapTransform.Origin);
        this.tileMap.enemyOnCell[this] = this.currentPositionOnTileMap;
    }
}
