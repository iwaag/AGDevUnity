using UnityEngine;
using System.Collections;
using AGBLang;

namespace AGDevUnity {
	public abstract class MonoBBehaver : MonoBehaviour {
		public abstract Behaver behaver { get; }
	}
}
