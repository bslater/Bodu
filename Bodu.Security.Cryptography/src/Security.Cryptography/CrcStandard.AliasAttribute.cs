// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CrcStandards.CrcAliasAttribute.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	public sealed partial class CrcStandard
	{
		/// <summary>
		/// Marks a property of <see cref="CrcStandard" /> as an alias of another CRC Standard. This class cannot be inherited.
		/// </summary>
		[AttributeUsage(AttributeTargets.Property, Inherited = false)]
		internal sealed class CrcAliasAttribute
			: System.Attribute
		{ }
	}
}