using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AGDevUnity {
	public abstract class MonoBUnityBehaviorSetter : MonoBehaviour {
		public abstract UnityBehaviorSetter behaviorSetter { get; }
	}
}

