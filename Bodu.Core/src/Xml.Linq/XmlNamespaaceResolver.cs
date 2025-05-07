﻿// ---------------------------------------------------------------------------------------------------------------
// <copyright file="XmlNamespaaceResolver.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Bodu.Xml.Linq
{
	/// <summary>
	/// Helper class for resolving and accessing XML elements within a specific namespace.
	/// </summary>
	public sealed class XmlNamespaceResolver
	{
		private readonly XNamespace xNamespace;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlNamespaceResolver" /> class with the specified root element.
		/// </summary>
		/// <param name="root">The root element from which to extract the default namespace.</param>
		/// <exception cref="InvalidOperationException">Thrown if the root element is null or has no namespace.</exception>
		public XmlNamespaceResolver(XElement root)
		{
			ThrowHelper.ThrowIfNull(root);

			xNamespace = root.Name.Namespace ?? throw new InvalidOperationException("Missing XML xNamespace on root element.");
		}

		/// <summary>
		/// Gets the fully qualified XName for the given local name in the current namespace.
		/// </summary>
		/// <param name="localName">The local (unqualified) element or attribute name.</param>
		/// <returns>The namespaced XName.</returns>
		public XName Name(string localName) =>
			xNamespace + localName;

		/// <summary>
		/// Safely gets a child element with the specified local name in the current namespace.
		/// </summary>
		/// <param name="parent">The parent element.</param>
		/// <param name="localName">The local name of the child element.</param>
		/// <returns>The matching child XElement, or null if not found.</returns>
		public XElement? Element(XElement parent, string localName) =>
			parent.Element(Name(localName));

		/// <summary>
		/// Safely gets all child elements with the specified local name in the current namespace.
		/// </summary>
		/// <param name="parent">The parent element.</param>
		/// <param name="localName">The local name of the child elements.</param>
		/// <returns>An enumerable of matching XElement objects.</returns>
		public IEnumerable<XElement> Elements(XElement parent, string localName) =>
			parent.Elements(Name(localName));
	}
}