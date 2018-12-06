using Godot;
using System;

public class SceneVariables : Node {
    public Node CurrentScene { get; set; }
    public int savedNumberOfLives;
    public int numberOfEnemies;
    public int numberOfPowerUps;
    public float powerup1DropChance;
    public float powerup2DropChance;
    public float powerup3DropChance;

    public override void _Ready() {
        Viewport root = GetTree().GetRoot();
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        this.savedNumberOfLives = 3;
        this.numberOfEnemies = 10; //TODO: change on level transitions
        this.numberOfPowerUps = 3;
        this.powerup1DropChance = 1.0f;
        this.powerup2DropChance = 0.5f;
        this.powerup3DropChance = 0.2f; //TODO: lower chances on level transition
    }

    //    public override void _Process(float delta) {       
    //    }
}
