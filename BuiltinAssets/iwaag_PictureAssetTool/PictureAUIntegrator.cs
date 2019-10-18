using AGAsset;
using AGAsset.StdUtil;
using AGDev;
using AGDevUnity;
using AGDevUnity.StdUtil;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class PictureAUIntegrator : MonoBAUIntegrator, AssetUnitIntegrator {
		public override AssetUnitIntegrator assetUnitInteg => this;
		void AssetUnitIntegrator.IntegrateAssetUnit(AssetRequestUnit request, AssetUnitIntegrateListener listener) {
			if (!AssetUtils.IsRequestedType(request, "BAgent")) {
				return;
			}
			var support = listener.OnBeginIntegrate();
			var coll = new PrvtColl { support = support, listener = listener, request = request };
			support.integrantGiver.Give(AssetUtils.ChangeAssetRequestType(request, "Picture"), coll);
		}
		class PrvtColl : Taker<AssetUnitInterface>, Taker<Texture2D> {
			public AssetUnitIntegrateSupport support;
			public AssetUnitIntegrateListener listener;
			public AssetRequestUnit request;
			void Taker<Texture2D>.Take(Texture2D texture) {
				var rootGObj = new GameObject();
				rootGObj.transform.SetParent(UnityAssetUtil.inSceneAssetWorkspace);
				rootGObj.name = request.attributes[0] + "BAgent";
				var bAgent = rootGObj.AddComponent<StdBehaverAgent>();
				var renderer = new GameObject().AddComponent<SpriteRenderer>();
				renderer.name = "Sprite";
				renderer.sprite = Sprite.Create(texture, new Rect(0,0, texture.width, texture.height), Vector2.one * 0.5f, texture.width);
				renderer.transform.SetParent(rootGObj.transform, false);
				renderer.transform.localRotation = Quaternion.Euler(0,180,0);
				support.generatedAssetInterface.modifier.SetContent(
					new AssetContentSettingParam<StdBehaverAgent> {
						content = bAgent,
						contentPath = "",
						doOverwrite = true
					},
					new StubAssetInResultListener<StdBehaverAgent>()
				);
				support.OnSucceed();
			}

			void Taker<AssetUnitInterface>.Take(AssetUnitInterface auInterface) {
				auInterface.referer.PickContent("", this as Taker<Texture2D>);
			}

			void Taker<AssetUnitInterface>.None() {
				support.OnFail();
			}

			void Taker<Texture2D>.None() {
				support.OnFail();
			}
		}
	}
}
