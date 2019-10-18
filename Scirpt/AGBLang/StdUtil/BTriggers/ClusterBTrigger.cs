using AGDev;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class ClusterBTrigger : MonoBBehaviorTrigger, ImmediateGiver<BehaviorTrigger, string> {
		public override ImmediateGiver<BehaviorTrigger, string> bTriggerGiver {
			get {
				return this;
			}
		}
		public List<MonoBBehaviorTrigger> triggers;
		BehaviorTrigger ImmediateGiver<BehaviorTrigger, string>.PickBestElement(string key) {
			foreach (var bTrigger in triggers) {
				var obtained = bTrigger.bTriggerGiver.PickBestElement(key);
				if (obtained != null)
					return obtained;
			}
			return null;
		}
	}
}