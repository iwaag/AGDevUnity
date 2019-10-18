using UnityEngine;
using System.Collections;

namespace AGDevUnity {
	public abstract class MonoBObjectContainer<ObjectType> : MonoBehaviour {
		public abstract ObjectType content{ get; }
	}
}