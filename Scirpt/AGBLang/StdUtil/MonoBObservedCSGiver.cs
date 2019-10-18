using AGBLang;
using AGDev.StdUtil;

namespace AGDevUnity.StdUtil {
	public class MonoBObservedCSGiver : MonoBCommonSenseGiver {
		public MonoBCommonSenseGiver clientGiver;
		public ObservedProcessHelper observeHelper = new ObservedProcessHelper();
		public override CommonSenseGiver commonSenseGiver => _csGiver ?? (_csGiver = new ObservedCSGiver { clientGiver = clientGiver.commonSenseGiver, helper = observeHelper });
		CommonSenseGiver _csGiver;
	}
}