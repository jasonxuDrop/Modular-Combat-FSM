using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OnceNull
{
	public class CharacterMovementController3D : CharacterAbility
	{

		[ReadOnly]
		public Transform BaseTransform;
		
		[Title("Settings")]
		public CharacterMovementControllerParameter OriginalParameters;

		public CharacterMovementControllerParameter? OverrideParameters;
	
		
		
		[Title("Info")]
		
		[ReadOnly]
		[Tooltip("This is the speed calculated from the position delta on late update. Meant to be read only.")]
		public Vector3 Speed;

		/// <summary>
		/// Modify this to change the speed of the character.
		/// This resets to zero every frame on late update.
		/// Add or Set, depending on how many sources are controlling this at the same time.
		/// This combines with SaturatedVelocity to create the final velocity.
		/// </summary>
		[ReadOnly]
		public Vector3 NormalVelocity;

		/// <summary>
		/// Use this if you want to ignore the MaxSpeed set by the character controller. 
		/// = NormalVelocity * MaxSpeed
		/// Also resets to zero every frame on late update. Add or Set if needed.
		/// This combines with NormalVelocity to create the final velocity.
		/// </summary>
		[ReadOnly]
		public Vector3 SaturatedVelocity;

		/// <summary>
		/// This is the velocity that is applied to the character.
		/// </summary>
		[ShowInInspector, ReadOnly]
		public Vector3 FinalVelocity => NormalVelocity * Parameters.Speed + SaturatedVelocity;

		private CharacterMovementControllerParameter Parameters => OverrideParameters ?? OriginalParameters;
		
		


		public Vector3 CurrentHorizontalVelocity { get; protected set; }


		private Character3D _character3D;
		private CharacterController _characterController;

		
		
		protected virtual void Start()
		{
			_character3D = Character as Character3D;
			if (_character3D != null) 
				BaseTransform = _character3D.transform;
			_characterController = _character3D?.CharacterController;
		}

		private void Update()
		{
			// move towards target velocity
			Vector3 targetHorizontalVelocity = FinalVelocity;
			targetHorizontalVelocity.y = 0;
			bool isAccelerating = targetHorizontalVelocity.magnitude >= CurrentHorizontalVelocity.magnitude;

			CurrentHorizontalVelocity = Vector3.MoveTowards(
				CurrentHorizontalVelocity,
				targetHorizontalVelocity,
				(isAccelerating ? Parameters.Acceleration : Parameters.Deceleration) * Time.deltaTime
			);

			// TODO: y velocity, gravity, jumping, etc.

			// movements applied on late update so other scripts can modify the velocity before applying
		}

		private void LateUpdate()
		{
			// apply velocity to character controller
			Vector3 move = new Vector3(CurrentHorizontalVelocity.x, 0, CurrentHorizontalVelocity.z) * Time.deltaTime;
			_characterController.Move(move);
			
			Reset();
		}


		
		public void SetRotation(Quaternion targetRotation)
		{
			// Set the character's rotation to the target rotation
			_character3D.transform.rotation = Quaternion.RotateTowards(
				_character3D.transform.rotation,
				targetRotation,
				Parameters.MaxRotationSpeed * Time.deltaTime
			);
		}
		
		
		
		private void Reset()
		{
			// reset normal velocity and saturated velocity
			NormalVelocity = Vector3.zero;
			SaturatedVelocity = Vector3.zero;
		}
	}

	
	[System.Serializable]
	public struct CharacterMovementControllerParameter
	{
		[Tooltip("This multiplier is applied to the normal velocity to form the final velocity.")]
		public float Speed;
		public float Acceleration; // max velocity change per second
		public float Deceleration; // max velocity change per second
		[SuffixLabel("degrees/second")]
		public float MaxRotationSpeed; // max rotation speed in degrees per second
	}
}