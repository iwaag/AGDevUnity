using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDev;
namespace AGDevUnity {
	public abstract class MonoBBAgentSpace : MonoBehaviour {
		public abstract BAgentSpace space { get; }
	}
}