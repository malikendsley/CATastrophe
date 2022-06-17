using UnityEngine;
using UnityEngine.AI;
[RequireComponent (typeof (NavMeshAgent))]
[RequireComponent (typeof (Animator))]
public class LocomotionSimpleAgent : MonoBehaviour {
    private Animator _anim;
    private NavMeshAgent _agent;
    private Vector2 _smoothDeltaPosition = Vector2.zero;
    private Vector2 _velocity = Vector2.zero;

	void Start () {
		_anim = GetComponent<Animator> ();
		_agent = GetComponent<NavMeshAgent> ();
		_agent.updatePosition = false;
	}
	
	void Update () {
		var worldDeltaPosition = _agent.nextPosition - transform.position;

		// Map 'worldDeltaPosition' to local space
		var dx = Vector3.Dot (transform.right, worldDeltaPosition);
		var dy = Vector3.Dot (transform.forward, worldDeltaPosition);
		var deltaPosition = new Vector2 (dx, dy);

		// Low-pass filter the deltaMove
		var smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
		_smoothDeltaPosition = Vector2.Lerp (_smoothDeltaPosition, deltaPosition, smooth);

		// Update velocity if delta time is safe
		if (Time.deltaTime > 1e-5f)
			_velocity = _smoothDeltaPosition / Time.deltaTime;

		var shouldMove = _velocity.magnitude > 0.5f && _agent.remainingDistance > _agent.radius;

		// Update animation parameters
		_anim.SetBool("move", shouldMove);
		_anim.SetFloat ("velx", _velocity.x);
		_anim.SetFloat ("vely", _velocity.y);

		var lookAt = GetComponent<LookAt> ();
		if (lookAt)
			lookAt.lookAtTargetPosition = _agent.steeringTarget + transform.forward;

		// Pull character towards agent
		if (worldDeltaPosition.magnitude > _agent.radius)
			transform.position = _agent.nextPosition - 0.9f*worldDeltaPosition;

//		// Pull agent towards character
		//if (worldDeltaPosition.magnitude > agent.radius)
		//	agent.nextPosition = transform.position + 0.9f*worldDeltaPosition;
	}

	void OnAnimatorMove () {
		// Update position to agent position
//		transform.position = agent.nextPosition;

		// Update position based on animation movement using navigation surface height
		var position = _anim.rootPosition;
		position.y = _agent.nextPosition.y;
		transform.position = position;
	}
}
