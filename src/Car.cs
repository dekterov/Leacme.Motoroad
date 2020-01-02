using Godot;
using System;
using System.Linq;

public class Car : Spatial {

	private enum Quadrant {
		NE, SE, SW, NW
	}

	private Quadrant spawnQuadrant;
	private float currVel = 0;
	private Vector3 rayDir = new Vector3(0, 0, 0);
	private const int spawnLimit = 30;
	private const int despawnLimit = 50;
	private const float laneSpacing = 2.5f;
	private const float rayDist = 2.5f;

	public override void _Ready() {
		spawnQuadrant = Enum.GetValues(typeof(Quadrant)).Cast<Quadrant>().ElementAt((int)GD.RandRange(0, Enum.GetValues(typeof(Quadrant)).Length));

		switch (spawnQuadrant) {
			case Quadrant.NE:
				Translate(new Vector3(laneSpacing, 0, spawnLimit));
				GetNode<StaticBody>("CarPhysics").RotateY(Mathf.Deg2Rad(90));
				break;
			case Quadrant.SE:
				Translate(new Vector3(-spawnLimit, 0, laneSpacing));
				break;
			case Quadrant.SW:
				Translate(new Vector3(-laneSpacing, 0, -spawnLimit));
				GetNode<StaticBody>("CarPhysics").RotateY(Mathf.Deg2Rad(-90));
				break;
			case Quadrant.NW:
				Translate(new Vector3(spawnLimit, 0, -laneSpacing));
				GetNode<StaticBody>("CarPhysics").RotateY(Mathf.Deg2Rad(180));
				break;
		}

		// GetNode<MeshInstance>("CarPhysics/CarCollision/Car").MaterialOverride = new SpatialMaterial() {
		// 	AlbedoColor = Colors.White
		// };

		// GD.Print(GetNode<MeshInstance>("CarPhysics/CarCollision/Car").GetSurfaceMaterialCount());


		GetNode<MeshInstance>("CarPhysics/CarCollision/Car").SetSurfaceMaterial(2, new SpatialMaterial() {
			AlbedoColor = Color.FromHsv(0, 0, (float)GD.RandRange(0.3f, 1))
		});


		GetNode<MeshInstance>("CarPhysics/CarCollision/Car").SetSurfaceMaterial(1, new SpatialMaterial() {
			EmissionEnabled = true,
			EmissionEnergy = 10,
			Emission = Colors.White
		});

	}

	public override void _PhysicsProcess(float delta) {
		switch (spawnQuadrant) {
			case Quadrant.NE:
				rayDir = new Vector3(0, 0, -rayDist);
				break;
			case Quadrant.SE:
				rayDir = new Vector3(rayDist, 0, 0);
				break;
			case Quadrant.SW:
				rayDir = new Vector3(0, 0, rayDist);
				break;
			case Quadrant.NW:
				rayDir = new Vector3(-rayDist, 0, 0);
				break;
		}

		var hitObj = GetWorld().DirectSpaceState.IntersectRay(Translation, Translation + rayDir, new Godot.Collections.Array { this });

		if (hitObj.Count > 0) {
			if (currVel > 0) {
				currVel -= 0.25f * delta;

				// GetNode<MeshInstance>("CarPhysics/CarCollision/Car").MaterialOverride = new SpatialMaterial() {
				// 	AlbedoColor = Colors.Red
				// };

				GetNode<MeshInstance>("CarPhysics/CarCollision/Car").SetSurfaceMaterial(0, new SpatialMaterial() {
					AlbedoColor = Colors.Red,
					EmissionEnabled = true,
					EmissionEnergy = 10,
					Emission = Colors.Red
				});

				if (currVel <= 0) {
					currVel = 0;
				}
			}
		} else {
			if (currVel <= 0.08f) {
				currVel += 0.03f * delta;
			}

			GetNode<MeshInstance>("CarPhysics/CarCollision/Car").SetSurfaceMaterial(0, null);

		}

		switch (spawnQuadrant) {
			case Quadrant.NE:
				Translate(new Vector3(0, 0, -currVel));
				break;
			case Quadrant.SE:
				Translate(new Vector3(currVel, 0, 0));
				break;
			case Quadrant.SW:
				Translate(new Vector3(0, 0, currVel));
				break;
			case Quadrant.NW:
				Translate(new Vector3(-currVel, 0, 0));
				break;
		}

		if (Transform.origin.z > despawnLimit || Transform.origin.z < -despawnLimit || Transform.origin.x > despawnLimit || Transform.origin.x < -despawnLimit) {
			QueueFree();
		}

	}

}
