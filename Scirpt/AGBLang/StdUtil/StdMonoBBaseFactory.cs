using UnityEngine;
using System.Collections;
using System;
using AGDev.Native;

namespace AGDevUnity.StdUtil {
#if false
    public class StdMonoBBaseFactory : MonoBBaseFactory {
		public MonoBNativeBehaver nativeBehaver;
		public override BridgeHelperFactoryBridge helperFactory {
			get {
				if (_helperFactory == null)
					_helperFactory = new BridgeHelperFactoryBridge();
				return _helperFactory;
			}
		}
		public override ConfigurableEWRIptrFactory iptrFactory {
			get {
				if (_iptrFactory == null) {
					_iptrFactory = new ConfigurableEWRIptrFactory(helperFactory);
					if (nativeBehaver != null) {
						_iptrFactory.nativeBehaverGetter = nativeBehaver.nativeBehaverGetter;
					}
				}
				return _iptrFactory;
			}
		}
		BridgeHelperFactoryBridge _helperFactory;
		ConfigurableEWRIptrFactory _iptrFactory;
	}
#endif
}