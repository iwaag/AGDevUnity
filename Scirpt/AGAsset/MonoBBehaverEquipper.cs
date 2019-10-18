using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
namespace AGDevUnity {
	public abstract class MonoBBehaverEquipper : MonoBehaviour {
		public abstract BehaverEquipper behaverEquipper { get; }
	}
	public class JustCheckEquipListener : BehaverEquipListener {
		public bool canEquip = false;
		void BehaverEquipListener.OnEquipBehaviorSetter(MonoBUnityBehaviorSetter setter) { canEquip = true; }
		void BehaverEquipListener.OnEquipBehaviorChecker(MonoBUnityBehaviorChecker getter) { canEquip = true; }
		void BehaverEquipListener.OnEquipSubBehaver(MonoBUnityBehaver behaver) { canEquip = true; }
		void BehaverEquipListener.OnEquipAvatar(MonoBBehaverAvatar avatar) { canEquip = true; }
	}
	public class ClusterBehaverEquipper : BehaverEquipper {
		public IEnumerable<BehaverEquipper> equippers;
		void BehaverEquipper.EquipBehaverByAttribute(GrammarBlock attribute, BehaverEquipListener listener) {
			foreach (var equipper in equippers) {
				equipper.EquipBehaverByAttribute(attribute, listener);
			}
		}
		void BehaverEquipper.EquipBehaverByBehavior(BehaviorExpression bevrExpr, BehaverEquipListener listener) {
			foreach (var equipper in equippers) {
				equipper.EquipBehaverByBehavior(bevrExpr, listener);
			}
		}
	}
}

