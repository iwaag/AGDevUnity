using AGDevUnity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AGDev.Native.Unity {
#if false
    public class MonoBNativeLProcessor : MonoBSyntacticProcessor {
		public MonoBBaseFactory baseFactory;
		public List<TextAsset> formatConfigurations;
		public List<TextAsset> grammarConfigurations;
		public override SyntacticProcessor LProcessor {
			get {
#if !UNITY_WEBGL || UNITY_EDITOR
                if (_LProcessor == null) {
					var fAnlys = baseFactory.iptrFactory.NewConfigurableFAnalyser();
					var gAnlys = baseFactory.iptrFactory.NewConfigurableGAnalyser();
					foreach (var formatConfiguration in formatConfigurations)
						fAnlys.ConfigureFormat(formatConfiguration.bytes);
					foreach (var grammarConfiguration in grammarConfigurations)
						gAnlys.ConfigureGrammar(grammarConfiguration.bytes);
					_LProcessor = new NativeConfigurableLProcessor {
						fAnalyser = fAnlys,
						gAnalyser = gAnlys
					};
				}
				return _LProcessor;
#endif
			}
		}
		public NativeConfigurableLProcessor _LProcessor;
	}
#endif
}
