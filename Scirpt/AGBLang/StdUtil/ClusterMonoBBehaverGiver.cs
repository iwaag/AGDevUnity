using System.Collections.Generic;
using UnityEngine;
using AGDevUnity;
using AGDev;
using AGBLang;

namespace AGDevUnity.StdUtil {
	public class ClusterMonoBBehaverGiver : MonoBBahaverGiver, ImmediateGiver<Behaver, GrammarBlock> {
		public override ImmediateGiver<Behaver, GrammarBlock> behaverGiver => this;
		public List<MonoBBehaver> regularBehavers;
		ImmediateGiver<Behaver, GrammarBlock> _behaverGiver;
		Behaver ImmediateGiver<Behaver, GrammarBlock>.PickBestElement(GrammarBlock key) {
			//try regular behavers
			foreach (var regularBehaver in regularBehavers) {
				foreach (var behaver in regularBehavers) {
					if (behaver.behaver.MatchAttribue(key) == AttributeMatchResult.POSITIVE) {
						return behaver.behaver;
					}
				}
			}
			//try sub pickers
			foreach (Transform child in transform) {
				var picker = child.GetComponent<MonoBBahaverGiver>();
				if(picker != null) {
					var behaver = picker.behaverGiver.PickBestElement(key);
					if (behaver != null)
						return behaver;
				}
			}
			return null;
		}
	}
}