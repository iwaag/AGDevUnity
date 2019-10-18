using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
using AGDev.StdUtil;

namespace AGDevUnity.StdUtil {
	public class ObservedMonoBLProcessor : MonoBSyntacticProcessor {
		public MonoBSyntacticProcessor clientProcessor;
		public ObservedProcessHelper observeHelper = new ObservedProcessHelper();
		public override NaturalLanguageProcessor LProcessor => _LProcessor ?? (_LProcessor = new ObservedSyntacticProcessor { clientProcessor = clientProcessor.LProcessor, helper = observeHelper });
		NaturalLanguageProcessor _LProcessor;
	}
}