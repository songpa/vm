﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace vm.Aspects.Diagnostics
{
    using System.Diagnostics.Contracts;
    using MetadataTypeAttribute = System.ComponentModel.DataAnnotations.MetadataTypeAttribute;

    /// <summary>
    /// Primary responsibility of the class is to retrieve the metadata (the buddy class) and DumpAttribute instance associated with a class to be
    /// dumped. For speed the class encapsulates a cache of type (usually class or struct) and the associated <see cref="ClassDumpData"/> which 
    /// contains the buddy and DumpAttribute instance. Allows to define externally metadata and a DumpAttribute on a class/struct level - useful for 
    /// FCL and 3rd party classes for which we do not have access to their source code.
    /// </summary>
    static class ClassMetadataResolver
    {
        // static cache of buddy class defined explicitly either in the initializer or by calling SetMetadataType
        static ReaderWriterLockSlim _typesDumpDataSync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        static Dictionary<Type, ClassDumpData> _typesDumpData = new Dictionary<Type, ClassDumpData>();

        /// <summary>
        /// Synchronizes the cache of dump metadata (buddy classes).
        /// </summary>
        static ReaderWriterLockSlim TypesDumpDataSync => _typesDumpDataSync;

        /// <summary>
        /// Gets or sets the cache of dump metadata (buddy class) defined explicitly either in the initializer above or by calling SetMetadataType.
        /// </summary>
        static Dictionary<Type, ClassDumpData> TypesDumpData => _typesDumpData;

        /// <summary>
        /// Adds buddy type and dump attribute for classes which we do not have access to, e.g. Exception.
        /// </summary>
        /// <param name="type">The type for which to set buddy type and dump attribute.</param>
        /// <param name="metadata">The metadata type (buddy class).</param>
        /// <param name="dumpAttribute">The dump attribute.</param>
        /// <param name="replace">
        /// If set to <see langword="false" /> and there is already dump metadata associated with the <paramref name="type"/>
        /// the method will throw exception of type <see cref="InvalidOperationException"/>;
        /// otherwise it will silently override the existing metadata with <paramref name="metadata"/> and <paramref name="dumpAttribute"/>.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <paramref name="replace"/> is <see langword="false"/> and there is already metadata associated with the <paramref name="type"/>.
        /// </exception>
        public static void SetClassDumpData(
            Type type,
            Type metadata = null,
            DumpAttribute dumpAttribute = null,
            bool replace = false)
        {
            Contract.Requires<ArgumentNullException>(type != null, nameof(type));

            if (metadata == null)
            {
                var attribute = type.GetCustomAttribute<MetadataTypeAttribute>();

                metadata = attribute != null
                                ? attribute.MetadataClassType
                                : type;
            }

            AddClassDumpData(type, metadata, dumpAttribute, replace);
        }

        /// <summary>
        /// Gets the dump attribute either from the type itself or if the class is applied <see cref="MetadataTypeAttribute"/> from the specified class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="DumpAttribute"/> reference or <c>null</c>.</returns>
        public static ClassDumpData GetClassDumpData(
            Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, nameof(type));

            // if the class is already in the cache return that
            var dumpData = TryGetClassDumpData(type);

            if (dumpData.HasValue)
                return dumpData.Value;

            // extract the dump data from the type
            dumpData = ExtractClassDumpData(type);

            try
            {
                AddClassDumpData(type, dumpData.Value, false);

                // return what we found
                return dumpData.Value;
            }
            catch (InvalidOperationException)
            {
                return TryGetClassDumpData(type).Value;
            }
        }

        static ClassDumpData ExtractClassDumpData(Type type)
        {
            Contract.Requires(type != null);

            // see if the class has a buddy:
            var attribute = type.GetCustomAttribute<MetadataTypeAttribute>();

            // if there is no buddy, we assume that the class provides the metadata itself
            Type metadata = attribute != null
                                ? attribute.MetadataClassType
                                : type;

            return new ClassDumpData(metadata);
        }

        static ClassDumpData? TryGetClassDumpData(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, nameof(type));

            ClassDumpData dumpData;

            try
            {
                TypesDumpDataSync.EnterReadLock();
                if (TypesDumpData.TryGetValue(type, out dumpData))
                    return dumpData;
            }
            finally
            {
                TypesDumpDataSync.ExitReadLock();
            }

            return null;
        }

        static void AddClassDumpData(Type type, Type buddy, DumpAttribute dumpAttribute, bool replace)
        {
            Contract.Requires<ArgumentNullException>(type != null, nameof(type));

            AddClassDumpData(type, new ClassDumpData(buddy, dumpAttribute), replace);
        }

        static void AddClassDumpData(Type type, ClassDumpData classDumpData, bool replace)
        {
            Contract.Requires<ArgumentNullException>(type != null, nameof(type));

            TypesDumpDataSync.EnterWriteLock();
            try
            {
                ClassDumpData dumpData;

                if (!replace  &&  TypesDumpData.TryGetValue(type, out dumpData))
                {
                    if (dumpData == classDumpData)
                        return;

                    throw new InvalidOperationException(
                                $"The type {type.FullName} is already associated with metadata type {TypesDumpData[type].Metadata.FullName} and a DumpAttribute instance.");
                }

                TypesDumpData[type] = classDumpData;
            }
            finally
            {
                TypesDumpDataSync.ExitWriteLock();
            }
        }
    }
}
