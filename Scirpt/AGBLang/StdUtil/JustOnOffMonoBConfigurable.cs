using AGDev;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AGDevUnity.StdUtil {
	public class JustOnOffMonoBConfigurable : MonoBConfigurable, Configurable, Taker<bool>{
		public override Configurable configurable => this;
		void Taker<bool>.Take(bool item) {
			gameObject.SetActive(item);
		}
		void Taker<bool>.None() {}
		void Configurable.ProvideConfiguration(ConfigurationListener listener) {
			listener.OnEnableConfigure(name, this, gameObject.activeSelf);
		}

		
	}
}
