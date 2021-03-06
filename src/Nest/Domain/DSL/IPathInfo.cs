﻿using System.Security.Cryptography.X509Certificates;
using Elasticsearch.Net;

namespace Nest
{
	public interface IPathInfo<TParameters> 
		where TParameters : IRequestParameters, new()
	{
		ElasticsearchPathInfo<TParameters> ToPathInfo(IConnectionSettingsValues settings);
	}
}