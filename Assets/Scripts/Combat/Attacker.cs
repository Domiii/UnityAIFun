using UnityEngine;
using System.Collections;

/// <summary>
/// Automatically attacks nearby enemies, using a Shooter component.
/// </summary>
[RequireComponent (typeof(Shooter))]
[RequireComponent(typeof(TargetFinder))]
public class Attacker : MonoBehaviour {
	public float attackRadius = 10.0f;
	public bool attackOnSight = true;

	Living currentTarget;
	Shooter shooter;
	TargetFinder targetFinder;

	void Awake () {
		shooter = GetComponent<Shooter> ();
		targetFinder = GetComponent<AttackTargetFinder> ();
	}

	void Update () {
		if (attackOnSight) {
			EnsureTarget ();
		}
		KeepAttackingCurrentTarget ();
	}

	void OnDeath (DamageInfo damageInfo) {
		enabled = false;
	}


	#region Public
	public Living CurrentTarget {
		get {
			return currentTarget;
		}
	}

	public bool CanAttackCurrentTarget {
		get { return IsCurrentValid && IsCurrentInRange; }
	}

	public bool IsCurrentValid {
		get {
			return currentTarget != null && targetFinder.IsValidTarget (currentTarget);
		}
	}

	public bool IsCurrentInRange {
		get {
			return currentTarget != null && IsInRange (currentTarget);
		}
	}

	public bool IsInRange (Living target) {
		return targetFinder.IsInRange (target, attackRadius);
	}

	public bool CanAttack (Living target) {
		return IsInRange (target) && targetFinder.IsValidTarget (target);
	}

	void AttackCurrentTargetUnchecked() {
		shooter.StartShootingAt (currentTarget.transform.position);
	}

	bool KeepAttackingCurrentTarget () {
		if (CanAttackCurrentTarget) {
			AttackCurrentTargetUnchecked ();
			return true;
		}
		StopAttack ();
		return false;
	}

	public bool StartAttack (Living target) {
		if (CanAttackCurrentTarget) {
			StopAttack ();
		}

		currentTarget = target;
		if (CanAttackCurrentTarget) {
			AttackCurrentTargetUnchecked ();
			return true;
		}
		return false;
	}

	public void StopAttack () {
		shooter.StopShooting ();
	}

	public bool EnsureTarget () {
		// #1 keep attacking previous target.
		// #2 if currently has no target: look for new target to attack
		if (!CanAttackCurrentTarget && !FindNewTarget ()) {
			// could not find a valid target -> Stop
			StopAttack ();
			return false;
		}
		return true;
	}

	public bool FindNewTarget () {
		// find new target
		var target = targetFinder.FindTarget (attackRadius);

		if (target != null) {
			StartAttack (target);
			return true;
		}
		return false;
	}
	#endregion
}