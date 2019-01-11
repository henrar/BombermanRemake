using Godot;
using System;

public class HUD : CanvasLayer {
    private Label timeLeft;
    private Label score;
    private Label lives;

    private LevelTimer levelTimer;

    private SceneVariables sceneVariables;

    private Sprite timeSprite;
    private Sprite lifeSprite;

    public override void _Ready() {
        this.sceneVariables = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;

        this.timeLeft = GetTree().GetRoot().GetNode("World/Player/PlayerCamera/HUD/interface/TimeLeft") as Label;
        this.score = GetTree().GetRoot().GetNode("World/Player/PlayerCamera/HUD/interface/CurrentScore") as Label;
        this.lives = GetTree().GetRoot().GetNode("World/Player/PlayerCamera/HUD/interface/LivesLeft") as Label;

        this.levelTimer = GetTree().GetRoot().GetNode("World/LevelTimer") as LevelTimer;

        this.lifeSprite = GetTree().GetRoot().GetNode("World/Player/PlayerCamera/HUD/interface/lifesprite") as Sprite;
        this.timeSprite = GetTree().GetRoot().GetNode("World/Player/PlayerCamera/HUD/interface/timesprite") as Sprite;

        ReplaceLifeSprite();
    }

    public override void _PhysicsProcess(float delta) {
        if (this.timeLeft != null) {
            this.timeLeft.SetText(Convert.ToInt32(this.levelTimer.GetTimeLeft()).ToString());
        }
        if (this.score != null) {
            this.score.SetText(this.sceneVariables.score.ToString());
        }

        if (this.lives != null) {
            this.lives.SetText(this.sceneVariables.numberOfLives.ToString());
        }
    }

    private void ReplaceLifeSprite() {
        ImageTexture tex = new ImageTexture();

        int lifeCount = this.sceneVariables.numberOfLives;
        if (lifeCount == 0) {
            lifeCount = 1;
        } else if(lifeCount > 3) {
            lifeCount = 3;
        }

        tex.Load("res://Assets/hud/zyciex" + lifeCount + ".png");
        this.lifeSprite.SetTexture(tex);
    }
}
