using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class EngineUIMonoBConfigurableField : MonoBConfigurableField, Taker<Configurable>, ConfigurationListener {
		public override Taker<Configurable> configurableField => this;
		public UnityEngine.UI.Toggle settingContentPrefab;
		public GameObject fieldRootGObj;
		void Taker<Configurable>.Take(Configurable item) {
			item.ProvideConfiguration(this);
		}
		void Taker<Configurable>.None() {}
		void ConfigurationListener.OnEnableConfigure<Type>(string name, Taker<Type> collector, Type initialValue) {
			if (typeof(Type) == typeof(bool)){
				bool _initialValue = (bool)(object)initialValue;
				var settingContent = GameObject.Instantiate(settingContentPrefab, fieldRootGObj.transform);
				settingContent.isOn = _initialValue;
				settingContent.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<bool>((toggleValue) => { collector.Take((Type)(object)toggleValue); }));
			}
		}

		void ConfigurationListener.OnDisableConfigure(string name) {
			//stub
		}

		
	}
}