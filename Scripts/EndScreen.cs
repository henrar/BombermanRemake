using Godot;

public class EndScreen: Node {
    private SoundPlayer soundPlayer;

    public override void _Ready() {
        this.soundPlayer = new SoundPlayer();
        AddChild(this.soundPlayer);
        this.soundPlayer.PlayMusic(Music.Menu);
    }

    public override void _PhysicsProcess(float delta) {
        if (Input.IsActionPressed("ui_accept") || Input.IsActionPressed("ui_cancel") || Input.IsActionPressed("ui_select")) {
            GetTree().ChangeScene("res://Scenes/MainMenu.tscn");
        }
    }
}