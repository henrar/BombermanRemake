using Godot;

public class BombExplosionSprites: Node2D {
    private Timer displayTimer;

    public readonly string pathToSplash = "res://Assets/explosion_anim/splash.png";
    public readonly string pathToDestroyBox = "res://Assets/explosion_anim/explosion_anim_2/boxdestroy.png";
    public readonly string pathToExplosionCenter = "res://Assets/explosion_anim/boom-cloud.png";

    public readonly string pathToExplosionLeft = "res://Assets/explosion_anim/zasiegwybuchulewo.png";
    public readonly string pathToExplosionRight = "res://Assets/explosion_anim/zasiegwybuchuprawo.png";
    public readonly string pathToExplosionUp = "res://Assets/explosion_anim/zasiegwybuchugora.png";
    public readonly string pathToExplosionDown = "res://Assets/explosion_anim/zasiegwybuchudol.png";

    private ImageTexture splash;
    private ImageTexture destroyBox;
    private ImageTexture explosionCenter;
    private ImageTexture explosionLeft;
    private ImageTexture explosionRight;
    private ImageTexture explosionUp;
    private ImageTexture explosionDown;

    private TileMap map;

    public override void _Ready() {
        this.map = GetTree().GetRoot().GetNode("World/TileMap") as TileMap;

        this.splash = new ImageTexture();
        this.splash.Load(this.pathToSplash);

        this.destroyBox = new ImageTexture();
        this.destroyBox.Load(this.pathToDestroyBox);

        this.explosionCenter = new ImageTexture();
        this.explosionCenter.Load(this.pathToExplosionCenter);
        this.explosionLeft = new ImageTexture();
        this.explosionLeft.Load(this.pathToExplosionLeft);
        this.explosionRight = new ImageTexture();
        this.explosionRight.Load(this.pathToExplosionRight);
        this.explosionUp = new ImageTexture();
        this.explosionUp.Load(this.pathToExplosionUp);
        this.explosionDown = new ImageTexture();
        this.explosionDown.Load(this.pathToExplosionDown);
    }

    public override void _PhysicsProcess(float delta) {
        if (this.displayTimer != null && this.displayTimer.GetTimeLeft() <= 0.0f) {
            QueueFree();
        }
    }

    public void SetExplosionSprite(Vector2 position, Vector2 direction, bool isEnd) {
        Sprite sprite = new Sprite();

        if (direction == Directions.noDirection) {
            sprite.SetTexture(this.explosionCenter);
        } else if (direction == Directions.directionLeft) {
            sprite.SetTexture(this.explosionLeft);
        } else if (direction == Directions.directionRight) {
            sprite.SetTexture(this.explosionRight);
        } else if (direction == Directions.directionUp) {
            sprite.SetTexture(this.explosionUp);
        } else if (direction == Directions.directionDown) {
            sprite.SetTexture(this.explosionDown);
        }

        if (isEnd) {
            sprite.SetScale(new Vector2(1.25f, 1.25f));
        }

        sprite.SetZIndex(this.map.GetZIndex() + 10);
        Transform2D mapTransform = this.map.GetTransform();
        sprite.SetGlobalPosition(position + mapTransform.Origin);
        AddChild(sprite);
    }

    public void SetTimer() {
        this.displayTimer = new Timer();
        this.displayTimer.SetWaitTime(0.2f);
        this.displayTimer.SetOneShot(true);
        this.displayTimer.SetAutostart(false);
        this.displayTimer.Start();
        AddChild(this.displayTimer);
    }
}