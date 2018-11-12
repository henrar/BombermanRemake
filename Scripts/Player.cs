using Godot;
using System;

public class Player : KinematicBody2D {
    private readonly int moveModifier = 100;

    private bool bombDropped;

    public override void _Ready() {
        bombDropped = false;
    }

    public override void _Process(float delta) {
        Vector2 motion = new Vector2(0.0f, 0.0f);

        if (Input.IsActionPressed("move_left")) {
            motion.x = -moveModifier;
        } else if (Input.IsActionPressed("move_right")) {
            motion.x = moveModifier;
        } else if (Input.IsActionPressed("move_up")) {
            motion.y = -moveModifier;
        } else if (Input.IsActionPressed("move_down")) {
            motion.y = moveModifier;
        }

        MoveAndSlide(motion);

        if (Input.IsActionPressed("ui_accept") && !bombDropped) {
            Vector2 pos = this.GetPosition();
            Bomb bomb = new Bomb();
            bomb.SetPosition(pos); //so it spawn where player is
            bomb.position = this.GetGlobalPosition();
            Node world = GetTree().GetRoot();
            bomb.SetName("Bomb");
            world.AddChild(bomb);
            bombDropped = true;
        }

        if (!GetTree().GetRoot().HasNode("Bomb") && bombDropped) {
            bombDropped = false;
        }
    }
}
