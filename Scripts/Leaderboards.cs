using Godot;

public class Leaderboards: Node {
    private SoundPlayer soundPlayer;

    public override void _Ready() {
        this.soundPlayer = new SoundPlayer();
        AddChild(this.soundPlayer);
        this.soundPlayer.PlayMusic(Music.Menu);
    }

    public override void _PhysicsProcess(float delta) {
        if (Input.IsActionPressed("ui_cancel")) {
            GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
        }
    }
}