using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDev;
using AGDevUnity;
using AGDev.StdUtil;
namespace AGDevUnity.StdUtil {
	public class StdMonoBPlayerInterface : MonoBPlayerInterface, UnityPlayerInterface {
		public override UnityPlayerInterface playerInterface => this;
		public List<ChoiceField> choiceFields = new List<ChoiceField>();
		[System.Serializable]
		public class ChoiceField : GObjPickupInterface {
			public StdMonoBPlayerInterface parent;
			public bool didFinish = false;
			public GObjPickupListener listener;
			public List<MonoBBehaverAgent> candidates = new List<MonoBBehaverAgent>();
			void GObjPickupInterface.AddCandidate(MonoBBehaverAgent candidateGObj) {
				AGDev.StdUtil.Utilities.AddIfNotDuplicated(candidates, candidateGObj);
			}
			public void Update() {
				if (!didFinish) {
					if (Input.GetMouseButtonDown(0)) {
						Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
						bool didHit = Physics.Raycast(ray, out var hitInfo);
						if (didHit) {
							if (candidates.Contains(hitInfo.transform.gameObject.GetComponent<MonoBBehaverAgent>())) {
								listener.OnCandidatePicked(hitInfo.transform.gameObject.GetComponent<MonoBBehaverAgent>(), () => {
									didFinish = true;
								});
							}
						}
					}
				}
			}
		}
		void Update() {
			var count = choiceFields.Count;
			choiceFields.RemoveAll((elem) => elem.didFinish);
			foreach (var choiceField in choiceFields) {
				choiceField.Update();
				if (count != choiceFields.Count) {
					break;
				}
			}
		}
		GObjPickupInterface UnityPlayerInterface.NewGObjChoiceField(GObjPickupListener listener) {
			var field = new ChoiceField { listener = listener, parent = this };
			choiceFields.Add(field);
			return field;
		}
	}
}