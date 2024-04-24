using Godot;
using System;


public partial class player : CharacterBody3D
{
	[Export] public float acceleration = 1.0f / 0.25f;
	[Export] public float stopping_acceleration = 20.0f;
	[Export] public float mouseSensitivity = 0.003f;
	
	Vector3 velocity = Vector3.Zero;
	Camera3D camera = null;
	Node3D cameraRotX = null;
	RayCast3D cameraColisionDetector = null;
	CanvasLayer ui = null;
	ScoreBoard scoreBoard = null;
	CanvasLayer login = null;

	
	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Visible;

		camera = GetNode<Camera3D>("CameraRig/RotY/Camera");
		cameraRotX = GetNode<Node3D>("CameraRig/RotY");
		cameraColisionDetector = GetNode<RayCast3D>("CameraRig/RotY/CameraColisionDetector");

		ui = GetNode<CanvasLayer>("../UI");
		scoreBoard = GetNode<ScoreBoard>("../Scores");
		MaxCameraDistance = (float)ui.GetNode<Slider>("Control/Panel/Control/HBoxContainer/Camera settings/CameraSettings/Distance/Input").Value;
		CameraOfset = (float)ui.GetNode<Slider>("Control/Panel/Control/HBoxContainer/Camera settings/CameraSettings/Ofset/Input").Value;

		cameraColisionDetector.TargetPosition = new Vector3(CameraOfset, CameraHight, MaxCameraDistance);
		cameraColisionDetector.HitBackFaces = false;

		login = GetNode<CanvasLayer>("../Login");
	}
	
	public const float Speed = 5.0f;
	public const float JumpVelocity = 5.2f;
	public bool FlyMode = false;
	float LastJump = 1.0f;
	bool Started = false;

	[Export] public float MaxCameraDistance; 
	[Export] public float CameraHight = 0.0f; 
	[Export] public float CameraOfset;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public override void _Process(double delta)
	{
		base._Process(delta);
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if (login.Visible || ui.Visible) {
			scoreBoard.Stop();
			HandleCameraCollision(new Vector2(0.0f, 0.0f));
			return;
		}
		//  scoreBoard.Continue();
		// StartClock();
		Vector3 velocity = Velocity;
		float VelocityY = velocity.Y;
		
		Vector2 inputXZ = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		if (!inputXZ.IsZeroApprox()) StartClock();
		// float inputY = Input.GetAxis("fly_up", "fly_down");
		Vector3 target_velocity = new Vector3();
		Vector3 forward = GlobalTransform.Basis.Z;
		Vector3 right = GlobalTransform.Basis.X;
		// Vector3 up = camera.GlobalTransform.Basis.Y;
		GD.Print($"inputXY: {inputXZ.X}, {inputXZ.Y} end");
		target_velocity += right * inputXZ.X;
		target_velocity += forward * inputXZ.Y;
		// target_velocity += up * inputY;
		// target_velocity.Y = FlyMode ? target_velocity.Y : 0.0f;
		
		
		target_velocity = target_velocity.Normalized() * Speed;
		GD.Print($"vel {velocity.X}, {velocity.Y}, {velocity.Z} vel end");

		float target_acceleration = acceleration;
		if (target_velocity.LengthSquared() <= 0.1f) target_acceleration = stopping_acceleration;
		if (!IsOnFloor()) target_acceleration /= 10;
		velocity = velocity.Lerp(target_velocity, target_acceleration * (float)delta);
		velocity.Y = VelocityY;
		
		// if (!FlyMode) {
		// Add the gravity.
		if (!IsOnFloor()) velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) {
			velocity.Y = JumpVelocity * 2;
			StartClock();
		}
		// }
		// LastJump += (float)delta;
		// if (Input.IsActionJustPressed("ui_accept")) {
			
		// 	if (LastJump < 0.5f) {
		// 		FlyMode = !FlyMode;
		// 		LastJump = 1.0f;
		// 		velocity.Y = 0.0f;

		// 	} else {
		// 		LastJump = 0.0f;
		// 	}
		// }
		Velocity = velocity;
		MoveAndSlide();
		HandleCameraCollision(new Vector2(0.0f, 0.0f));
	}
	
	public override void _Input(InputEvent e) {
		
		if (e is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured) {
			// StartClock();
			var m_event = e as InputEventMouseMotion;
			Vector2 motion = -m_event.Relative * mouseSensitivity;

			HandleCameraCollision(motion);

			cameraRotX.RotateX(motion.Y);
			if (cameraRotX.RotationDegrees.X <= -89.997f) {
				cameraRotX.RotationDegrees = new Vector3(-90.0f, 0.0f, 0.0f);
			
			} else if (cameraRotX.RotationDegrees.X >= 89.997f) {
				cameraRotX.RotationDegrees = new Vector3(90.0f, 0.0f, 0.0f);

			} else {

				camera.LookAt(ToGlobal(new Vector3(camera.Position.X, camera.Position.Y, 0.0f)), new Vector3(0.0f, 1.0f, 0.0f));
			}
		}
		if (e.IsActionPressed("ui_cancel") && !login.Visible) {
			// scoreBoard.Continue();
			StartClock();
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			ui.Visible = Input.MouseMode == Input.MouseModeEnum.Visible;
		}
	}

	void HandleCameraCollision(Vector2 motion){
		RotateY(motion.X);
		cameraColisionDetector.TargetPosition = new Vector3(CameraOfset, CameraHight, MaxCameraDistance);
		cameraColisionDetector.ForceRaycastUpdate();
		Vector3 posOfCameraColision;
		if (cameraColisionDetector.IsColliding()) {
			posOfCameraColision = ToLocal(cameraColisionDetector.GetCollisionPoint());
			float len = new Vector3(CameraOfset, CameraHight, MaxCameraDistance).Length() / posOfCameraColision.Length() + 0.01f;
			camera.Position = new Vector3(CameraOfset, CameraHight, MaxCameraDistance) / len;
			GD.Print($"Col: {cameraColisionDetector.GetCollisionPoint()}");
		} else {
			camera.Position = new Vector3(CameraOfset, CameraHight, MaxCameraDistance);
		}
	}

	void StartClock() {
		if (!Started) scoreBoard.Continue();
		Started = true;
	}
}
