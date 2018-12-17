using Godot;
using System;

public class Player : KinematicBody2D {
    private TileMap map;
    public int numberOfDroppedBombs;

    private SceneVariables sceneVariables;

    private int dropBombCooldown;

    public override void _Ready() {
        this.numberOfDroppedBombs = 0;
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
        this.sceneVariables = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;
        this.dropBombCooldown = 20;
    }

    public void Die() {
        Console.WriteLine("YOU DIED!");
        this.numberOfDroppedBombs = 0;
        this.map.FreeBombs();
        if (this.sceneVariables.numberOfLives > 0) {
            this.sceneVariables.numberOfLives -= 1;
            this.sceneVariables.ResetPlayerVariablesOnDeath();
            GetTree().ReloadCurrentScene();
        } else {
            //TODO: go to menu or sth
            this.sceneVariables.ResetPlayerVariablesOnFinalDeath();
            GetTree().ReloadCurrentScene();
        }
    }

    private void CheckForPowerups() {
        if (this.map.powerups.ContainsKey(GetPositionOnTileMap())) {
            Console.WriteLine("Powerup!");
            Powerup powerup = this.map.powerups[GetPositionOnTileMap()];
            powerup.ExecuteEffect();
            this.map.powerups.Remove(GetPositionOnTileMap());
            powerup.QueueFree();
        }
    }

    private void CheckForExit() {
        if(this.map.exitTile != null && this.map.exitTile.positionOnTileMap != TileMap.invalidTile && GetPositionOnTileMap() == this.map.exitTile.positionOnTileMap) {
            //TODO: change level, cleanup
        }
    }

    public override void _PhysicsProcess(float delta) {
        Vector2 motion = Move();
        motion = ModifyMoveBasedOnSurrounding(motion, delta);
        ExecuteMovement(motion, delta);

        if (Input.IsActionPressed("ui_accept") && this.numberOfDroppedBombs < this.sceneVariables.maxNumberOfDroppedBombs && this.dropBombCooldown > 20 && this.map.FindBombOnTile(GetPositionOnTileMap()) == null) {
            DropBomb();
            this.dropBombCooldown = 0;
        }

        this.dropBombCooldown += 1;

        CheckForPowerups();
        CheckForExit();
    }

    private Vector2 ModifyMoveBasedOnSurrounding(Vector2 originalMotion, float delta) {
        Vector2 newMotion = originalMotion;
        Vector2 currentTilePosition = GetPositionOnTileMap();
        Vector2 potentialTile = currentTilePosition + newMotion;
        Vector2 playerPosition = GetGlobalPosition();

        Transform2D mapTransform = this.map.GetGlobalTransform();

        if (newMotion.x != 0.0f
            && !this.map.IsTileValidForMovement(currentTilePosition + Directions.directionLeft)
            && !this.map.IsTileValidForMovement(currentTilePosition + Directions.directionRight)) {
            newMotion = Directions.noDirection;
        }

        if (newMotion.y != 0.0f
            && !this.map.IsTileValidForMovement(currentTilePosition + Directions.directionUp)
            && !this.map.IsTileValidForMovement(currentTilePosition + Directions.directionDown)) {
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
        KinematicCollision2D collision = MoveAndCollide(motion * delta * this.sceneVariables.playerMovementSpeed);
        if (collision != null && collision.GetCollider().GetType() == typeof(Enemy)) {
            Die();
            return;
        }
    }

    private void DropBomb() {
        Console.WriteLine("Drop bomb");
        Bomb bomb = new Bomb();

        Vector2 pos = this.map.GetPositionOfTileCenter(GetPositionOnTileMap());

        Transform2D mapTransform = this.map.GetGlobalTransform();

        bomb.position = pos;
        bomb.SetGlobalPosition(pos + mapTransform.Origin);
        bomb.SetName("Bomb" + this.numberOfDroppedBombs);

        Node world = GetTree().GetRoot();
        world.AddChild(bomb);
        this.numberOfDroppedBombs += 1;

        this.map.droppedBombPositions[bomb] = this.map.WorldToMap(bomb.position);
    }

    public Vector2 GetPositionOnTileMap() {
        if (this.map == null) {
            return TileMap.invalidTile;
        }
        Transform2D mapTransform = this.map.GetGlobalTransform();
        return this.map.WorldToMap(GetGlobalPosition() - mapTransform.Origin);
    }
}
