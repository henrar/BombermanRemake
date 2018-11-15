using Godot;
using System;

public class Player : KinematicBody2D {
    public int numberOfLives = 3;
    private readonly int moveModifier = 200;

    private bool bombDropped;

    public override void _Ready() {
        this.numberOfLives = (GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables).savedNumberOfLives;
        this.bombDropped = false;
    }

    public override void _Process(float delta) {
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

        MoveAndSlide(motion);

        int slideCount = GetSlideCount();
        for(int i = 0; i < slideCount; ++i) {
            KinematicCollision2D collision = GetSlideCollision(i);
            if (collision.GetCollider().GetType() == typeof(Enemy)) {
                Die();
            }
        }

        if (Input.IsActionPressed("ui_accept") && !this.bombDropped) {
            Vector2 pos = this.GetPosition();
            Bomb bomb = new Bomb();
            bomb.SetPosition(pos); //so it spawn where player is
            bomb.position = this.GetGlobalPosition();
            Node world = GetTree().GetRoot();
            bomb.SetName("Bomb");
            world.AddChild(bomb);
            this.bombDropped = true;
        }

        if (!GetTree().GetRoot().HasNode("Bomb") && this.bombDropped) {
            this.bombDropped = false;
        }
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

    public Vector2 GetPositionOnTileMap() {
        TileMap map = GetTree().GetRoot().GetNode("World/Nav/TileMap") as TileMap;
        return map.WorldToMap(this.GetPosition());
    }
}
