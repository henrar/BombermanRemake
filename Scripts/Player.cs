using Godot;
using System;

public class Player : KinematicBody2D {
    public int numberOfLives = 3;
    private readonly int moveModifier = 180;
    private TileMap map;

    private bool bombDropped;
    private Bomb currentDroppedBomb;

    public override void _Ready() {
        this.numberOfLives = (GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables).savedNumberOfLives;
        this.bombDropped = false;
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
    }

    public void Die() {
        Console.WriteLine("YOU DIED!");
        this.bombDropped = false;
        if (this.currentDroppedBomb != null) {
            this.currentDroppedBomb.QueueFree();
        }
        if (this.numberOfLives > 0) {
            this.numberOfLives -= 1;
            (GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables).savedNumberOfLives -= 1;
            GetTree().ReloadCurrentScene();
        } else {
            //TODO: go to menu or sth
            (GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables).savedNumberOfLives = 3;
            GetTree().ReloadCurrentScene();
        }
    }

    public override void _PhysicsProcess(float delta) {
        Vector2 motion = Move();
        motion = ModifyMoveBasedOnSurrounding(motion, delta);
        ExecuteMovement(motion, delta);

        if (Input.IsActionPressed("ui_accept") && !this.bombDropped) {
            DropBomb();
        }

        if (!GetTree().GetRoot().HasNode("Bomb") && this.bombDropped) {
            this.bombDropped = false;
        }
    }

    private Vector2 ModifyMoveBasedOnSurrounding(Vector2 originalMotion, float delta) {
        Vector2 newMotion = originalMotion;
        Vector2 currentTilePosition = GetPositionOnTileMap();
        Vector2 potentialTile = currentTilePosition + newMotion;
        Vector2 playerPosition = GetGlobalPosition();

        Transform2D mapTransform = this.map.GetGlobalTransform();

        if (newMotion.x != 0.0f
            && !this.map.isTileValidForMovement(currentTilePosition + Directions.directionLeft)
            && !this.map.isTileValidForMovement(currentTilePosition + Directions.directionRight)) {
            newMotion = Directions.noDirection;
        }

        if (newMotion.y != 0.0f
            && !this.map.isTileValidForMovement(currentTilePosition + Directions.directionUp)
            && !this.map.isTileValidForMovement(currentTilePosition + Directions.directionDown)) {
            newMotion = Directions.noDirection;
        }

        if (newMotion == Directions.directionLeft || newMotion == Directions.directionRight) {
            Vector2 newPos = this.map.GetPositionOfTileCenter(potentialTile) + mapTransform.Origin;
            newPos.x = playerPosition.x;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        if (newMotion == Directions.directionUp || newMotion == Directions.directionDown) {
            Vector2 newPos = this.map.GetPositionOfTileCenter(potentialTile) + mapTransform.Origin;
            newPos.y = playerPosition.y;
            SetGlobalPosition(playerPosition.LinearInterpolate(newPos, delta * 10));
        }

        return newMotion;
    }

    private Vector2 Move() {
        Vector2 motion = new Vector2(0.0f, 0.0f);

        if (Input.IsActionPressed("move_left")) {
            motion = Directions.directionLeft;
        } else if (Input.IsActionPressed("move_right")) {
            motion = Directions.directionRight;
        } else if (Input.IsActionPressed("move_up")) {
            motion = Directions.directionUp;
        } else if (Input.IsActionPressed("move_down")) {
            motion = Directions.directionDown;
        }

        return motion;
    }

    private void ExecuteMovement(Vector2 motion, float delta) {
        KinematicCollision2D collision = MoveAndCollide(motion * delta * this.moveModifier);
        if (collision != null && collision.GetCollider().GetType() == typeof(Enemy)) {
            Die();
            return;
        }
    }

    private void DropBomb() {
        this.currentDroppedBomb = new Bomb();

        Vector2 pos = this.map.GetPositionOfTileCenter(GetPositionOnTileMap());

        Transform2D mapTransform = this.map.GetGlobalTransform();

        this.currentDroppedBomb.position = pos;
        this.currentDroppedBomb.SetGlobalPosition(pos + mapTransform.Origin);
        this.currentDroppedBomb.SetName("Bomb");

        Node world = GetTree().GetRoot();
        world.AddChild(this.currentDroppedBomb);
        this.bombDropped = true;

        this.map.droppedBombPosition = this.map.WorldToMap(this.currentDroppedBomb.position);
    }

    public Vector2 GetPositionOnTileMap() {
        if(this.map == null) {
            return TileMap.invalidTile;
        }
        Transform2D mapTransform = this.map.GetGlobalTransform();
        return this.map.WorldToMap(GetGlobalPosition() - mapTransform.Origin);
    }
}
