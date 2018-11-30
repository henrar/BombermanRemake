using Godot;
using System;

public class Bomb : StaticBody2D {
    private Timer timer;
    private Sprite bombSprite;
    public Vector2 position;
    private CollisionShape2D collision;
    private TileMap map;

    public int range = 1;

    public override void _Ready() {
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;

        this.bombSprite = new Sprite();
        ImageTexture bombTex = new ImageTexture();
        bombTex.Load("res://Assets/bomb.png");
        this.bombSprite.SetTexture(bombTex);
        this.bombSprite.SetPosition(GetPosition());

        AddChild(this.bombSprite);

        this.timer = new Timer();
        this.timer.SetWaitTime(2.0f);
        this.timer.SetOneShot(true);
        this.timer.SetAutostart(false);
        this.timer.Start();
        AddChild(this.timer);

        Vector2 pos = new Vector2(0, 0);
        SetPosition(pos);

        this.collision = new CollisionShape2D();

        CircleShape2D shape = new CircleShape2D();
        shape.SetRadius(40.0f);
        this.collision.SetShape(shape);

        this.collision.SetPosition(position + this.map.GetGlobalTransform().Origin);

        this.collision.SetName("BombCollision");
    }

    public override void _PhysicsProcess(float delta) {
        Player player = GetTree().GetRoot().GetNode("World/Player") as Player;
        if(player == null) {
            return;
        }

        if (this.position != null && player.GetPositionOnTileMap() != this.map.WorldToMap(this.position) && this.position.DistanceTo(player.GetGlobalPosition() - this.map.GetGlobalTransform().Origin) >= 80.0f) { //when we leave the tile that contains bomb, we should turn on collision as in the original
            AddCollision();
        }

        if (timer.GetTimeLeft() <= 0.0f) {
            Explode();
        }
    }

    private void ExplodePlayer(Vector2 tile) {
        Player player = GetTree().GetRoot().GetNode("World/Player") as Player;
        if (player == null) {
            Console.WriteLine("Something went terribly wrong");
        }

        Vector2 playerPosition = player.GetPositionOnTileMap();
        if (playerPosition == tile) {
            Console.WriteLine("Player exploded!");
            player.Die();
        }
    }

    private void ExplodeEnemy(Vector2 tile) {
        Enemy enemy = this.map.FindEnemyOnTile(tile);
        if (enemy != null) {
            this.map.RemoveEnemyEntry(enemy);
            enemy.Die();
        }
    }

    private bool ExecuteExplosionAtTile(Vector2 tile) {
        if (this.map.GetCellv(tile) == (int)TileType.TileType_Wall) {
            return false;
        }
        ExplodePlayer(tile);
        ExplodeEnemy(tile);

        if (map.GetCellv(tile) == (int)TileType.TileType_Bricks) {
            map.SetCellv(tile, (int)TileType.TileType_Grass);
        }
        return true;
    }

    private void Explode() {
        Console.WriteLine("BOOM!");

        AudioStreamPlayer2D soundPlayer = GetTree().GetRoot().GetNode("World/ExplosionSound") as AudioStreamPlayer2D;
        soundPlayer.Play();

        Vector2 explosionPosition = map.WorldToMap(this.position);
        Console.WriteLine("Explosion at: " + explosionPosition);

        for(int x = (int)explosionPosition.x; x <= (int)explosionPosition.x + range; ++x) {
            Vector2 tile = new Vector2(x, explosionPosition.y);
            if(!ExecuteExplosionAtTile(tile)) {
                break;
            }
        }

        for(int x = (int)explosionPosition.x; x >= (int)explosionPosition.x - range; --x) {
            Vector2 tile = new Vector2(x, explosionPosition.y);
            if (!ExecuteExplosionAtTile(tile)) {
                break;
            }
        }

        for (int y = (int)explosionPosition.y; y <= (int)explosionPosition.y + range; ++y) {
            Vector2 tile = new Vector2(explosionPosition.x, y);
            if (!ExecuteExplosionAtTile(tile)) {
                break;
            }
        }

        for (int y = (int)explosionPosition.y; y >= (int)explosionPosition.y - range; --y) {
            Vector2 tile = new Vector2(explosionPosition.x, y);
            if (!ExecuteExplosionAtTile(tile)) {
                break;
            }
        }

        this.map.droppedBombPosition = TileMap.invalidTile;

        QueueFree();
    }

    public void AddCollision() {
        if(this.collision != null && this.GetNode("BombCollision") == null) {
            AddChild(this.collision);
        }
    }
}

