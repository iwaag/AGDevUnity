using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	public abstract class MonoBPlayerInterface : MonoBehaviour {
		public abstract UnityPlayerInterface playerInterface { get; }
	}
}