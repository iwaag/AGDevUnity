using System;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace AGAsset
{
	
#if false
	public class WebClientDataCopier : DataCopier {
		void DataCopier.CopyData(string copySource, DataCopyListener listener) {
			ServicePointManager.ServerCertificateValidationCallback = (o, certificate, chain, errors) => true;
			WebClient client = new WebClient();
			var data = client.DownloadData(copySource);
			listener.OnSuccess(data);
		}
	};

#endif
}