using Godot;
using System;

public class Player : KinematicBody2D {
    public int numberOfLives = 3;
    private readonly int moveModifier = 10;

    private bool bombDropped;

    public override void _Ready() {
        this.numberOfLives = (GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables).savedNumberOfLives;
        this.bombDropped = false;
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

        CheckCollision(motion);

        if (Input.IsActionPressed("ui_accept") && !this.bombDropped) {
            DropBomb();
        }

        if (!GetTree().GetRoot().HasNode("Bomb") && this.bombDropped) {
            this.bombDropped = false;
        }
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

    private void CheckCollision(Vector2 motion) {
        KinematicCollision2D collision = MoveAndCollide(motion);
        if (collision != null && collision.GetCollider().GetType() == typeof(Enemy)) {
            Die();
            return;
        }
    }

    private void DropBomb() {
        Bomb bomb = new Bomb();

        Vector2 tile = GetPositionOnTileMap();
        TileMap map = GetTree().GetRoot().GetNode("World/Nav/TileMap") as TileMap;
        Vector2 pos = map.MapToWorld(tile);
        pos = pos + map.GetCellSize() / 2.0f;

        bomb.SetPosition(pos);

        Node world = GetTree().GetRoot();
        bomb.SetName("Bomb");
        world.AddChild(bomb);
        this.bombDropped = true;
    }

    public Vector2 GetPositionOnTileMap() {
        TileMap map = GetTree().GetRoot().GetNode("World/Nav/TileMap") as TileMap;
        return map.WorldToMap(this.GetGlobalPosition());
    }
}
