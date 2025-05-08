using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace OnceNull.FSM
{
	public class StateMachineUGUIDebugState : MonoBehaviour
	{
		private static readonly int IsActive = Animator.StringToHash("IsActive");

		[Title("References")]
		public StateMachine StateMachine;
		public State State;
		public StateActionTimeInState TimeInState;

		[Title("UI Components")]
		public Animator Animator;
		public GameObject ProgressBar;
		public Image ImageProgressBar;

		private bool _isStateActive;
		
		private void OnEnable()
		{
			StateMachine.OnStateMachineEvent += StateMachineEventHandler;
		}
		
		private void OnDisable()
		{
			StateMachine.OnStateMachineEvent -= StateMachineEventHandler;
		}

		private void Start()
		{
			TimeInState = State.GetAction<StateActionTimeInState>();
			
			if (TimeInState == null) 
				ProgressBar.SetActive(false);
			
			if (StateMachine.CurrentState == State)
				OnStateEntered();
		}

		private void Update()
		{
			if (StateMachine.CurrentState != State) return;
			StateUpdate();
		}

		private void StateMachineEventHandler(StateMachineEvent e)
		{
			if (e.State != State) return;
			switch (e.Type)
			{
				case StateMachineEventType.EnteredState:
					OnStateEntered();
					break;
				case StateMachineEventType.ExitedState:
					OnStateExited();
					break;
			}
		}

		private void OnStateEntered()
		{
			Animator?.SetBool(IsActive, true);
			_isStateActive = true;
		}

		private void OnStateExited()
		{
			Animator?.SetBool(IsActive, false);
			_isStateActive = false;
			ImageProgressBar.fillAmount = 0;
		}

		private void StateUpdate()
		{
			if (TimeInState == null) return;
			ImageProgressBar.fillAmount = TimeInState.TimeInState / TimeInState.EndTime;
		}
	}
}