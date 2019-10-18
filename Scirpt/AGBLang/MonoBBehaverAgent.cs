using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDevUnity;
namespace AGDevUnity{
	public abstract class MonoBBehaverAgent : MonoBehaviour {
		public abstract BehaverAgent behaverAgent { get; }
	}
}