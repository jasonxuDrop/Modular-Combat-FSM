using System;
using OnceNull;
using OnceNull.FSM;
using Sirenix.OdinInspector;
using UnityEngine;

public class StateActionPlayerInputMove : CharacterStateAction
{
	public CharacterInputReaderZZZ InputReader;
	public CharacterMovementController3D CharacterController3D;
	public MainCameraController MainCameraController;
	
	[Title("Debug")]
	public bool DoDrawGizmos;

	private Vector3 _rotatedInput;


	protected override void Start()
	{
		base.Start();
		InputReader = Character.GetAbility<CharacterInputReaderZZZ>();
		if (InputReader == null) Debug.LogError("StateActionPlayerInputMove: InputReader is null.");
		CharacterController3D = Character.GetAbility<CharacterMovementController3D>();
		if (CharacterController3D == null) Debug.LogError("StateActionPlayerInputMove: CharacterController3D is null.");
		MainCameraController = MainCameraController.Current;
		if (MainCameraController == null) Debug.LogError("StateActionPlayerInputMove: MainCameraController is null.");
	}

	public override void OnUpdate()
	{
		base.OnUpdate();

		InputToCharacterControllerNormalVelocity();
	}


	/// <summary>
	/// This method is called every frame to update the character's velocity based on input.
	/// </summary>
	private void InputToCharacterControllerNormalVelocity()
	{
		// Get the camera's forward vector and project it onto the horizontal plane
		Vector3 cameraForward = MainCameraController.transform.forward;
		cameraForward.y = 0; // Remove vertical influence
		cameraForward.Normalize(); // Normalize to maintain direction
		// Get the right direction based on the camera's forward vector
		Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward); 

		// get move input (from input system)
		Vector2 moveInput = InputReader.Move;
		
		// calculate target velocity
		_rotatedInput = cameraRight * moveInput.x + cameraForward * moveInput.y;
		
		CharacterController3D.NormalVelocity += _rotatedInput;
	}

	
	
	private void OnDrawGizmos()
	{
		if (!DoDrawGizmos) return;
		
		if (!Application.isPlaying) return;

		if (MainCameraController == null) return;
		
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + _rotatedInput);
	}
}