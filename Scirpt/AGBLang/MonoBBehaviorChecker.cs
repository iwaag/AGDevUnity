using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AGDevUnity {
	public abstract class MonoBUnityBehaviorChecker : MonoBehaviour {
		public abstract UnityBehaviorChecker behaviorChecker { get; }
	}
}

