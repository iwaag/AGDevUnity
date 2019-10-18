using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDev;
using AGBLang;
namespace AGDevUnity{
	public class StdMonoBLProcessor : MonoBSyntacticProcessor, NaturalLanguageProcessor {
		public override NaturalLanguageProcessor LProcessor => this;
		void NaturalLanguageProcessor.PerformSyntacticProcess(string behaviorExpression, Taker<GrammarBlock> listener) {
			throw new System.NotImplementedException();
		}
	}
}