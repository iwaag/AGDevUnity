using AGDev.StdUtil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
namespace AGDevUnity.StdUtil {
	public class StdSpaceDistributer : MonoBSpaceDistributer, SpaceDistributer {
		public MonoBBAgentSpace defaultSpace;
		public override SpaceDistributer spaceDistributer => this;

		IEnumerable<BAgentSpace> SpaceDistributer.spaces {
			get {
				return new ConvertingEnumarable<BAgentSpace, MonoBBAgentSpace> { sourceEnumerable = GetComponentsInChildren<MonoBBAgentSpace>(), convertFunc = (elem) => elem.space };
			}
		}

		void SpaceDistributer.DistributeSpaceToBehave(GameObject behaver, BehaviorExpression behavior, SpaceInfoListener listener) {
			foreach( var space in GetComponentsInChildren<StdBehaviorSpaceReservedForBehaver>()) {
				if (space.behaviorNames.Contains(behaver.name)) {
					listener.OnSpaceUpdated(space.GetComponent<MonoBBAgentSpace>().space);
					return;
				}
				listener.OnSpaceUpdated(defaultSpace.space);
			}
			
		}
	}
}