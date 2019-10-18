using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AGDev.Native;
using AGBLang;

namespace AGDevUnity {
	public abstract class MonoBBehaviorAnalyzer : MonoBehaviour {
		public abstract BehaviorAnalyzer bAnlys { get; }
	}
}
