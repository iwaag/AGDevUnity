using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGDevUnity;
using AGDev.StdUtil;
using AGDev;
using AGAsset.StdUtil;
using AGAsset;

namespace AGDevUnity {
	public class RootAUICustomizer : MonoBAUICustomizer, ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> {
		public override ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> assetCustomizer => this;
		public MonoBAssetMediator assetMed;
		ClusterImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> rootGiver;
		AssetUnitInterface ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO>.PickBestElement(AssetUnitBasicIO key) {
			if (rootGiver == null) {
				var customizers = new List<ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO>>();
				customizers.Add(new AssetToolAUICustomizer{});
				customizers.Add(new PrefabAUICustomizer<CustomizableUnityBehaver> {
					mainAssetName = "Main.prefab" , supportedAssetTypes = {"CustomBehaver" } });
				customizers.Add(new PrefabAUICustomizer<MonoBUnityBehaver> {
					mainAssetName = "Main.prefab" , supportedAssetTypes = {"Behaver" } });
				customizers.Add( new PrefabAUICustomizer<MonoBBehaverEquipper> {
					mainAssetName = "Main.prefab" , supportedAssetTypes = {"BEquipper"} });
				customizers.Add( new PrefabAUICustomizer<MonoBBehaverAgent> {
					mainAssetName = "Main.prefab" , supportedAssetTypes = {"BAgent"} });
				customizers.Add( new PrefabAUICustomizer<MonoBBehaverAvatar> {
					mainAssetName = "Main.prefab" , supportedAssetTypes = {"BAvatar"} });
				customizers.Add( new StdAUICustomizer<AudioClip, AudioClip> {
					mainAssetName = "Main.mp3" , supportedAssetTypes = {"Music"} });
				customizers.Add(new StdAUICustomizer<byte[], byte[]> {
					mainAssetName = "Main.txt",
					supportedAssetTypes = { "EWCs" }
				});
				foreach (var customizer in assetMed.assetMed.GetImplementedAssets<MonoBAUICustomizer>()) {
					customizers.Add(customizer.assetCustomizer);
				}
				rootGiver = new ClusterImmediateGiver<AssetUnitInterface, AssetUnitBasicIO> { pickers = customizers };
			}
			return (rootGiver as ImmediateGiver<AssetUnitInterface, AssetUnitBasicIO>).PickBestElement(key);
		}
	}
}
