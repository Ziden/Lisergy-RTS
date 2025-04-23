/*
 * Copyright 2015 Tomi Valkeinen
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using System.Reflection;

namespace NetSerializer;

public sealed class TypeData
{
	public TypeData(Type type, uint typeID, ITypeSerializer typeSerializer)
	{
		Type = type;
		TypeID = typeID;
		TypeSerializer = typeSerializer;
	}

	public Type Type { get; }
	public uint TypeID { get; private set; }

	public ITypeSerializer TypeSerializer { get; }

	public MethodInfo WriterMethodInfo;
	public MethodInfo ReaderMethodInfo;

	internal SerializeDelegate<object> WriterTrampolineDelegate;
	internal Delegate WriterDirectDelegate;

	internal DeserializeDelegate<object> ReaderTrampolineDelegate;
	internal Delegate ReaderDirectDelegate;

	public bool WriterNeedsInstance
	{
		get
		{
#if GENERATE_DEBUGGING_ASSEMBLY
                if (this.WriterMethodInfo is MethodBuilder)
                    return this.WriterNeedsInstanceDebug;
#endif
			return WriterMethodInfo.GetParameters().Length == 3;
		}
	}

	public bool ReaderNeedsInstance
	{
		get
		{
#if GENERATE_DEBUGGING_ASSEMBLY
                if (this.ReaderMethodInfo is MethodBuilder)
                    return this.ReaderNeedsInstanceDebug;
#endif
			return ReaderMethodInfo.GetParameters().Length == 3;
		}
	}

#if GENERATE_DEBUGGING_ASSEMBLY
        // MethodBuilder doesn't support GetParameters(), so we need to track this separately
        public bool WriterNeedsInstanceDebug;
        public bool ReaderNeedsInstanceDebug;
#endif

	public bool CanCallDirect
	{
		get
		{
			// We can call the (De)serializer method directly for:
			// - Value types
			// - Array types
			// - Sealed types with static (De)serializer method, as the method will handle null
			// Other types go through the ObjectSerializer

			var type = Type;

			if (type.IsValueType || type.IsArray)
				return true;

			if (type.IsSealed && TypeSerializer is IStaticTypeSerializer)
				return true;

			return false;
		}
	}
}