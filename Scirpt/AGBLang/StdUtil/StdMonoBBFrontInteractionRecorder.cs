using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity.StdUtil {
#if false
	public class StdMonoBBFrontInteractionRecorder : MonoBBFrontInteractionRecorder, BFrontInteractionRecorder {
		public StdMonoBBahaverFrontFactory parentFactory;
		public override BFrontInteractionRecorder recorder => this;
		private void Awake() {
			parentFactory = GetComponent<StdMonoBBahaverFrontFactory>();
		}
		void BFrontInteractionRecorder.RecordBFrontInteraction(GameObject bFrontGObj) {
			var bFrontGObjRoot = Utilities.FindBFrontGObjRoot(bFrontGObj);
			foreach (var bFront in parentFactory.bFronts) {
				if ((bFront as AttributeMatcher).MatchAttribue(new StdGrammarUnit(bFrontGObjRoot.name)) == AttributeMatchResult.POSITIVE) {
					if (bFront.bFrontRoots.Contains(bFrontGObjRoot))
						bFront.recentTriggeredUnit = bFrontGObjRoot;
				}
			}
		}
		
	}
#endif
}