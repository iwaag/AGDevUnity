using AGAsset;
using AGAsset.StdUtil;
using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity {
	[System.Serializable]
	public class AddressAndIO {
		public string address;
		public InSceneAssetUnitBasicIO io;
	}
	public class InSceneAssetBasicIO : MonoBAssetBasicIO, AssetBasicIO, ImmediateGiver<AssetUnitBasicIO, AssetUnitInfo> {
		public override AssetBasicIO assetIO => this;
		ImmediateGiver<AssetUnitBasicIO, AssetUnitInfo> AssetBasicIO.assetGiver => this;
		string AssetBasicIO.LocalizedAssetRef(AssetUnitInfo sourceAsset) {
			return "InScene";
		}
		public List<AddressAndIO> nameAndIO;
		static string Address(AssetUnitInfo key) {
			return key.reference + "_" + AssetUtils.StdAssetIDName(key);
		}
		AssetUnitBasicIO ImmediateGiver<AssetUnitBasicIO, AssetUnitInfo>.PickBestElement(AssetUnitInfo key) {
			var found = nameAndIO.Find((elem) => elem.address == Address(key));
			if (found != null) {
				return found.io;
			} else if ((this as AssetBasicIO).LocalizedAssetRef(key) == key.reference) {
				var newOne = new AddressAndIO { address = key.reference + "_" + AssetUtils.StdAssetIDName(key), io = new InSceneAssetUnitBasicIO { _baseAssetUnitInfo = key } };
				nameAndIO.Add(newOne);
				return newOne.io;
			}
			return null;
		}
	}
	public class NamedElement<type> {
		type element;
		string name;
	}
	[System.Serializable]
	public class NamedObject : NamedElement<object> { }
	[System.Serializable]
	public class InSceneAssetUnitBasicIO : AssetUnitBasicIO, AssetUnitBasicIn, AssetUnitBasicOut {
		AssetUnitBasicIn AssetUnitBasicIO.assetIn => this;
		AssetUnitBasicOut AssetUnitBasicIO.assetOut => this;
		public AssetUnitInfo _baseAssetUnitInfo;
		AssetUnitInfo AssetUnitBasicIO.baseAssetUnitInfo => _baseAssetUnitInfo;

		public static string CPToPath(AssetContentAndPath cp) { return cp.path; }
		public List<AssetContentAndPath> namedObjects = new List<AssetContentAndPath>();
		[System.Serializable]
		public class AssetContentAndPath {
			public string path;
			public object content;
		}
		void AssetUnitBasicIn.SetContent<AssetContentType>(BasicAssetInParam<AssetContentType> settingParam, AssetInResultListener<AssetContentType> listener) {
			try {
				var nObj = namedObjects.Find((elem) => settingParam.contentPath == elem.path);
				if (nObj != null) {
					if (settingParam.doOverwrite) {
						nObj.path = settingParam.contentPath;
						listener.OnOverwrite();
						return;
					}
					//stub
					//string new Path = listener.OnRequestPathChange();
				}
				namedObjects.Add(new AssetContentAndPath { content = settingParam.content, path = settingParam.contentPath });
				listener.OnSuccess();
			} catch (System.Exception e) {
				Debug.LogError(e);
				listener.OnFail();
			}
		}

		void AssetUnitBasicOut.PickAssetAtPath<ElementType>(string path, AssetBasicTaker<ElementType> collector) {
			try {
				var nObj = namedObjects.Find((elem) => path == elem.path);
				if (nObj != null) {
					collector.CollectAsset((ElementType)nObj.content);
				}
				collector.OnFinish();
			} catch (System.Exception e) {
				Debug.LogError(e);
				collector.OnFinish();
			}
		}
	}
}