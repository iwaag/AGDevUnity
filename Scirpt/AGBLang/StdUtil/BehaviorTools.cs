using AGBLang;
using UnityEngine;
using AGBLang.StdUtil;

namespace AGDevUnity {
	public class BehaviorUtilities {
		public static Vector3 LocalPosition(BAgentSpace space, BehaviorExpression bExpr) {
			if (bExpr.verb.modifier != null) {
				var at_where = GrammarBlockUtils.ShallowSeek(bExpr.verb.modifier, "at");
				if (at_where != null) {
					if (at_where.modifier.unit != null) {
						var positionDesc = at_where.modifier.unit.word.Split('-');
						if (positionDesc.Length == 2) {
							var posOffset = Vector3.zero;
							posOffset.z = 0.5f;
							if (positionDesc[0] == "lower")
								posOffset.y = 0.25f;
							else if (positionDesc[0] == "middle")
								posOffset.y = 0.5f;
							else if (positionDesc[0] == "upper")
								posOffset.y = 0.75f;
							if (positionDesc[1] == "left")
								posOffset.x = 0.25f;
							else if (positionDesc[1] == "center")
								posOffset.x = 0.5f;
							else if (positionDesc[1] == "right")
								posOffset.x = 0.75f;
							return space.lowerBoundary + Vector3.Scale(space.upperBoundary - space.lowerBoundary, posOffset);
						}
					}
				}
			}
			return Vector3.zero;
		}
	}
}