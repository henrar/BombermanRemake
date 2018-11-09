using Godot;
using System;

public class Bomb : StaticBody2D {
    private Timer timer;
    private Sprite bombSprite;
    private int range = 1;

    public Bomb() {

    }

    public override void _Ready() {
        bombSprite = new Sprite();
        ImageTexture bombTex = new ImageTexture();
        bombTex.Load("res://Assets/bomb.png");
        bombSprite.SetTexture(bombTex);
        bombSprite.SetPosition(GetPosition());

        AddChild(bombSprite);

        timer = new Timer();
        timer.SetWaitTime(2.0f);
        timer.SetOneShot(true);
        timer.SetAutostart(false);
        timer.Start();
        AddChild(timer);

        Vector2 pos = new Vector2(0, 0);
        SetPosition(pos);
    }

    public override void _Process(float delta) {
        if(timer.GetTimeLeft() > 0.0f) {
            Console.WriteLine("Tick... {0}", timer.GetTimeLeft());
        }
        if (timer.GetTimeLeft() <= 0.0f) {
            Explode();
        }
    }

    private void Explode() {
        Console.WriteLine("BOOM!");
        //GetParent().RemoveChild(this);
        QueueFree();
    }
}
