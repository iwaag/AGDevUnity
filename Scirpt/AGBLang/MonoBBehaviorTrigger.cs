using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBBehaviorTrigger : MonoBehaviour {
		public abstract ImmediateGiver<BehaviorTrigger, string> bTriggerGiver { get; }
	}
}