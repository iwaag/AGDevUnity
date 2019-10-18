using AGBLang;
using AGDevUnity;
using AGBLang.StdUtil;

namespace iwaag {
	public class GUIObjectBChecker : MonoBUnityBehaviorChecker, UnityBehaviorChecker {
		public override UnityBehaviorChecker behaviorChecker => this;

		UnityBehaviorCheckTrigger UnityBehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport listener) {
			UnityBehaviorCheckTrigger trigger = null;
			if (GrammarBlockUtils.IsUnit(bExpr.verb, "hit")) {
				var button = GrammarBlockUtils.ShallowSeek(bExpr.verb.modifier, "button");
				GrammarBlockUtils.ForEachUnits(
					button.modifier,
					(unit) => {
						trigger = FindObjectOfType<GUIBehaver>().NewHitButtonCheckTrigger(unit.word);
					}
				);
				return trigger;
			}
			return trigger;
		}
	}
}
