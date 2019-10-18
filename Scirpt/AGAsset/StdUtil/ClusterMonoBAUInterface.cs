using System.Collections;
using System.Collections.Generic;
using AGAsset;
using AGAsset.StdUtil;
using AGDev;
using AGDev.StdUtil;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class ClusterMonoBAUInterface : MonoBAUInterface {
		public List<MonoBAUInterface> interfaces = new List<MonoBAUInterface>();
		public AssetUnitInfo auInfo;
		public override AssetUnitInterface auInterface => clusterAUItfc;
		public ClusterAUInterface clusterAUItfc {
			get {
				if(_clusterAUItfc == null) {
					_clusterAUItfc = new ClusterAUInterface {
						auInfo = auInfo,
						cluster = new ConvertingEnumarable<AssetUnitInterface, MonoBAUInterface> {
							convertFunc = (elem) => elem.auInterface, sourceEnumerable = interfaces
						}
					};
				}
				return _clusterAUItfc;
			}
		}
		ClusterAUInterface _clusterAUItfc;
		

	}
}