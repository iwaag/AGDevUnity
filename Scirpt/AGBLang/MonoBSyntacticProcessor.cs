using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using AGBLang;
namespace AGDevUnity {
	public abstract class MonoBSyntacticProcessor : MonoBehaviour {
		public abstract NaturalLanguageProcessor LProcessor { get; }
	}
}
