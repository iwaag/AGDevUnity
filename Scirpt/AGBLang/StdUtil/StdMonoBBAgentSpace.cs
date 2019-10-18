using AGDev;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public interface TransformInfo {
		Vector3 position { get; }
		Vector3 rotation { get; }
	}
	[System.Serializable]
	public class StdPrimitiveTransform : TransformInfo {
		public Vector3 position;
		public Vector3 rotation;
		Vector3 TransformInfo.position { get { return position; } }
		Vector3 TransformInfo.rotation { get { return rotation; } }
	}
	[System.Serializable]
	public class SpaceSetting {
		public string name;
		public Vector3 lowerBoundary;
		public Vector3 upperBoundary;
		public StdPrimitiveTransform transform;
	}
	public class StdMonoBBAgentSpace : MonoBBAgentSpace, BAgentSpace {
		public Vector3 _lowerBoundary;
		public Vector3 _upperBoundary;
		public override BAgentSpace space => this;
		public Vector3 defaultItemSize = new Vector3(0.3f, 0.3f, 0.3f);
		Vector3 BAgentSpace.lowerBoundary { get { return _lowerBoundary; } }
		Vector3 BAgentSpace.upperBoundary { get { return _upperBoundary; } }
		Transform BAgentSpace.origin { get { return transform; } }
		void BAgentSpace.AcceptBehaverAgent(BehaverAgent bAgent) {
			bAgent.rootBObj.transform.SetParent(transform, false);
			//var pos = BehaviorUtilities.LocalPosition(spaceInfo, behavior);
			//bAgent.rootBObj.transform.SetParent(origin, false);
			//bAgent.transform.localPosition = pos;
			//bAgent.rootBObj.GetComponent<MonoBBehaverAgent>().behaverAgent.TryFittingSpace(defaultItemSize);
		}

		private void OnDrawGizmos() {
			Gizmos.color = new Color(1, 0, 0, 0.2f);
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube((_lowerBoundary + _upperBoundary) * 0.5f, (_upperBoundary - _lowerBoundary + new Vector3(0.05f, 0.05f, 0.05f)));
		}

		void BAgentSpace.Clear() {
			foreach (Transform agent in transform) {
				var bAgent = agent.GetComponent<MonoBBehaverAgent>();
				if (bAgent != null)
					bAgent.behaverAgent.DispatchFromSpace();
			}
		}
	}
}