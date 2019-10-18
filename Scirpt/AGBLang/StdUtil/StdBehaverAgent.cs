using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
namespace AGDevUnity.StdUtil {
	public enum AspectType {
		FixedAspect,
		FreeAspect
	}
	public class StdBehaverAgent : MonoBBehaverAgent, BehaverAgent, BAgentSpace {
		public AspectType aspectType;
		public List<MonoBBAgentSpace> subSpaces = new List<MonoBBAgentSpace>();
		public override BehaverAgent behaverAgent => this;
		GameObject BehaverAgent.rootBObj => gameObject;
		BAgentSpace BehaverAgent.selfSpace => this;
		Transform BAgentSpace.origin { get { return transform; } }
		BAgentSpace BehaverAgent.rootSpace { get => _rootSpace; set => _rootSpace = value; }
		BAgentSpace _rootSpace;
		Vector3 BAgentSpace.lowerBoundary  { get{ return _lowerBoundary; } }
		public Vector3 _lowerBoundary = -Vector3.one;
		Vector3 BAgentSpace.upperBoundary { get { return _upperBoundary; } }
		public Vector3 _upperBoundary = Vector3.one;
		void BAgentSpace.AcceptBehaverAgent(BehaverAgent behaver) {}
		void BehaverAgent.TryFittingSpace(Vector3 size) {
			var baseSize = (this as BAgentSpace).upperBoundary - (this as BAgentSpace).lowerBoundary;
			var newScale = new Vector3(size.x / baseSize.x, size.y / baseSize.x, size.z / baseSize.x);
			if (aspectType == AspectType.FixedAspect) {
				if (newScale.x > newScale.y)
					transform.localScale = Vector3.one * newScale.y;
				else
					transform.localScale = Vector3.one * newScale.x;
			} else {
				newScale = size;
			}
		}

		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) { return AttributeMatchResult.NEUTRAL; }

		void BAgentSpace.Clear() { }

		void BehaverAgent.DispatchFromSpace() {
			gameObject.SetActive(false);
		}

		BAgentSpace BehaverAgent.GetSubSapce(GrammarUnit name) {
			foreach (var subSpace in subSpaces) {
				if(name.word.Equals(subSpace.name, System.StringComparison.CurrentCultureIgnoreCase) ) {
					return subSpace.space;
				}
			}
			return null;
		}
		private void OnDrawGizmos() {
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.DrawWireCube( (_lowerBoundary + _upperBoundary) * 0.5f, (_upperBoundary - _lowerBoundary));
		}
	}
}