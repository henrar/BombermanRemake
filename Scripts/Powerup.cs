using Godot;
using System;

public enum PowerUpType {
    Powerup_AddBombRange = 0,
    Powerup_AddBomb = 1,
    Powerup_IncreaseSpeed = 2,
    Powerup_AddLife = 3
}

public class Powerup : StaticBody2D {
    private Sprite sprite;
    public PowerUpType type;
    private SceneVariables sv;
    public bool uncovered;

    public override void _Ready() {
        this.sv = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;
        this.uncovered = false;
    }

    public void SetType(PowerUpType type) {
        this.type = type;
    }

    public void SetSpriteBasedOnType() {
        this.sprite = new Sprite();
        ImageTexture tex = new ImageTexture();

        if (this.type == PowerUpType.Powerup_AddBombRange) {
            tex.Load("res://Assets/assetyver2/powerup-power.png");
        } else if (this.type == PowerUpType.Powerup_AddBomb) {
            tex.Load("res://Assets/assetyver2/powerup-bomb.png");
        } else if (this.type == PowerUpType.Powerup_IncreaseSpeed) {
            tex.Load("res://Assets/assetyver2/powerup-speed.png");
        } else if (this.type == PowerUpType.Powerup_AddLife) {
            tex.Load("res://Assets/assetyver2/powerup-life.png");
        }

        this.sprite.SetTexture(tex);
        this.sprite.SetScale(new Vector2(0.5f, 0.5f));

        TileMap map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;
        Transform2D mapTransform = map.GetGlobalTransform();

        this.sprite.SetPosition(map.GetPositionOfTileCenter(map.FindPowerUpPosition(this)));

        AddChild(this.sprite);
    }

    public void ExecuteEffect() {
        this.sv.score += 1000;

        if (this.type == PowerUpType.Powerup_AddBombRange) {
            if (this.sv.bombRange < 6) {
                this.sv.bombRange += 1;
            }
        } else if (this.type == PowerUpType.Powerup_AddBomb) {
            if(this.sv.maxNumberOfDroppedBombs < 5) {
                this.sv.maxNumberOfDroppedBombs += 1;
            }
        } else if (this.type == PowerUpType.Powerup_IncreaseSpeed) {
            if (this.sv.playerMovementSpeed < 300) {
                this.sv.playerMovementSpeed += 20;
            }
        } else if (this.type == PowerUpType.Powerup_AddLife) {
            this.sv.numberOfLives += 1;
        }
    }
}
