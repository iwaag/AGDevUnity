using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGBLang;
using AGDev;
namespace AGDevUnity {
	public abstract class MonoBBahaverGiver: MonoBehaviour {
		public abstract ImmediateGiver<Behaver, GrammarBlock> behaverGiver { get; }
	}
}