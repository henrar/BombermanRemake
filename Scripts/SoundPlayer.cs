using System;
using Godot;

public enum SoundEffect {
    Explosion,
    Step,
    PlantBomb,
    Death,
    Doors,
    Achievement,
    LastEnemy,
    PushMenu,
    EnemyDeath
}

public enum Music {
    NoMusic,
    Main,
    LoadingLevel,
    Menu
}

public class SoundPlayer : Node {
    public AudioStreamPlayer musicPlayer;
    private AudioStreamSample musicStream;
    private Music currentTrack;

    private System.Collections.Generic.Dictionary<SoundEffect, AudioStreamPlayer> effectPlayers;

    public override void _Ready() {
        InitMusicStream();

        this.effectPlayers = new System.Collections.Generic.Dictionary<SoundEffect, AudioStreamPlayer>();
        RegisterSoundEffectsPlayers();
    }

    private void InitMusicStream() {
        this.currentTrack = Music.NoMusic;

        this.musicPlayer = new AudioStreamPlayer();
        this.musicStream = new AudioStreamSample();

        this.musicStream.SetFormat(AudioStreamSample.FormatEnum.Format16Bits);
        this.musicStream.SetStereo(true);
        this.musicStream.SetMixRate(48000);

        this.musicPlayer.SetStream(this.musicStream);

        AddChild(this.musicPlayer);
    }

    public override void _PhysicsProcess(float delta) {

    }

    private string GetPathToMusic(Music track) {
        switch (track) {
            case Music.Main: {
                    return "res://Assets/sound/Finalsound16/main.wav";
                }
            case Music.LoadingLevel: {
                    return "res://Assets/sound/Finalsound16/loadinglevel.wav";
                }
            case Music.Menu: {
                    return "res://Assets/sound/Finalsound16/menu.wav";
                }
            default: {
                    return "";
                }
        }
    }

    public void PlayMusic(Music track) {
        StopMusic();

        this.currentTrack = track;

        Godot.File file = new Godot.File();
        Godot.Error error = file.Open(GetPathToMusic(track), (int)Godot.File.ModeFlags.Read);
        if (error != Error.Ok) {
            Console.WriteLine("Error while opening stream: " + error.ToString());
            this.currentTrack = Music.NoMusic;
            return;
        }

        this.musicStream.SetPath(GetPathToMusic(track));
        this.musicStream.Data = file.GetBuffer(file.GetLen());

        this.musicStream.SetLoopBegin(0);
        this.musicStream.SetLoopEnd(Convert.ToInt32(this.musicStream.GetLength() * 48000.0f));
        this.musicStream.SetLoopMode(AudioStreamSample.LoopModeEnum.Forward);

        file.Close();

        this.musicPlayer.Play();

    }

    public void StopMusic() {
        if (this.musicPlayer.IsPlaying()) {
            this.musicPlayer.Stop();
            this.currentTrack = Music.NoMusic;
        }
    }

    public void PlaySoundEffect(SoundEffect sound) {
        if (this.effectPlayers.ContainsKey(sound)) {
            if (!this.effectPlayers[sound].IsPlaying())
                this.effectPlayers[sound].Play();
        }
    }

    private string GetPathToSoundEffect(SoundEffect sound) {
        switch (sound) {
            case SoundEffect.Explosion: {
                    return "res://Assets/sound/Finalsound16/boom.wav";
                }
            case SoundEffect.Step: {
                    return "res://Assets/sound/Finalsound16/steps.wav";
                }
            case SoundEffect.PlantBomb: {
                    return "res://Assets/sound/Finalsound16/podkladanie.wav";
                }
            case SoundEffect.Death: {
                    return "res://Assets/sound/Finalsound16/death.wav";
                }
            case SoundEffect.Doors: {
                    return "res://Assets/sound/Finalsound16/doors.wav";
                }
            case SoundEffect.Achievement: {
                    return "res://Assets/sound/Finalsound16/achievement.wav";
                }
            case SoundEffect.LastEnemy: {
                    return "res://Assets/sound/Finalsound16/lastenemy.wav";
                }
            case SoundEffect.PushMenu: {
                    return "res://Assets/sound/Finalsound16/pushmenu.wav";
                }
            case SoundEffect.EnemyDeath: {
                    return "res://Assets/sound/Finalsound16/achievement.wav";
                }
            default: {
                    return "";
                }
        }
    }

    private AudioStreamSample CreateSoundEffectStream(SoundEffect sound) {
        AudioStreamSample effectStream = new AudioStreamSample();

        effectStream.SetFormat(AudioStreamSample.FormatEnum.Format16Bits);
        effectStream.SetStereo(true);
        effectStream.SetMixRate(48000);

        effectStream.SetPath(GetPathToSoundEffect(sound));

        Godot.File file = new Godot.File();
        Godot.Error error = file.Open(GetPathToSoundEffect(sound), (int)Godot.File.ModeFlags.Read);
        if (error != Error.Ok) {
            Console.WriteLine("Error while opening sound effect file: " + error.ToString());
            return null;
        }
        effectStream.Data = file.GetBuffer(file.GetLen());

        file.Close();

        return effectStream;
    }

    public void RegisterSoundEffectsPlayers() {
        foreach (SoundEffect effect in Enum.GetValues(typeof(SoundEffect))) {
            this.effectPlayers.Add(effect, new AudioStreamPlayer());
            this.effectPlayers[effect].SetStream(CreateSoundEffectStream(effect));
            this.AddChild(this.effectPlayers[effect]);
        }
    }
}
