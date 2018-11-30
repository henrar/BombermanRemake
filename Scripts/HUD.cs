using Godot;
using System;

public class HUD : CanvasLayer {
    private Label timeLeft;
    private LevelTimer levelTimer;

    public override void _Ready() {
        this.timeLeft = GetTree().GetRoot().GetNode("World/Player/PlayerCamera/HUD/interface/TimeLeft") as Label;
        this.levelTimer = GetTree().GetRoot().GetNode("World/LevelTimer") as LevelTimer;
    }

    public override void _PhysicsProcess(float delta) {
        this.timeLeft.SetText(Convert.ToInt32(this.levelTimer.GetTimeLeft()).ToString());
    }
}
