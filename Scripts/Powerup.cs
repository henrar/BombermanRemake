using Godot;
using System;

public enum PowerUpType {
    Powerup_AddBombRange = 0,
    Powerup_AddBomb = 1,
    Powerup_IncreaseSpeed = 2,
    Powerup_AddLife = 3
}

public class Powerup : Node {
    public PowerUpType type;
    private SceneVariables sv;

    public override void _Ready() {
        this.sv = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;
    }

    public override void _PhysicsProcess(float delta) {

    }

    public void SetType(PowerUpType type) {
        this.type = type;
    }
}
