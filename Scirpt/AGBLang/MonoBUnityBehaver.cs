using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity{
	public abstract class MonoBUnityBehaver : MonoBehaviour {
		public abstract UnityBehaver behaver { get; }
	}
}