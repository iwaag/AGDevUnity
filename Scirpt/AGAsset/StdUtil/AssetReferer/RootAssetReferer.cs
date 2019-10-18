using AGAsset;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGAsset.StdUtil;
using AGDev.StdUtil;

namespace AGDevUnity.StdUtil {
	public class RootAssetReferer : MonoBAssetReferer {
		public ObservedProcessHelper helper = new ObservedProcessHelper();
		public bool doCacheAsset;
		public override AssetReferer assetReferer {
			get {
				if (_observedReferer == null) {
					var referers = new List<AssetReferer>();
					referers.Add(new RepositoryReferer{});
					referers.Add(new DirectLinkReferer { });
					//referers.Add(new StdArchiveReferer{});
					foreach (var referer in GetComponentsInChildren<MonoBAssetReferer>()) {
						if (referer != this)
							referers.Add(referer.assetReferer);
					}
					_observedReferer = new ObservedAssetReferer { clientReferer = new ClusterAssetReferer { referers = referers }, helper = helper };
				}
				return _observedReferer;
			}
		}
		public ObservedAssetReferer _observedReferer;
	}
}