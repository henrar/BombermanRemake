using Godot;
using System;

public class Player : KinematicBody2D {
    public override void _Ready() {

    }

    public override void _Process(float delta) {
        Vector2 motion = new Vector2(0.0f, 0.0f);

        if (Input.IsActionPressed("ui_left")) {
            motion.x = -100;
        } else if (Input.IsActionPressed("ui_right")) {
            motion.x = 100;
        } else if (Input.IsActionPressed("ui_up")) {
            motion.y = -100;
        } else if (Input.IsActionPressed("ui_down")) {
            motion.y = 100;
        }

        MoveAndSlide(motion);

        if (Input.IsActionPressed("ui_space")) {
            //TODO: Bomb
        }
    }
}
