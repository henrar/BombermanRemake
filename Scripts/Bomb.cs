using Godot;
using System;

public class Bomb : StaticBody2D {
    private Timer timer;
    private Sprite bombSprite;
    public Vector2 position;

    public int range = 1;

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

    public override void _PhysicsProcess(float delta) {
        if (timer.GetTimeLeft() <= 0.0f) {
            Explode();
        }
    }

    private void Explode() {
        Console.WriteLine("BOOM!");
        Node root = GetTree().GetRoot();

        AudioStreamPlayer2D soundPlayer = root.GetNode("World/ExplosionSound") as AudioStreamPlayer2D;
        soundPlayer.Play();

        Player player = root.GetNode("World/Player") as Player;
        if(player == null) {
            Console.WriteLine("Something went terribly wrong");
        }

        Vector2 playerPosition = player.GetPositionOnTileMap();

        bool playerExploded = false;

        TileMap map = root.GetNode("World/Nav/TileMap") as TileMap;
        if (map != null) {
            Vector2 explosionPosition = map.WorldToMap(this.position);
            Console.WriteLine("Explosion at: " + explosionPosition);
            for (int x = (int)explosionPosition.x - range; x <= range + (int)explosionPosition.x; ++x) {
                if (playerExploded == true) {
                    break;
                }
                for (int y = (int)explosionPosition.y - range; y <= range + (int)explosionPosition.y; ++y) {
                    if(playerExploded == true) {
                        break;
                    }
                    if ((x == explosionPosition.x || y == explosionPosition.y) && (x >= 0 && y >= 0)) {
                        if (map.GetCell(x, y) == (int)TileTypes.TileType_Bricks) {// == "mur"                          
                            map.SetCell(x, y, (int)TileTypes.TileType_Grass);
                        }

                        if(playerPosition == new Vector2(x, y)) {
                            Console.WriteLine("Player exploded!");
                            player.Die();
                            playerExploded = true;
                        }
                        Enemy enemy = map.findEnemyOnCell(new Vector2(x, y));
                        if (enemy != null) {
                            map.removeEnemyEntry(enemy);
                            enemy.Die();
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
