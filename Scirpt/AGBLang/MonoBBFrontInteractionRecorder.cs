using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBBFrontInteractionRecorder : MonoBehaviour {
		public abstract BFrontInteractionRecorder recorder { get; }
	}
}
