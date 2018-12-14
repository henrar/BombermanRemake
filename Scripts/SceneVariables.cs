using Godot;
using System;

public class SceneVariables : Node {
    public Node CurrentScene { get; set; }
    public int numberOfLives;
    public int numberOfEnemies;
    public int numberOfPowerUps;
    public float powerup1DropChance;
    public float powerup2DropChance;
    public float powerup3DropChance;

    public int score;
    public int bombRange;

    public override void _Ready() {
        Viewport root = GetTree().GetRoot();
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        this.numberOfEnemies = 10; //TODO: change on level transitions
        this.numberOfPowerUps = 3;
        this.powerup1DropChance = 1.0f;
        this.powerup2DropChance = 0.5f;
        this.powerup3DropChance = 0.2f; //TODO: lower chances on level transition

        this.bombRange = 1;
        this.score = 0;
        this.numberOfLives = 3;

        Console.WriteLine("Ready SV");
    }

    public void ResetPlayerVariables() {
        this.bombRange = 1;
        this.score = 0;
        this.numberOfLives = 3;
    }
}
