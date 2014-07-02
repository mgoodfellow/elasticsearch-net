﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elasticsearch.Net;
using Nest.Resolvers;

namespace Nest
{
	public interface IDocumentOptionalPath<TParameters> : IRequest<TParameters>
		where TParameters : IRequestParameters, new()
	{
		IndexNameMarker Index { get; set; }
		TypeNameMarker Type { get; set; }
		string Id { get; set; }
		object IdFrom { get; set; }
	}

	internal static class DocumentOptionalPathRouteParameters
	{
		public static void SetRouteParameters<TParameters>(
			IDocumentOptionalPath<TParameters> path,
			IConnectionSettingsValues settings,
			ElasticsearchPathInfo<TParameters> pathInfo)
			where TParameters : IRequestParameters, new()
		{
			var inferrer = new ElasticInferrer(settings);

			pathInfo.Index = inferrer.IndexName(path.Index);
			pathInfo.Type = inferrer.TypeName(path.Type);
			pathInfo.Id = path.Id ?? inferrer.Id(path.IdFrom);
		}

		public static void SetRouteParameters<TParameters, T>(
			IDocumentOptionalPath<TParameters> path,
			IConnectionSettingsValues settings,
			ElasticsearchPathInfo<TParameters> pathInfo)
			where TParameters : IRequestParameters, new()
			where T : class
		{
			var inferrer = new ElasticInferrer(settings);

			var index = path.Index != null ? inferrer.IndexName(path.Index) : inferrer.IndexName<T>();
			var type = path.Type != null ? inferrer.TypeName(path.Type) : inferrer.TypeName<T>();
			var id = path.Id ?? inferrer.Id(path.IdFrom);

			pathInfo.Index = index;
			pathInfo.Type = type;
			pathInfo.Id = id;
		}

	}

	public abstract class DocumentOptionalPathBase<TParameters> : BasePathRequest<TParameters>, IDocumentOptionalPath<TParameters>
		where TParameters : FluentRequestParameters<TParameters>, new()
	{
		public IndexNameMarker Index { get; set; }
		public TypeNameMarker Type { get; set; }
		public string Id { get; set; }
		public object IdFrom { get; set; }

		protected override void SetRouteParameters(
			IConnectionSettingsValues settings, ElasticsearchPathInfo<TParameters> pathInfo)
		{
			DocumentOptionalPathRouteParameters.SetRouteParameters(this, settings, pathInfo);
		}
	}

	public abstract class DocumentOptionalPathBase<TParameters, T> : DocumentOptionalPathBase<TParameters>
		where TParameters : IRequestParameters, new()
		where T : class
	{
		protected override void SetRouteParameters(IConnectionSettingsValues settings, ElasticsearchPathInfo<TParameters> pathInfo)
		{
			DocumentOptionalPathRouteParameters.SetRouteParameters<TParameters, T>(this, settings, pathInfo);
		}
	}

	/// <summary>
	/// Provides a base for descriptors that need to describe a path in the form of 
	/// <pre>
	///	/{index}/{type}/{id}
	/// </pre>
	/// if one of the parameters is not explicitly specified this will fall back to the defaults for type 
	/// this version won't throw if any of the parts are inferred to be empty<para>T</para>
	/// </summary>
	public abstract class DocumentOptionalPathDescriptor<TDescriptor, TParameters, T>
		: BasePathDescriptor<TDescriptor, TParameters>, IDocumentOptionalPath<TParameters>
		where TDescriptor : DocumentOptionalPathDescriptor<TDescriptor, TParameters, T>, new()
		where TParameters : FluentRequestParameters<TParameters>, new()
		where T : class
	{

		private IDocumentOptionalPath<TParameters> Self { get { return this; } }
		IndexNameMarker IDocumentOptionalPath<TParameters>.Index { get; set; }
		TypeNameMarker IDocumentOptionalPath<TParameters>.Type { get; set; }
		string IDocumentOptionalPath<TParameters>.Id { get; set; }
		object IDocumentOptionalPath<TParameters>.IdFrom { get; set; }

		public TDescriptor Index(string index)
		{
			Self.Index = index;
			return (TDescriptor)this;
		}

		public TDescriptor Index(Type index)
		{
			Self.Index = index;
			return (TDescriptor)this;
		}

		public TDescriptor Index<TAlternative>() where TAlternative : class
		{
			Self.Index = typeof(TAlternative);
			return (TDescriptor)this;
		}

		public TDescriptor Type(string type)
		{
			Self.Type = type;
			return (TDescriptor)this;
		}
		public TDescriptor Type(Type type)
		{
			Self.Type = type;
			return (TDescriptor)this;
		}
		public TDescriptor Type<TAlternative>() where TAlternative : class
		{
			Self.Type = typeof(TAlternative);
			return (TDescriptor)this;
		}
		public TDescriptor Id(long id)
		{
			return this.Id(id.ToString());
		}
		public TDescriptor Id(string id)
		{
			Self.Id = id;
			return (TDescriptor)this;
		}
		public TDescriptor Object(T @object)
		{
			Self.IdFrom = @object;
			return (TDescriptor)this;
		}

		protected override void SetRouteParameters(
			IConnectionSettingsValues settings, ElasticsearchPathInfo<TParameters> pathInfo)
		{
			DocumentOptionalPathRouteParameters.SetRouteParameters(this, settings, pathInfo);
		}

		public ElasticsearchPathInfo<TParameters> ToPathInfo(IConnectionSettingsValues settings)
		{
			throw new NotImplementedException();
		}
	}
}
