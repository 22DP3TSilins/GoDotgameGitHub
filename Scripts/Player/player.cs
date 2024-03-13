using Godot;
using System;


public partial class player : CharacterBody3D
{
	[Export] public float acceleration = 1.0f / 0.5f;
	[Export] public float stopping_acceleration = 10.0f;
	[Export] public float mouseSensitivity = 0.003f;
	Vector3 velocity = Vector3.Zero;
	Camera3D camera = null;
	Node3D cameraRotX = null;
	RayCast3D cameraColisionDetector = null;
	
	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		camera = GetNode<Camera3D>("CameraRig/RotY/Camera");
		cameraRotX = GetNode<Node3D>("CameraRig/RotY");
		cameraColisionDetector = GetNode<RayCast3D>("CameraRig/RotY/CameraColisionDetector");
	}
	
	public const float Speed = 5.0f;
	public const float JumpVelocity = 5.2f;
	public bool FlyMode = false;
	
	float LastJump = 1.0f;

	// Maximum Camera distance from player camera is (x = 0; y = 2; z = 4) distance is aproximatly sqrt(0^2 + 2^2 + 4^2) ~ 4.472135955
	const float MaxCameraDistance = 4.0f; 

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Process(double delta)
	{
		base._Process(delta);
		
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		float VelocityY = velocity.Y;
		
		Vector2 inputXZ = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		float inputY = Input.GetAxis("fly_up", "fly_down");
		Vector3 target_velocity = new Vector3();
		Vector3 forward = camera.GlobalTransform.Basis.Z;
		Vector3 right = camera.GlobalTransform.Basis.X;
		Vector3 up = camera.GlobalTransform.Basis.Y;
		
		target_velocity += right * inputXZ.X;
		target_velocity += forward * inputXZ.Y;
		target_velocity += up * inputY;
		target_velocity.Y = FlyMode ? target_velocity.Y : 0.0f;
		
		
		target_velocity = target_velocity.Normalized() * Speed;

		float target_acceleration = acceleration;
		if (target_velocity.LengthSquared() <= 0.1f) target_acceleration = stopping_acceleration;
		velocity = velocity.Lerp(target_velocity, target_acceleration * (float)delta);
		velocity.Y = VelocityY;
		
		if (!FlyMode) {
			// Add the gravity.
			if (!IsOnFloor()) velocity.Y -= gravity * (float)delta;

			// Handle Jump.
			if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) velocity.Y = JumpVelocity;
		}
		LastJump += (float)delta;
		if (Input.IsActionJustPressed("ui_accept")) {
			if (LastJump < 0.5f) {
				FlyMode = !FlyMode;
				LastJump = 1.0f;
				velocity.Y = 0.0f;

			} else {
				LastJump = 0.0f;
			}
		}
		Velocity = velocity;
		MoveAndSlide();
		HandleCameraCollision();
	}
	
	// move camera
	public override void _Input(InputEvent e) {
		if (e is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured) {
			var m_event = e as InputEventMouseMotion;
			Vector2 motion = -m_event.Relative * mouseSensitivity;

			GD.Print($"rotation: {cameraRotX.GlobalRotationDegrees.X}\nmotion: {motion.Y}");
			GD.Print($"full rotation: {cameraRotX.GlobalRotationDegrees.X + motion.Y}");
			RotateY(motion.X);
			// float Precision = 1.0f;
			if (cameraRotX.GlobalRotationDegrees.X + motion.Y <= -89.997f) {
				cameraRotX.RotationDegrees = new Vector3(-90.0f, 0.0f, 0.0f);
			
			} else if (cameraRotX.GlobalRotationDegrees.X + motion.Y >= 89.997f) {
				cameraRotX.RotationDegrees = new Vector3(90.0f, 0.0f, 0.0f);

			} else {
				cameraRotX.RotateX(motion.Y);
			}
			GD.Print('\n');
			HandleCameraCollision();
			
		}
		if (e.IsActionPressed("ui_cancel")) {
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}
	}

	void HandleCameraCollision(){
		cameraColisionDetector.ForceRaycastUpdate();
			Vector3 posOfCameraColision;
			if (cameraColisionDetector.IsColliding()) {
				posOfCameraColision = this.ToLocal(cameraColisionDetector.GetCollisionPoint());

				if (MaxCameraDistance > posOfCameraColision.Length()) {
					// Vector2 Current = new Vector2(posOfCameraColision.Y, posOfCameraColision.Z);
					// float angle = (float)Math.Acos(Current.Dot(new Vector2(-posOfCameraColision.Y, -posOfCameraColision.Z)));

					camera.Position = posOfCameraColision;

				} else {
					camera.Translate(new Vector3(0.0f, 0.0f, 4.0f) - camera.Position);
				}
				
			} else {
				camera.Translate(new Vector3(0.0f, 0.0f, 4.0f) - camera.Position);
			}
	}
	
	
}
