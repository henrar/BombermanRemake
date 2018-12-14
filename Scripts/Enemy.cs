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
    public EnemyType enemyType;

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

    public void SetEnemyType(EnemyType enemyType) {
        this.enemyType = enemyType;
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

        if (!this.tileMap.isTileValidForMovement(nextTile) && GetGlobalPosition().DistanceTo(this.tileMap.GetPositionOfTileCenter(nextTile) + mapTransform.Origin) < (this.tileMap.GetCellSize().x + 10.0f)) {
            return false;
        }

        Bomb bomb = this.tileMap.FindBombOnTile(nextTile);
        if (bomb != null && GetGlobalPosition().DistanceTo(this.tileMap.GetPositionOfTileCenter(this.tileMap.droppedBombPositions[bomb]) + mapTransform.Origin) <= this.tileMap.GetCellSize().x) {
            return false;
        }

        return ((this.tileMap.isTileValidForMovement(nextTile))
            || (!this.tileMap.isTileValidForMovement(nextTile) && GetGlobalPosition().DistanceTo(this.tileMap.GetPositionOfTileCenter(nextTile) + mapTransform.Origin) >= (this.tileMap.GetCellSize().x + 10.0f)))
            && this.tileMap.FindEnemyOnTile(nextTile) == null;
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
                    this.previousDirection = direction;
                } else {
                    direction = Directions.GetRandomDirection();
                    directionFound = false;
                    this.previousDirection = direction;
                }
            } else {
                direction = Directions.GetRandomDirection();
                directionFound = true;
            }
        }

        this.previousDirection = direction;

        return direction;
    }

    private void SetScore() {
        SceneVariables sv = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;

        if (this.enemyType == EnemyType.EnemyType_Balloon) {
            sv.score += 100;
        } else if (this.enemyType == EnemyType.EnemyType_Mushroom) {
            sv.score += 200;
        } else if (this.enemyType == EnemyType.EnemyType_Barrel) {
            sv.score += 300;
        } else if (this.enemyType == EnemyType.EnemyType_Ghost) {
            sv.score += 200;
        } else if (this.enemyType == EnemyType.EnemyType_Coin) {
            sv.score += 200;
        }
    }

    public void Die() {
        SetScore();

        this.QueueFree();
    }

    private Vector2 ModifyMoveBasedOnSurrounding(Vector2 originalMotion, float delta) {
        Vector2 newMotion = originalMotion;
        Vector2 currentTilePosition = this.currentPositionOnTileMap;
        Vector2 potentialTile = currentTilePosition + newMotion;

        Transform2D mapTransform = this.tileMap.GetGlobalTransform();

        Vector2 enemyPosition = GetGlobalPosition();

        if (newMotion.x != 0.0f
            && !this.tileMap.isTileValidForMovement(currentTilePosition + Directions.directionLeft)
            && !this.tileMap.isTileValidForMovement(currentTilePosition + Directions.directionRight)) {
            newMotion = Directions.noDirection;
        }

        if (newMotion.y != 0.0f
            && !this.tileMap.isTileValidForMovement(currentTilePosition + Directions.directionUp)
            && !this.tileMap.isTileValidForMovement(currentTilePosition + Directions.directionDown)) {
            newMotion = Directions.noDirection;
        }

        if (newMotion == Directions.directionLeft || newMotion == Directions.directionRight) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile) + mapTransform.Origin;
            newPos.x = enemyPosition.x;
            SetGlobalPosition(enemyPosition.LinearInterpolate(newPos, delta * 10));
        }

        if (newMotion == Directions.directionUp || newMotion == Directions.directionDown) {
            Vector2 newPos = this.tileMap.GetPositionOfTileCenter(potentialTile) + mapTransform.Origin;
            newPos.y = enemyPosition.y;
            SetGlobalPosition(enemyPosition.LinearInterpolate(newPos, delta * 10));
        }

        return newMotion;
    }

    private void UpdatePositionOnTileMap() {
        Transform2D mapTransform = this.tileMap.GetGlobalTransform();
        this.currentPositionOnTileMap = this.tileMap.WorldToMap(this.GetGlobalPosition() - mapTransform.Origin);
        this.tileMap.enemyOnCell[this] = this.currentPositionOnTileMap;
    }
}
