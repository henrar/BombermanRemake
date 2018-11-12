using Godot;
using System;

public class Bomb : StaticBody2D {
    private Timer timer;
    private Sprite bombSprite;
    public Vector2 position;

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
        if (timer.GetTimeLeft() <= 0.0f) {
            Explode();
        }
    }

    private void Explode() {
        Console.WriteLine("BOOM!");
        Node root = GetTree().GetRoot();
        TileMap map = root.GetNode("World/TileMap") as TileMap;
        if (map != null) {
            Vector2 explosionPosition = map.WorldToMap(this.position);
            Console.WriteLine("Explosion at: " + explosionPosition);
            for(int x = (int)explosionPosition.x - range; x <= range + (int)explosionPosition.x; ++x) {
                for (int y = (int)explosionPosition.y - range; y <= range + (int)explosionPosition.y; ++y) {
                    if(x == explosionPosition.x || y == explosionPosition.y) {
                        //TODO: check whether tile is destructible or not
                        int idx = map.GetCell(x, y);
                        Console.WriteLine("IDX: " + idx);
                        if (idx == 1) {// == "mur"                          
                            map.SetCell(x, y, 0);
                        }
                    } 
                }
            }
        } else {
            Console.WriteLine("TileMap not found!");
        }
        QueueFree();
    }
}
