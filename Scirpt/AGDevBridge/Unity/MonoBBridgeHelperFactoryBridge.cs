using UnityEngine;
using System.Collections;
using AGDev.Native;

namespace AGDevUnity {
#if false
    [RequireComponent(typeof(MonoBBaseFactory))]
	public class MonoBBridgeHelperFactoryBridge : MonoBehaviour {
		public BridgeHelperFactoryBridge BridgeHelperFactoryBridge {
			get {
				if (_BridgeHelperFactoryBridge == null) {
					Init();
				}
				return _BridgeHelperFactoryBridge;
			}
		}
		void Init() {
			_BridgeHelperFactoryBridge = GetComponent<MonoBBaseFactory>().helperFactory;
		}
		public BridgeHelperFactoryBridge _BridgeHelperFactoryBridge;
	}
#endif
}