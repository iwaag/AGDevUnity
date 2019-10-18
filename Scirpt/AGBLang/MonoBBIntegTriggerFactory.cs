using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDev;
using AGBLang;

namespace AGDevUnity.StdUtil {
	public abstract class MonoBBIntegTriggerFactory : MonoBehaviour {
		public abstract IntegrableBTrigger NewIntegrableBTrigger();
	}
	public interface IntegrableBTrigger : BehaviorTrigger {
		void AddSubTrigger(NameAndBTrigger namedTrigger);
	}
}