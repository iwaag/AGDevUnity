using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBConfigurable : MonoBehaviour {
		public abstract Configurable configurable { get; }
	}
}