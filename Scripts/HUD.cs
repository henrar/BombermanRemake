using Godot;
using System;

public class HUD : CanvasLayer {
    private Label timeLeft;
    private Label score;
    private Label lives;

    private LevelTimer levelTimer;

    private SceneVariables sceneVariables;

    public override void _Ready() {
        this.sceneVariables = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;

        this.timeLeft = GetTree().GetRoot().GetNode("World/Player/PlayerCamera/HUD/interface/TimeLeft") as Label;
        this.score = GetTree().GetRoot().GetNode("World/Player/PlayerCamera/HUD/interface/CurrentScore") as Label;
        this.lives = GetTree().GetRoot().GetNode("World/Player/PlayerCamera/HUD/interface/LivesLeft") as Label;

        this.levelTimer = GetTree().GetRoot().GetNode("World/LevelTimer") as LevelTimer;
    }

    public override void _PhysicsProcess(float delta) {
        this.timeLeft.SetText(Convert.ToInt32(this.levelTimer.GetTimeLeft()).ToString());
        this.score.SetText(this.sceneVariables.score.ToString());
        this.lives.SetText(this.sceneVariables.numberOfLives.ToString());
    }
}
