using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBConfigurableField : MonoBehaviour {
		public abstract Taker<Configurable> configurableField { get; }
	}
}