using Godot;
using System;

public class Player : KinematicBody2D {
    private bool bombDropped;

    public override void _Ready() {
        bombDropped = false;
    }

    public override void _Process(float delta) {
        Vector2 motion = new Vector2(0.0f, 0.0f);

        if (Input.IsActionPressed("move_left")) {
            motion.x = -100;
        } else if (Input.IsActionPressed("move_right")) {
            motion.x = 100;
        } else if (Input.IsActionPressed("move_up")) {
            motion.y = -100;
        } else if (Input.IsActionPressed("move_down")) {
            motion.y = 100;
        }

        MoveAndSlide(motion);

        if (Input.IsActionPressed("ui_accept") && !bombDropped) {
            Vector2 pos = this.GetPosition();
            Bomb bomb = new Bomb();
            bomb.SetPosition(pos);
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
