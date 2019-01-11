using Godot;
using System;
using System.Collections.Generic;

public enum EnemyType {
    Balloon = 0,
    Mushroom = 1, //+20% speed
    Barrel = 2, //+33% speed
    Ghost = 3, // -15% speed, moves through bricks
    Coin = 4, //+45%, moves through bricks
}

public class Enemy : KinematicBody2D {
    public Vector2 currentPositionOnTileMap;
    private readonly float baseMovementSpeed = 180.0f;
    private float movementSpeed;
    private TileMap tileMap;


    private float animTimeElapsed;
    private int spriteNum;

    private Vector2 previousDirection;
    private AnimatedSprite animatedSprite;
    private CollisionPolygon2D collision;
    public EnemyType enemyType;
    private SceneVariables sceneVariables;

    private readonly string frontName = "front";
    private readonly string backName = "back";
    private readonly string leftName = "left";
    private readonly string rightName = "right";

    public override void _Ready() {
        this.tileMap = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
        this.sceneVariables = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;

        SetPosition(new Vector2(0, 0));

        Transform2D mapTransfrom = this.tileMap.GetGlobalTransform();
        SetGlobalPosition(this.tileMap.GetPositionOfTileCenter(this.currentPositionOnTileMap) + mapTransfrom.Origin);

        SetMovementSpeed();
        LoadAnimatedSprite();
        this.animTimeElapsed = 0.0f;
        this.previousDirection = Directions.noDirection;

        SetCollision();
        UpdatePositionOnTileMap();

        SetCollisionMask(2);
    }

    private void SetMovementSpeed() {
        if (this.enemyType == EnemyType.Balloon) {
            this.movementSpeed = this.baseMovementSpeed * 0.8f;
        } else if (this.enemyType == EnemyType.Mushroom) {
            this.movementSpeed = this.baseMovementSpeed * 1.0f;
        } else if (this.enemyType == EnemyType.Barrel) {
            this.movementSpeed = this.baseMovementSpeed * 1.33f;
        } else if (this.enemyType == EnemyType.Ghost) {
            this.movementSpeed = this.baseMovementSpeed * 0.75f;
        } else if (this.enemyType == EnemyType.Coin) {
            this.movementSpeed = this.baseMovementSpeed * 1.45f;
        }
    }

    private void SetCollision() {
        if (this.enemyType != EnemyType.Ghost && this.enemyType != EnemyType.Coin) {
            Vector2[] indices = {
                new Vector2(-30, -30),
                new Vector2(-30, 30),
                new Vector2(30, 30),
                new Vector2(30,-30)
            };

            this.collision = new CollisionPolygon2D();
            this.collision.SetPolygon(indices);
            AddChild(this.collision);
        }
    }

    private string GetEnemyName() {
        switch (this.enemyType) {
            case EnemyType.Balloon: {
                    return "balon";
                }
            case EnemyType.Barrel: {
                    return "beczka";
                }
            case EnemyType.Coin: {
                    return "moneta";
                }
            case EnemyType.Mushroom: {
                    return "grzyb";
                }
            case EnemyType.Ghost: {
                    return "duch";
                }
            default: {
                    return "";
                }
        }
    }

    private void SetSpriteBasedOnMovement(Vector2 motion, float delta) {
        string name = "";

        this.animTimeElapsed = this.animTimeElapsed + delta;

        if (motion == Directions.directionUp) {
            name = this.backName;
        } else if (motion == Directions.directionDown) {
            name = this.frontName;
        } else if (motion == Directions.directionLeft) {
            name = this.leftName;
        } else if (motion == Directions.directionRight) {
            name = this.rightName;
        } else {
            name = this.frontName;
        }

        this.animatedSprite.SetAnimation(name);

        if (motion != Directions.noDirection && this.animTimeElapsed > 0.1f) {
         //   Console.WriteLine("Type: " + this.enemyType);
           // Console.WriteLine("Name: " + name);
            this.spriteNum = (this.spriteNum + 1) % this.animatedSprite.GetSpriteFrames().GetFrameCount(name);
            this.animatedSprite.SetFrame(this.spriteNum);

            this.animTimeElapsed = 0.0f;
        }
    }

    private void LoadSpriteFrames(ref SpriteFrames spriteFrames, string name) {
        int count = 0;

        if (this.enemyType == EnemyType.Balloon) {
            count = 2;
        } else if(this.enemyType == EnemyType.Barrel) {
            if(name == this.frontName) {
                count = 6;
            } else if (name == this.backName) {
                count = 6;
            } else if(name == this.leftName) {
                count = 6;
            } else if(name == this.rightName) {
                count = 6;
            }
        } else if(this.enemyType == EnemyType.Coin) {
            if (name == this.frontName) {
                count = 2;
            } else if (name == this.backName) {
                count = 2;
            } else if (name == this.leftName) {
                count = 12;
            } else if (name == this.rightName) {
                count = 12;
            }
        } else if(this.enemyType == EnemyType.Ghost) {
            count = 2;
        } else if(this.enemyType == EnemyType.Mushroom) {
            if (name == this.frontName) {
                count = 4;
            } else if (name == this.backName) {
                count = 4;
            } else if (name == this.leftName) {
                count = 5;
            } else if (name == this.rightName) {
                count = 5;
            }
        }

        for (int i = 1; i <= count; ++i) {
            ImageTexture tex = new ImageTexture();
            tex.Load("res://Assets/enemy_anim/" + GetEnemyName() + "/" + GetEnemyName() + "-" + name + "-f" + i + ".png");
            spriteFrames.AddFrame(name, tex);
        }
    }

    private void LoadAnimatedSprite() {
        this.animatedSprite = new AnimatedSprite();
        SpriteFrames spriteFrames = new SpriteFrames();

        spriteFrames.AddAnimation(this.frontName);
        spriteFrames.AddAnimation(this.backName);
        spriteFrames.AddAnimation(this.leftName);
        spriteFrames.AddAnimation(this.rightName);

        LoadSpriteFrames(ref spriteFrames, this.frontName);
        LoadSpriteFrames(ref spriteFrames, this.backName);
        LoadSpriteFrames(ref spriteFrames, this.leftName);
        LoadSpriteFrames(ref spriteFrames, this.rightName);

        this.animatedSprite.SetScale(new Vector2(0.75f, 0.75f));

        this.animatedSprite.SetSpriteFrames(spriteFrames);

        this.animatedSprite.SetZIndex(this.tileMap.GetZIndex() + 1);

        AddChild(this.animatedSprite);
    }

    public void SetEnemyType(EnemyType enemyType) {
        this.enemyType = enemyType;
    }

    private void CheckIfContactedPlayer(KinematicCollision2D collision) {
        if (this.sceneVariables.godMode) {
            return;
        }

        Player player = GetTree().GetRoot().GetNode("World/Player") as Player;
        if (player == null) {
            return;
        }

        if (collision != null && collision.GetCollider().GetType() == typeof(Player)) {
            (collision.GetCollider() as Player).Die(true, DeathType.ByEnemy);
            return;
        }

        if (IsIgnoringCollisions() && (player.GetGlobalPosition().DistanceTo(this.GetGlobalPosition()) <= 40.0f)) {
            player.Die(true, DeathType.ByEnemy);
        }
    }

    public override void _PhysicsProcess(float delta) {
        UpdatePositionOnTileMap();

        if (this.sceneVariables.aiDisabled) {
            return;
        }

        Vector2 direction = GetRandomDirection();
        direction = ModifyMoveBasedOnSurrounding(direction, delta);

        SetSpriteBasedOnMovement(direction, delta);

        KinematicCollision2D collision = MoveAndCollide(direction * this.movementSpeed * delta);
        CheckIfContactedPlayer(collision);
    }

    public bool IsIgnoringCollisions() {
        return this.enemyType == EnemyType.Ghost || this.enemyType == EnemyType.Coin;
    }

    private bool ShouldStepOnTile(Vector2 currentTile, Vector2 nextTile) {
        Transform2D mapTransform = this.tileMap.GetGlobalTransform();

        if (!this.tileMap.IsTileValidForMovement(nextTile, IsIgnoringCollisions()) && GetGlobalPosition().DistanceTo(this.tileMap.GetPositionOfTileCenter(nextTile) + mapTransform.Origin) < (this.tileMap.GetCellSize().x + 10.0f)) {
            return false;
        }

        Bomb bomb = this.tileMap.FindBombOnTile(nextTile);
        if (bomb != null && GetGlobalPosition().DistanceTo(this.tileMap.GetPositionOfTileCenter(this.tileMap.droppedBombPositions[bomb]) + mapTransform.Origin) <= this.tileMap.GetCellSize().x) {
            return false;
        }

        return ((this.tileMap.IsTileValidForMovement(nextTile, IsIgnoringCollisions()))
            || (!this.tileMap.IsTileValidForMovement(nextTile, IsIgnoringCollisions()) && GetGlobalPosition().DistanceTo(this.tileMap.GetPositionOfTileCenter(nextTile) + mapTransform.Origin) >= (this.tileMap.GetCellSize().x + 10.0f)));
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

        if (this.enemyType == EnemyType.Balloon) {
            sv.score += 100;
        } else if (this.enemyType == EnemyType.Mushroom) {
            sv.score += 200;
        } else if (this.enemyType == EnemyType.Barrel) {
            sv.score += 300;
        } else if (this.enemyType == EnemyType.Ghost) {
            sv.score += 200;
        } else if (this.enemyType == EnemyType.Coin) {
            sv.score += 200;
        }
    }

    public void Die() {
        SetScore();

        this.tileMap.enemies.Remove(this);
        Console.WriteLine("Remaining enemies: " + this.tileMap.GetRemainingEnemiesCount());

        this.QueueFree();
    }

    private Vector2 ModifyMoveBasedOnSurrounding(Vector2 originalMotion, float delta) {
        Vector2 newMotion = originalMotion;
        Vector2 currentTilePosition = this.currentPositionOnTileMap;
        Vector2 potentialTile = currentTilePosition + newMotion;

        Transform2D mapTransform = this.tileMap.GetGlobalTransform();

        Vector2 enemyPosition = GetGlobalPosition();

        if (newMotion.x != 0.0f
            && !this.tileMap.IsTileValidForMovement(currentTilePosition + Directions.directionLeft, IsIgnoringCollisions())
            && !this.tileMap.IsTileValidForMovement(currentTilePosition + Directions.directionRight, IsIgnoringCollisions())) {
            newMotion = Directions.noDirection;
        }

        if (newMotion.y != 0.0f
            && !this.tileMap.IsTileValidForMovement(currentTilePosition + Directions.directionUp, IsIgnoringCollisions())
            && !this.tileMap.IsTileValidForMovement(currentTilePosition + Directions.directionDown, IsIgnoringCollisions())) {
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
