using Godot;

public class Pause: Node {
    private Button continueButton;
    private Button optionsButton;
    private Button helpButton;
    private Button leaderboardButton;
    private Button exitButton;

    public override void _Ready() {
        this.continueButton = GetTree().GetRoot().GetNode("MainMenu/Continue") as Button;
        this.optionsButton = GetTree().GetRoot().GetNode("MainMenu/Options") as Button;
        this.helpButton = GetTree().GetRoot().GetNode("MainMenu/Help") as Button;
        this.leaderboardButton = GetTree().GetRoot().GetNode("MainMenu/Leaderboard") as Button;
        this.exitButton = GetTree().GetRoot().GetNode("MainMenu/Exit") as Button;
    }

    public override void _PhysicsProcess(float delta) {
        
    }
}
