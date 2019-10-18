using AGDevUnity;
using AGBLang;
using AGBLang.StdUtil;

namespace iwaag {
	public class SpaceBehaver : MonoBUnityBehaver, UnityBehaver {
		public override UnityBehaver behaver => this;
		BehaverAvatar UnityBehaver.avatar => StubBehaverAvatar.instance;
		AttributeMatchResult AttributeMatcher.MatchAttribue(GrammarBlock attribute) {
			return GrammarBlockUtils.IsUnit(attribute, "space") ? AttributeMatchResult.POSITIVE : AttributeMatchResult.NEGATIVE;
		}
		UnityBehaviorTrigger UnityBehaviorSetter.ReadyBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport listener) {
			return null;
		}
		UnityBehaviorCheckTrigger UnityBehaviorChecker.ReadyCheckBehavior(BehaviorExpression bExpr, UnityBehaviorReadySupport listener) {
			return null;
		}
		ItemType UnityBehaver.GetModule<ItemType>() {
			return default(ItemType);
		}
	}
}