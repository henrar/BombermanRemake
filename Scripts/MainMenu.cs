using System;
using Godot;

public class MainMenu : Node {
    private Button newGameButton;
    private Button survivalButton;
    private Button leaderboardButton;
    private Button optionsButton;
    private Button creditsButton;
    private Button exitButton;
    private SceneVariables sceneVariables;
    private SoundPlayer soundPlayer;

    public override void _Ready() {
        this.newGameButton = GetTree().GetRoot().GetNode("MainMenu/newgame") as Button;
        this.survivalButton = GetTree().GetRoot().GetNode("MainMenu/survival") as Button;
        this.leaderboardButton = GetTree().GetRoot().GetNode("MainMenu/leaderboard") as Button;
        this.optionsButton = GetTree().GetRoot().GetNode("MainMenu/options") as Button;
        this.creditsButton = GetTree().GetRoot().GetNode("MainMenu/credits") as Button;
        this.exitButton = GetTree().GetRoot().GetNode("MainMenu/exit") as Button;

        this.sceneVariables = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;

        this.soundPlayer = GetTree().GetRoot().GetNode("SoundPlayer") as SoundPlayer;
        this.soundPlayer.PlayMusic(Music.Menu);
    }

    public override void _PhysicsProcess(float delta) {
        if (this.newGameButton.Pressed) {
            this.soundPlayer.PlaySoundEffect(SoundEffect.PushMenu);
            this.sceneVariables.currentLevel = 1;
            GetTree().ChangeScene("res://Scenes/Level1.tscn");
        } else if (this.survivalButton.Pressed) {
            this.soundPlayer.PlaySoundEffect(SoundEffect.PushMenu);
        } else if (this.leaderboardButton.Pressed) {
            this.soundPlayer.PlaySoundEffect(SoundEffect.PushMenu);
            GetTree().ChangeScene("res://Scenes/leaderboard.tscn");
        } else if (this.optionsButton.Pressed) {
            this.soundPlayer.PlaySoundEffect(SoundEffect.PushMenu);
            GetTree().ChangeScene("res://Scenes/options.tscn");
        } else if (this.creditsButton.Pressed) {
            this.soundPlayer.PlaySoundEffect(SoundEffect.PushMenu);
            GetTree().ChangeScene("res://Scenes/credits.tscn");
        } else if (this.exitButton.Pressed) {
            this.soundPlayer.PlaySoundEffect(SoundEffect.PushMenu);
            GetTree().Quit();
        }
    }
}

