using Godot;
using System;

public class Player : KinematicBody2D {
    public int numberOfLives = 3;
    private readonly int moveModifier = 200;
    private TileMap map;

    private bool bombDropped;
    private Vector2 droppedBombPositionOnTileMap;
    private Bomb currentDroppedBomb;

    public override void _Ready() {
        this.numberOfLives = (GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables).savedNumberOfLives;
        this.bombDropped = false;
        this.map = GetTree().GetRoot().GetNode("World/Nav/TileMap") as TileMap;
    }

    public void Die() {
        Console.WriteLine("YOU DIED!");
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

        if(this.currentDroppedBomb != null && this.bombDropped && GetPositionOnTileMap() != this.droppedBombPositionOnTileMap) { //when we leave the tile that contains bomb, we should turn on collision as in the original
            this.currentDroppedBomb.AddCollision();
        }

        if (!GetTree().GetRoot().HasNode("Bomb") && this.bombDropped) {
            this.bombDropped = false;
        }
    }

    private Vector2 ModifyMoveBasedOnSurrounding(Vector2 originalMotion, float delta) {
        Vector2 newMotion = originalMotion;   
       /* Vector2 currentTile = GetPositionOnTileMap();
        Vector2 potentialTile = new Vector2(-1, -1);
        Vector2 addVec;

        Vector2 currentTilePosition = this.map.MapToWorld(currentTile);
        currentTilePosition = currentTilePosition + this.map.GetCellSize() / 2;

        Vector2 playerPosition = GetGlobalPosition();

        if (newMotion.x > 0) {
            addVec = new Vector2(1, 0);
            potentialTile = currentTile + addVec;

            Vector2 range = new Vector2(80, 0);
            if (this.map.GetCellv(potentialTile) != (int)TileTypes.TileType_Grass) {
                Console.WriteLine("ding");
                newMotion = new Vector2(0, 0);
            }
        } else if (newMotion.x < 0) {
            addVec = new Vector2(-1, 0);
            potentialTile = currentTile + addVec;

            if (this.map.GetCellv(potentialTile) != (int)TileTypes.TileType_Grass) {
                newMotion = new Vector2(0, 0);
            }
        } else if (newMotion.y > 0) {
            addVec = new Vector2(0, 1);
            potentialTile = currentTile + addVec;

            if (this.map.GetCellv(potentialTile) != (int)TileTypes.TileType_Grass) {
                newMotion = new Vector2(0, 0);
            }
        } else if (newMotion.y < 0) {
            addVec = new Vector2(0, -1);
            potentialTile = currentTile + addVec;

            if (this.map.GetCellv(potentialTile) != (int)TileTypes.TileType_Grass) {
                newMotion = new Vector2(0, 0);
            }
        }

        if (newMotion.y != 0.0f && playerPosition.y != currentTilePosition.y) {
            Vector2 interpolatedPosition = playerPosition; // new Vector2();
            interpolatedPosition.x = currentTilePosition.x;//currentTilePosition.LinearInterpolate(playerPosition, moveModifier * delta).x;
            interpolatedPosition.y = playerPosition.y;
            SetGlobalPosition(playerPosition.LinearInterpolate(interpolatedPosition, delta));
        }

        if (newMotion.x != 0.0f && playerPosition.x != currentTilePosition.x) {
            Vector2 interpolatedPosition = playerPosition; // new Vector2();
            interpolatedPosition.y = currentTilePosition.y;//currentTilePosition.LinearInterpolate(playerPosition, moveModifier * delta).x;
            interpolatedPosition.x = playerPosition.x;
            SetGlobalPosition(playerPosition.LinearInterpolate(interpolatedPosition, delta));
        }
        */
        return newMotion;
    }

    private Vector2 Move() {
        Vector2 motion = new Vector2(0.0f, 0.0f);

        if (Input.IsActionPressed("move_left")) {
            motion.x = -this.moveModifier;
        } else if (Input.IsActionPressed("move_right")) {
            motion.x = this.moveModifier;
        } else if (Input.IsActionPressed("move_up")) {
            motion.y = -this.moveModifier;
        } else if (Input.IsActionPressed("move_down")) {
            motion.y = this.moveModifier;
        }

        return motion;
    }

    private void ExecuteMovement(Vector2 motion, float delta) {
        KinematicCollision2D collision = MoveAndCollide(motion * delta);
        if (collision != null && collision.GetCollider().GetType() == typeof(Enemy)) {
            Die();
            return;
        }
    }

    private void DropBomb() {
        this.currentDroppedBomb = new Bomb();

        Vector2 tile = GetPositionOnTileMap();     
        Vector2 pos = this.map.MapToWorld(tile);
        pos = pos + this.map.GetCellSize() / 2.0f;

        this.currentDroppedBomb.position = pos;
        this.currentDroppedBomb.SetPosition(pos);
        this.currentDroppedBomb.SetName("Bomb");

        Node world = GetTree().GetRoot();
        world.AddChild(this.currentDroppedBomb);
        this.bombDropped = true;

        this.droppedBombPositionOnTileMap = this.map.WorldToMap(this.currentDroppedBomb.position);
    }

    public Vector2 GetPositionOnTileMap() {
        return this.map.WorldToMap(this.GetGlobalPosition());
    }
}
