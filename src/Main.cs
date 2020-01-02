// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();
	private PackedScene carPS = GD.Load<PackedScene>("res://scenes/Car.tscn");
	private AudioStream roadAudio = GD.Load<AudioStream>("res://assets/road.ogg");
	private readonly float spawnRateLimit = 0.0003f;
	private SpatialMaterial vMat;
	private SpatialMaterial hMat;

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		var env = GetNode<WorldEnvironment>("sky").Environment;
		env.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);

		InitSound();
		AddChild(Audio);
		roadAudio.Play(Audio);
		Audio.Seek((float)GD.RandRange(0, roadAudio.GetLength()));

		GetNode<Spatial>("Blockers").Translate(new Vector3(0, (float)GD.RandRange(-9, 9), 0));

		vMat = new SpatialMaterial() {
			EmissionEnabled = true,
			EmissionEnergy = 1
		};
		hMat = new SpatialMaterial() {
			EmissionEnabled = true,
			EmissionEnergy = 1
		};

		GetNode<MeshInstance>("VLight/StopLight").SetSurfaceMaterial(0, vMat);
		GetNode<MeshInstance>("VLight2/StopLight").SetSurfaceMaterial(0, vMat);
		GetNode<MeshInstance>("HLight/StopLight").SetSurfaceMaterial(0, hMat);
		GetNode<MeshInstance>("HLight2/StopLight").SetSurfaceMaterial(0, hMat);

	}

	public override void _Process(float delta) {
		var blockers = GetNode<Spatial>("Blockers");
		blockers.Translate(new Vector3(0, 1f * delta, 0));
		if (blockers.Transform.origin.y > 10) {
			blockers.Translate(new Vector3(0, -20, 0));
		}

		if (GD.Randf() * delta < spawnRateLimit) {
			AddChild(carPS.Instance());
		}

		if (blockers.Transform.origin.y > 0) {
			vMat.AlbedoColor = Colors.Red;
			vMat.Emission = Colors.Red;
			hMat.AlbedoColor = Colors.Green;
			hMat.Emission = Colors.Green;
		} else {
			vMat.AlbedoColor = Colors.Green;
			vMat.Emission = Colors.Green;
			hMat.AlbedoColor = Colors.Red;
			hMat.Emission = Colors.Red;
		}
	}

}
