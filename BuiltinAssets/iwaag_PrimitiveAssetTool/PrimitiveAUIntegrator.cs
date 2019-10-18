using AGAsset.StdUtil;
using AGDevUnity.StdUtil;
using AGDev;
using AGDevUnity;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AGAsset;

namespace AGDevUnity {
	public class PrimitiveAUIntegrator : MonoBAUIntegrator, AssetUnitIntegrator {
		public override AssetUnitIntegrator assetUnitInteg => this;
		public Material material;
		void AssetUnitIntegrator.IntegrateAssetUnit(AssetRequestUnit request, AssetUnitIntegrateListener listener) {
			if (!AssetUtils.IsRequestedType(request, "BAgent")) {
				return;
			}
			var support = listener.OnBeginIntegrate();
			var coll = new PrvtColl { parent = this, listener = listener, request = request, support = support };
			support.integrantGiver.Give(
				AssetUtils.ChangeAssetRequestType(request, "Primitive"), coll
			);
		}
		class PrvtColl : Taker<AssetUnitInterface>, Taker<Primitive>, PrimitiveVisitor {
			public PrimitiveAUIntegrator parent;
			public AssetUnitIntegrateSupport support;
			public AssetUnitIntegrateListener listener;
			public AssetRequestUnit request;
			public GameObject generatedPrimitiveRoot;
			public GameObject generatedPrimitive;
			public bool didGetInterface = false;
			public List<StdMonoBBAgentSpace> subSpaces = new List<StdMonoBBAgentSpace>();
			Primitive primitive;
			void Taker<Primitive>.Take(Primitive _primitive) {
				primitive = _primitive;
				didGetInterface = true;
				primitive.AcceptVisitor(this);
				if (generatedPrimitive != null) {
					generatedPrimitiveRoot = new GameObject();
					generatedPrimitive.transform.SetParent(generatedPrimitiveRoot.transform);
					generatedPrimitiveRoot.transform.SetParent(UnityAssetUtil.inSceneAssetWorkspace);
					var newBAgent = generatedPrimitiveRoot.AddComponent<StdBehaverAgent>();
					foreach (var subSpace in subSpaces) {
						newBAgent.subSpaces.Add(subSpace);
						subSpace.transform.SetParent(newBAgent.transform, true);
					}
					support.generatedAssetInterface.modifier.SetContent(
						new AssetContentSettingParam<StdBehaverAgent> { content = newBAgent, doOverwrite = true },
						new StubAssetInResultListener<StdBehaverAgent> { }
					);
					support.OnSucceed();
				}
				else
					support.OnFail();
			}

			void PrimitiveVisitor.IfCapsule(Capsule capsule) {
				var newAvatarGObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
				newAvatarGObj.transform.localPosition = capsule.transform.position;
				newAvatarGObj.transform.localRotation = Quaternion.Euler(capsule.transform.rotation);
				newAvatarGObj.GetComponent<Renderer>().sharedMaterial = parent.material;
				generatedPrimitive = newAvatarGObj;
			}
			void Taker<Primitive>.None() {
				support.OnFail();
			}
			void Taker<AssetUnitInterface>.Take(AssetUnitInterface auInterface) {
				auInterface.referer.PickContent("", this as Taker<Primitive>);
			}

			void Taker<AssetUnitInterface>.None() {
				support.OnFail();
			}
			void PrimitiveVisitor.HasSubPrimitives(IEnumerable<Primitive> subPrimitives) {
				
			}

			void PrimitiveVisitor.IfCube(Cube cube){
				var newAvatarGObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
				newAvatarGObj.transform.localPosition = cube.transform.position;
				newAvatarGObj.transform.localRotation = Quaternion.Euler(cube.transform.rotation);
				newAvatarGObj.transform.localScale = cube.size;
				newAvatarGObj.GetComponent<Renderer>().sharedMaterial = parent.material;
				generatedPrimitive = newAvatarGObj;
			}

			void PrimitiveVisitor.IfHasSubSapce(SpaceSetting subSpaceSetting) {
				var subSpaceGObj = new GameObject();
				var spaceComponent = subSpaceGObj.AddComponent<StdMonoBBAgentSpace>();
				spaceComponent._upperBoundary = subSpaceSetting.upperBoundary;
				spaceComponent._lowerBoundary = subSpaceSetting.lowerBoundary;
				spaceComponent.transform.localPosition = subSpaceSetting.transform.position;
				spaceComponent.transform.localRotation = Quaternion.Euler( subSpaceSetting.transform.rotation );
				spaceComponent.name = subSpaceSetting.name;
				subSpaces.Add(spaceComponent);
			}
		}
	}
	public interface Capsule {
		TransformInfo transform { get; }
		Color color { get; }
		float length { get; }
		float raduis { get; }
	}
	public interface Cube {
		TransformInfo transform { get; }
		Color color { get; }
		Vector3 size { get; }
	}
	public interface PrimitiveVisitor {
		void IfCapsule(Capsule capsule);
		void IfCube(Cube cube);
		void IfHasSubSapce(SpaceSetting subSpaces);
		void HasSubPrimitives(IEnumerable<Primitive> subPrimitives);
	}
	public interface Primitive {
		void AcceptVisitor(PrimitiveVisitor visitor);
	}
}
