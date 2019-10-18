
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDev;
using AGBLang;

namespace AGDevUnity {
	public abstract class MonoBCommonSenseGiver : MonoBehaviour {
		public abstract CommonSenseGiver commonSenseGiver { get; }
	}
}
