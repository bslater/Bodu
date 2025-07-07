﻿// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DaysOfWeekSet.Serializable.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Bodu
{
	public partial struct DaysOfWeekSet
		: System.Runtime.Serialization.ISerializable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DaysOfWeekSet" /> structure from serialized data.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo" /> containing the serialized data.</param>
		/// <param name="context">The destination for this serialization (not used).</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="info" /> is <see langword="null" />.</exception>
		/// <exception cref="SerializationException">Thrown if the stored value is invalid.</exception>
		private DaysOfWeekSet(SerializationInfo info, StreamingContext context)
		{
			ThrowHelper.ThrowIfNull(info);

			int data = info.GetByte(nameof(selectedDays));
			if (data < MinValue || data > MaxValue)
				throw new SerializationException(string.Format(ResourceStrings.SerializationException_InvalidState, nameof(DaysOfWeekSet)));

			selectedDays = (byte)data;
		}

		/// <summary>
		/// Populates a <see cref="SerializationInfo" /> with the data needed to serialize the current <see cref="DaysOfWeekSet" />.
		/// </summary>
		/// <param name="info">The <see cref="SerializationInfo" /> to populate with data.</param>
		/// <param name="context">The destination for this serialization (not used).</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="info" /> is <see langword="null" />.</exception>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			ThrowHelper.ThrowIfNull(info);

			info.AddValue(nameof(selectedDays), selectedDays);
		}
	}
}