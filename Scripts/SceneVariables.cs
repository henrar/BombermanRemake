using Godot;
using System;

public class SceneVariables : Node {
    public Node CurrentScene { get; set; }
    public int numberOfLives;
    public int playerMovementSpeed;

    public int numberOfEnemies;
    public int numberOfPowerUps;
    public float powerup1DropChance;
    public float powerup2DropChance;
    public float powerup3DropChance;

    public float maxRandomCellsPercentage;

    public int score;

    public int bombRange;
    public int maxNumberOfDroppedBombs;

    public int currentLevel;

    public override void _Ready() {
        Viewport root = GetTree().GetRoot();
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
        this.numberOfEnemies = 10; //TODO: change on level transitions
        this.numberOfPowerUps = 3;

        this.currentLevel = 1; //TODO: change level on level transition

        switch(this.currentLevel) {
            case 1: {
                    this.powerup1DropChance = 0.9f; 
                    this.powerup2DropChance = 0.45f;
                    this.powerup3DropChance = 0.15f; 
                    break;
                }
            case 2: {
                    this.powerup1DropChance = 0.7f;
                    this.powerup2DropChance = 0.3f;
                    this.powerup3DropChance = 0.1f;
                    break;
                }
            case 3: {
                    this.powerup1DropChance = 0.5f;
                    this.powerup2DropChance = 0.25f;
                    this.powerup3DropChance = 0.1f;
                    break;
                }
            default: {
                    this.powerup1DropChance = 0.5f;
                    this.powerup2DropChance = 0.25f;
                    this.powerup3DropChance = 0.1f;
                    break;
                }
        }

        this.maxRandomCellsPercentage = 0.4f;

        this.playerMovementSpeed = 180;

        this.maxNumberOfDroppedBombs = 1;
        this.bombRange = 1;
        this.score = 0;
        this.numberOfLives = 3;

        Console.WriteLine("Ready SV");
    }

    public void ResetPlayerVariablesOnDeath() {
        this.bombRange = 1;
        this.maxNumberOfDroppedBombs = 1;
    }

    public void ResetPlayerVariablesOnFinalDeath() {
        this.bombRange = 1;
        this.score = 0;
        this.numberOfLives = 3;
        this.maxNumberOfDroppedBombs = 1;
    }
}
