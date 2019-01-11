using Godot;
using System;

public struct EnemyCountConfig {
    public int GetEnemyCount() {
        return this.numberOfBalloons + this.numberOfShrooms + this.numberOfBarrels; 
    }

    public int numberOfBalloons;
    public int numberOfShrooms;
    public int numberOfBarrels;
}

public class SceneVariables : Node {
    public Node CurrentScene { get; set; }
    public int numberOfLives;
    public int playerMovementSpeed;

    EnemyCountConfig level1Config;
    EnemyCountConfig level2Config;
    EnemyCountConfig level3Config;

    public int numberOfPowerUps;
    public float powerup1DropChance;
    public float powerup2DropChance;
    public float powerup3DropChance;

    public float maxRandomCellsPercentage;

    public int score;

    public int bombRange;
    public int maxNumberOfDroppedBombs;

    public int currentLevel;

    //CHEATS
    public bool aiDisabled;
    public bool godMode;

    public int coinCountToSpawn;
    public int ghostCountToSpawnOnExitExplosion;

    public override void _Ready() {
        Viewport root = GetTree().GetRoot();
        this.CurrentScene = root.GetChild(root.GetChildCount() - 1);

        level1Config.numberOfBalloons = 6;
        level1Config.numberOfShrooms = 0;
        level1Config.numberOfBarrels = 0;

        level2Config.numberOfBalloons = 4;
        level2Config.numberOfShrooms = 3;
        level2Config.numberOfBarrels = 0;

        level3Config.numberOfBalloons = 3;
        level3Config.numberOfShrooms = 3;
        level3Config.numberOfBarrels = 3;

        this.numberOfPowerUps = 3;

        this.currentLevel = 1; 

        switch(this.currentLevel) {
            case 1: {
                    this.powerup1DropChance = 1.0f; 
                    this.powerup2DropChance = 0.85f;
                    this.powerup3DropChance = 0.75f; 
                    break;
                }
            case 2: {
                    this.powerup1DropChance = 0.85f;
                    this.powerup2DropChance = 0.75f;
                    this.powerup3DropChance = 0.65f;
                    break;
                }
            case 3: {
                    this.powerup1DropChance = 0.66f;
                    this.powerup2DropChance = 0.66f;
                    this.powerup3DropChance = 0.66f;
                    break;
                }
            default: {
                    this.powerup1DropChance = 0.5f;
                    this.powerup2DropChance = 0.25f;
                    this.powerup3DropChance = 0.1f;
                    break;
                }
        }

        this.maxRandomCellsPercentage = 0.35f;

        this.playerMovementSpeed = 190;

        this.maxNumberOfDroppedBombs = 1;
        this.bombRange = 1;
        this.score = 0;
        this.numberOfLives = 3;

        this.coinCountToSpawn = 10;
        this.ghostCountToSpawnOnExitExplosion = 3;

        //CHEATS
        this.aiDisabled = false;
        this.godMode = false;
    }

    public EnemyCountConfig GetEnemyConfig() {
        switch(this.currentLevel) {
            case 1: {
                    return this.level1Config;
                }
            case 2: {
                    return this.level2Config;
                }
            case 3: {
                    return this.level3Config;
                }
            default: {
                    return this.level1Config;
                }
        }
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
