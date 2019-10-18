using System.Collections;
using System.Collections.Generic;
using AGAsset;
using AGDev;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	[System.Serializable]
	public class NameAndInputField {
		public string name;
		public UnityEngine.UI.InputField inputField;
	}
	public class PleyerEWCsAUInterface : MonoBAUInterface, AssetUnitInterface, AssetModifyInterface, AssetReferInterface {
		public List<NameAndInputField> nameAndUIs;
		public AssetUnitInfo auInfo;
		public List<System.Action> tasks = new List<System.Action>();
		public override AssetUnitInterface auInterface => this;
		AssetReferInterface AssetUnitInterface.referer => this;
		AssetModifyInterface AssetUnitInterface.modifier => this;
		AssetUnitInfo AssetUnitInterface.baseAssetInfo => auInfo;
		void AssetReferInterface.PickContent<ContentType>(string path, Taker<ContentType> collector) {
			if (typeof(ContentType) == typeof(byte[])) {
				var field = nameAndUIs.Find((item) => string.Compare( item.name, path, true) == 0);
				if (field != null) {
					if (!string.IsNullOrEmpty(field.inputField.text)) {
						collector.Take((ContentType)(object)System.Text.Encoding.UTF8.GetBytes(field.inputField.text));
                        return;
					} /*else {
						tasks.Add(()=> {
							collector.Take((ContentType)(object)System.Text.Encoding.UTF8.GetBytes(field.inputField.text));
						});
						return;
					}*/
				}
			}
			collector.None();
		}
		public void DecideText() {
			foreach (var task in tasks) {
				task();
			}
			tasks.Clear();
		}
		void AssetModifyInterface.SetContent<ContentType>(AssetContentSettingParam<ContentType> setParam, AssetInResultListener<ContentType> listener) {
			listener.OnFail();
		}
	}
}
