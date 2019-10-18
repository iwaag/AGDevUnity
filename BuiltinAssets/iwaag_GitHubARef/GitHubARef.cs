using AGDev;
using AGAsset;
using AGDevUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using AGDevUnity.StdUtil;

namespace iwaag {
	public class GitHubARef : MonoBAssetReferer, AssetReferer {
		public string addressBase = "https://raw.githubusercontent.com/";
		public override AssetReferer assetReferer => this;
		void AssetReferer.ReferAsset(AssetUnitInfo assetUnitInfo, AssetReferenceListener listener) {
			string[] assetRef = assetUnitInfo.reference.Split(':');
			if (System.Collections.CaseInsensitiveComparer.Equals(assetRef[0], "github")) {
				listener.OnBeginRefering();
				//whole repository
				if (assetRef.Length == 2) {
					listener.OnBasicIOObtained(new GitHubAssetBasicIO{ parent = this, repositoryAddress = assetRef[1], auInfo = assetUnitInfo });
					listener.OnFinish();
				}
				if (assetRef.Length == 3) {
					var www = new UnityWebRequest(addressBase + assetRef[1] + assetRef[2]);
					NetworkUtil.ProcessWebRequest(www, (givenWWW) => {
						if( string.IsNullOrEmpty( givenWWW.error ) )
							listener.OnRawAssetContentObtained(givenWWW.downloadHandler.data, assetRef[2]);
						listener.OnFinish();
					});
				}
			}
		}
	}
	public class GitHubAssetBasicIO : AssetUnitBasicIO, AssetUnitBasicIn, AssetUnitBasicOut {
		public GitHubARef parent;
		public AssetUnitInfo auInfo;
		public string repositoryAddress;
		AssetUnitBasicIn AssetUnitBasicIO.assetIn => this;
		AssetUnitBasicOut AssetUnitBasicIO.assetOut => this;
		AssetUnitInfo AssetUnitBasicIO.baseAssetUnitInfo => auInfo;
		void AssetUnitBasicOut.PickAssetAtPath<ContentType>(string path, AssetBasicTaker<ContentType> collector) {
			string url = parent.addressBase + repositoryAddress + "/" + path;
			UnityAssetUtil.PickFromDownloadLink(url, collector);
		}
		void AssetUnitBasicIn.SetContent<ContentType>(BasicAssetInParam<ContentType> settingParam, AssetInResultListener<ContentType> listener) {
			listener.OnFail();
		}
	}
}
