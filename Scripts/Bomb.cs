using Godot;
using System;

public class Bomb : StaticBody2D {
    private Timer timer;
    public Vector2 position;
    private CollisionShape2D collision;
    private TileMap map;

    private World world;
    private SoundPlayer soundPlayer;
    private SceneVariables sceneVariables;
    private BombAnimatedSprite bombAnimatedSprite;
    private BombExplosionSprites bombExplosionSprites;

    private bool killedPlayer;

    public override void _Ready() {
        this.world = GetTree().GetRoot().GetNode("World") as World;
        this.sceneVariables = GetTree().GetRoot().GetNode("SceneVariables") as SceneVariables;
        this.soundPlayer = this.world.soundPlayer;
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;

        this.bombAnimatedSprite = new BombAnimatedSprite();
        AddChild(this.bombAnimatedSprite);
        this.bombAnimatedSprite.SetBombAnimation();

        this.bombExplosionSprites = new BombExplosionSprites();
        this.world.AddChild(this.bombExplosionSprites);

        this.timer = new Timer();
        this.timer.SetWaitTime(2.0f);
        this.timer.SetOneShot(true);
        this.timer.SetAutostart(false);
        this.timer.Start();
        AddChild(this.timer);

        SetZIndex(this.map.GetZIndex() + 5);
        this.bombAnimatedSprite.SetZIndex(this.map.GetZIndex() + 5);

        this.killedPlayer = false;
    }

    public override void _PhysicsProcess(float delta) {
        Player player = GetTree().GetRoot().GetNode("World/Player") as Player;
        if (player == null) {
            return;
        }

        if (this.map == null) {
            return;
        }

        if (this.collision == null
            && player.GetPositionOnTileMap() != TileMap.invalidTile
            && player.GetPositionOnTileMap() != this.map.droppedBombPositions[this]
            && this.position.DistanceTo(player.GetGlobalPosition() - this.map.GetGlobalTransform().Origin) >= 80.0f) { //when we leave the tile that contains bomb, we should turn on collision as in the original
            AddCollision();
        }

        this.bombAnimatedSprite.SwapBombTexture(this.timer.GetTimeLeft());
       
        if (this.timer != null && this.timer.GetTimeLeft() <= 0.0f) {
            Explode();
        }
    }

    private void ExplodePlayer(Vector2 tile) {
        Player player = GetTree().GetRoot().GetNode("World/Player") as Player;
        if (player == null) {
            Console.WriteLine("Something went terribly wrong");
        }

        Vector2 playerPosition = player.GetPositionOnTileMap();
        if (playerPosition == tile && !this.killedPlayer) {
            Console.WriteLine("Player exploded!");
            this.killedPlayer = true;
            player.Die();
        }
    }

    private void ExplodeEnemy(Vector2 tile) {
        Enemy enemy = this.map.FindEnemyOnTile(tile);
        if (enemy != null) {
            this.bombExplosionSprites.SetSplashSprite(this.map.GetPositionOfTileCenter(tile), SplashType.Enemy);
            this.map.RemoveEnemyEntry(enemy);
            this.soundPlayer.PlaySoundEffect(SoundEffect.EnemyDeath);
            enemy.Die();
        }
    }

    private void ExplodeExitTile(Vector2 tile) {
        if (this.map.exitTile.positionOnTileMap == tile && this.map.exitTileUncovered) {
            this.map.exitTile.exploded = true;
            this.map.exitTile.LoadTexture();
            this.map.SpawnEnemyAtLocation(EnemyType.Ghost, this.map.exitTile.positionOnTileMap, this.sceneVariables.ghostCountToSpawnOnExitExplosion);
        }
    }

    private bool ExecuteExplosionAtTile(Vector2 tile) {
        if (this.map.IsWall(tile)) {
            return false;
        }

        if (!this.sceneVariables.godMode) {
            ExplodePlayer(tile);
        }
        ExplodeEnemy(tile);
        ExplodeExitTile(tile);

        if (this.map.GetCellv(tile) == (int)TileType.TileType_Bricks) {
            this.map.SetCellv(tile, (int)TileType.TileType_Floor);
            this.bombExplosionSprites.SetSplashSprite(this.map.GetPositionOfTileCenter(tile), SplashType.Bricks);
            this.map.UncoverPowerUp(tile);
        }

        if (this.map.GetRemainingEnemiesCount() == 0) {
            this.soundPlayer.PlaySoundEffect(SoundEffect.LastEnemy);
        }

        return true;
    }

    private void Explode() {
        Console.WriteLine("BOOM!");

        this.soundPlayer.PlaySoundEffect(SoundEffect.Explosion);

        Vector2 explosionPosition = this.map.WorldToMap(this.position);
        Console.WriteLine("Explosion at: " + explosionPosition);

        this.bombExplosionSprites.SetTimer();

        Player player = GetTree().GetRoot().GetNode("World/Player") as Player;
        if (player != null) {
            player.numberOfDroppedBombs -= 1;
        }

        this.bombExplosionSprites.SetExplosionSprite(this.position, Directions.noDirection, false);

        for (int x = (int)explosionPosition.x; x <= (int)explosionPosition.x + this.sceneVariables.bombRange; ++x) {
            Vector2 tile = new Vector2(x, explosionPosition.y);
            if (!ExecuteExplosionAtTile(tile)) {
                break;
            }
            if (x != (int)explosionPosition.x) {
                this.bombExplosionSprites.SetExplosionSprite(this.map.GetPositionOfTileCenter(tile), Directions.directionRight, false);
            }
        }

        for (int x = (int)explosionPosition.x; x >= (int)explosionPosition.x - this.sceneVariables.bombRange; --x) {
            Vector2 tile = new Vector2(x, explosionPosition.y);
            if (!ExecuteExplosionAtTile(tile)) {
                break;
            }
            if (x != (int)explosionPosition.x) {
                this.bombExplosionSprites.SetExplosionSprite(this.map.GetPositionOfTileCenter(tile), Directions.directionLeft, false);
            }
        }

        for (int y = (int)explosionPosition.y; y <= (int)explosionPosition.y + this.sceneVariables.bombRange; ++y) {
            Vector2 tile = new Vector2(explosionPosition.x, y);
            if (!ExecuteExplosionAtTile(tile)) {
                break;
            }
            if (y != (int)explosionPosition.y) {
                this.bombExplosionSprites.SetExplosionSprite(this.map.GetPositionOfTileCenter(tile), Directions.directionDown, false);
            }
        }

        for (int y = (int)explosionPosition.y; y >= (int)explosionPosition.y - this.sceneVariables.bombRange; --y) {
            Vector2 tile = new Vector2(explosionPosition.x, y);
            if (!ExecuteExplosionAtTile(tile)) {
                break;
            }
            if (y != (int)explosionPosition.y) {
                this.bombExplosionSprites.SetExplosionSprite(this.map.GetPositionOfTileCenter(tile), Directions.directionUp, false);
            }
        }

        this.map.droppedBombPositions.Remove(this);
        QueueFree();
    }

    public void AddCollision() {
        this.collision = new CollisionShape2D();
        CircleShape2D shape = new CircleShape2D();
        shape.SetRadius(40.0f);
        this.collision.SetShape(shape);
        Console.WriteLine(this.position);
        this.collision.SetName("BombCollision");
        AddChild(this.collision);
        this.collision.SetGlobalPosition(this.position + this.map.GetGlobalTransform().Origin);
    }

    public void SetBombSpritePosition(Vector2 pos) {
        this.bombAnimatedSprite.SetGlobalPosition(pos);
    }
}

