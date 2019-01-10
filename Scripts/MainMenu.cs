﻿using System;
using Godot;

public class MainMenu : Node {
    private Button newGameButton;
    private Button survivalButton;
    private Button leaderboardButton;
    private Button optionsButton;
    private Button creditsButton;
    private Button exitButton;

    public override void _Ready() {
        this.newGameButton = GetTree().GetRoot().GetNode("MainMenu/newgame") as Button;
        this.survivalButton = GetTree().GetRoot().GetNode("MainMenu/survival") as Button;
        this.leaderboardButton = GetTree().GetRoot().GetNode("MainMenu/leaderboard") as Button;
        this.optionsButton = GetTree().GetRoot().GetNode("MainMenu/options") as Button;
        this.creditsButton = GetTree().GetRoot().GetNode("MainMenu/credits") as Button;
        this.exitButton = GetTree().GetRoot().GetNode("MainMenu/exit") as Button;
    }

    public override void _PhysicsProcess(float delta) {
        if (this.newGameButton.Pressed) {
            GetTree().ChangeScene("res://Scenes/Level1.tscn");
        } else if (this.survivalButton.Pressed) {

        } else if (this.leaderboardButton.Pressed) {
            GetTree().ChangeScene("res://Scenes/leaderboard.tscn");
        } else if (this.optionsButton.Pressed) {
            GetTree().ChangeScene("res://Scenes/options.tscn");
        } else if (this.creditsButton.Pressed) {

        } else if (this.exitButton.Pressed) {
            GetTree().Quit();
        }
    }
}
