using UnityEngine;
using AGDev;
using AGDevUnity;
using AGBLang;
using AGBLang.StdUtil;
using AGDev.StdUtil;

namespace AGDevUnity.StdUtil {
	public class StdMonoBBEquipper : MonoBBehaverEquipper, BehaverEquipper {
		public override BehaverEquipper behaverEquipper => this;
		public MonoBUnityBehaver behaverPrefab;
		public MonoBUnityBehaviorSetter setterPrefab;
		public MonoBUnityBehaviorChecker checkerPrefab;
		void BehaverEquipper.EquipBehaverByBehavior(BehaviorExpression bExpr, BehaverEquipListener listener) {
			var support = new StdBehaviorReadySupport { assetMediator = new StubAssetMediator() };
			var unityBRSupport = new StdUnityBehaviorReadySupport { basic = support, giver = new StubImmediateGiver<UnityBehaver, GrammarBlock>()};
			if (behaverPrefab != null) {
				var trigger = behaverPrefab.behaver.ReadyBehavior(bExpr, unityBRSupport);
				var cTrigger = behaverPrefab.behaver.ReadyCheckBehavior(bExpr, unityBRSupport);
				if (trigger != null || cTrigger != null) {
					listener.OnEquipSubBehaver(behaverPrefab);
					return;
				}
			}
			if (setterPrefab != null) {
				var trigger = setterPrefab.behaviorSetter.ReadyBehavior(bExpr, unityBRSupport);
				if (trigger != null) {
					listener.OnEquipBehaviorSetter(setterPrefab);
					return;
				}
			}
			if (checkerPrefab != null) {
				var trigger = checkerPrefab.behaviorChecker.ReadyCheckBehavior(bExpr, unityBRSupport);
				if (trigger != null) {
					listener.OnEquipBehaviorChecker(checkerPrefab);
					return;
				}
			}
		}

		void BehaverEquipper.EquipBehaverByAttribute(GrammarBlock attribute, BehaverEquipListener listener) {
			if (behaverPrefab != null) {
				if (behaverPrefab.behaver.MatchAttribue(attribute) == AttributeMatchResult.POSITIVE) {
					listener.OnEquipSubBehaver(behaverPrefab);
				}
			}
		}
	}
}