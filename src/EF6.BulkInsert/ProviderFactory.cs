﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using EF6.BulkInsert.Exceptions;
using EF6.BulkInsert.Providers;

namespace EF6.BulkInsert
{
    public class ProviderFactory
    {
        private static Dictionary<string, Func<IEfBulkInsertProvider>> _providers;

        private static readonly object ProviderInitializerLockObject = new object();

        /// <summary>
        /// Registered bulkinsert providers container
        /// </summary>
        private static Dictionary<string, Func<IEfBulkInsertProvider>> Providers
        {
            get
            {
                lock (ProviderInitializerLockObject)
                {
                    if (_providers == null)
                    {
                        _providers = new Dictionary<string, Func<IEfBulkInsertProvider>>();

                        // bundled providers
                        Register<DefaultBulkInsertProvider>();
                    }
                }

                return _providers;
            }
        }

        /// <summary>
        /// Register new bulkinsert provider.
        /// Rplaces existing if name matches.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        public static void Register<T>(string name)
            where T : IEfBulkInsertProvider, new()
        {
            Providers[name] = () => new T();
        }

        /// <summary>
        /// Register new bulk insert provider.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>()
            where T : IEfBulkInsertProvider, new()
        {
            var instance = new T();
            Providers[instance.ProviderIdentifier] = () => new T();
        }

        /// <summary>
        /// Get bulkinsert porvider by connection used in context
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEfBulkInsertProvider Get(DbContext context)
        {
            var connectionTypeName = context.Database.Connection.GetType().FullName;
            if (!Providers.ContainsKey(connectionTypeName))
            {
                // Try the default provider
                if (Providers.ContainsKey(string.Empty))
                {
                    connectionTypeName = string.Empty;
                }
                else
                {
                    throw new BulkInsertProviderNotFoundException(connectionTypeName);
                }
            }

            return Providers[connectionTypeName]().SetContext(context);
        }
    }
}
