using Godot;
using System;

enum PowerUpType {
    Powerup_AddBombRange = 0,
    Powerup_AddBomb = 1,
    Powerup_IncreaseSpeed = 2
}

public class Powerup : Node {
    public override void _Ready() {

    }

    public override void _PhysicsProcess(float delta) {
    }
}
