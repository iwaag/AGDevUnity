using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using AGDevUnity;
using AGAsset;
using System.Collections;
using AGDev;
using AGAsset.StdUtil;
using AGDevUnity.StdUtil;

namespace AGDevUnity {
	[System.Serializable]
	public class StdTransformInfo : TransformInfo {
		public Vector3 position;
		public Vector3 rotation;
		Vector3 TransformInfo.position { get { return position; } }
		Vector3 TransformInfo.rotation { get { return rotation; } }
	}
	[System.Serializable]
	public class StdCapsule : Capsule {
		float Capsule.length { get { return length; } }
		public float length;
		float Capsule.raduis { get { return raduis; } }
		public float raduis;
		Color Capsule.color => color;
		public Color color;
		TransformInfo Capsule.transform { get { return transform; } }
		public StdTransformInfo transform;
	}
	[System.Serializable]
	public class StdCube : Cube
	{
		TransformInfo Cube.transform { get { return transform; } }
		public StdTransformInfo transform;
		Color Cube.color => color;
		public Color color;
		Vector3 Cube.size => size;
		public Vector3 size;
	}
	[System.Serializable]
	public class SerializedPrimitive : Primitive {
		public StdCapsule capsule;
		public StdCube cube;
		public SpaceSetting[] spaces;
		void Primitive.AcceptVisitor(PrimitiveVisitor visitor) {
			//if(capsule != null)
			//	visitor.IfCapsule(capsule);
			if(cube != null)
				visitor.IfCube(cube);
			if (spaces != null) {
				foreach (var space in spaces) {
					visitor.IfHasSubSapce(space);
				}
			}
		}
	}
	public class PrimitiveAUICustomizer : MonoBAUICustomizer {
		[Serializable]
		public class PrimitiveAssetGiver : JsonAUICustomizer<SerializedPrimitive, Primitive> { };
		public PrimitiveAssetGiver pickerInstance;
		public override ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> assetCustomizer { get { return pickerInstance; } }

	}
}