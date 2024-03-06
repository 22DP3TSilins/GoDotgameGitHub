using Godot;
using System;


public partial class player : CharacterBody3D
{
	[Export] public float acceleration = 1.0f / 0.5f;
	[Export] public float stopping_acceleration = 10.0f;
	[Export] public float mouseSensitivity = 0.003f;
	Vector3 velocity = Vector3.Zero;
	Camera3D camera = null;
	Node3D cameraRotY = null;
	
	public override void _Ready() {
		Input.MouseMode = Input.MouseModeEnum.Captured;
		camera = GetNode<Camera3D>("CameraRig/RotY/Camera");
		cameraRotY = GetNode<Node3D>("CameraRig/RotY");
	}
	
	public const float Speed = 5.0f;
	public const float JumpVelocity = 5.2f;

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
		
		Vector2 input = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 target_velocity = new Vector3();
		Vector3 forward = camera.GlobalTransform.Basis.Z;
		Vector3 right = camera.GlobalTransform.Basis.X;

		target_velocity += right * input.X;
		target_velocity += forward * input.Y;
		target_velocity.Y = 0.0f;
		
		target_velocity = target_velocity.Normalized() * Speed;

		float target_acceleration = acceleration;
		if (target_velocity.LengthSquared() <= 0.1f) target_acceleration = stopping_acceleration;
		velocity = velocity.Lerp(target_velocity, target_acceleration * (float)delta);
		velocity.Y = VelocityY;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;
		
		Velocity = velocity;
		MoveAndSlide();
	}
	
	// move camera
	public override void _Input(InputEvent e) {
		if (e is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured) {
			var m_event = e as InputEventMouseMotion;
			Vector2 motion = -m_event.Relative * mouseSensitivity;
			this.RotateY(motion.X);
			cameraRotY.RotateX(motion.Y);
			
			
		}
		if (e.IsActionPressed("ui_cancel")) {
			Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
		}
	}
	
	
}
