﻿using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace vm.Aspects.Wcf.Services
{
    /// <summary>
    /// The objects implementing this interface initialize service hosts with some particular actions that may not be possible to perform at CreateHost time.
    /// It allows for checking the initialization status and performing the actions at a later stage, e.g. first call, etc.
    /// </summary>
    [ContractClass(typeof(IInitializeServiceContract))]
    public interface IInitializeService
    {
        /// <summary>
        /// Gets a value indicating whether the service host has been fully initialized.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if this host is initialized; otherwise, <see langword="false"/>.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes the service. E.g. creates database, makes calls to dependant services, etc. The host may be used to obtain some additional information,
        /// e.g. the address of the service to be used in subscriptions.
        /// If initialization fails the method should throw an exception.
        /// </summary>
        /// <param name="host">The service host to be initialized.</param>
        /// <param name="messagingPattern">The messaging pattern.</param>
        /// <param name="maxWaitTime">The maximum time to wait for the initialization to finish.
        /// If 0 - the method will return immediately and if -1 - the method will wait indefinitely or until completion.</param>
        /// <returns><see langword="true" /> if the service was initialized successfully, <see langword="false" /> otherwise.</returns>
        bool Initialize(ServiceHost host, string messagingPattern, int maxWaitTime);

        /// <summary>
        /// Asynchronous (and await-able) version of <see cref="M:IInitializeService.Initialize" />
        /// </summary>
        /// <param name="host">The service host to be initialized.</param>
        /// <param name="messagingPattern">The messaging pattern.</param>
        /// <param name="maxWaitTime">The maximum time to wait for the initialization to finish.
        /// If 0 - the method will return immediately and if -1 - the method will wait indefinitely or until completion.</param>
        /// <returns>A <see cref="T:Task{bool}" /> object representing the service initialization.</returns>
        Task<bool> InitializeAsync(ServiceHost host, string messagingPattern, int maxWaitTime);
    }

    [ContractClassFor(typeof(IInitializeService))]
    abstract class IInitializeServiceContract : IInitializeService
    {
        #region IInitializeService Members

        public bool IsInitialized
        {
            get { throw new System.NotImplementedException(); }
        }

        public bool Initialize(
            ServiceHost host,
            string messagingPattern,
            int maxWaitTime)
        {
            Contract.Requires<ArgumentNullException>(host != null, nameof(host));
            Contract.Requires<ArgumentNullException>(messagingPattern!=null, nameof(messagingPattern));
            Contract.Requires<ArgumentException>(messagingPattern.Length > 0, "The argument "+nameof(messagingPattern)+" cannot be empty or consist of whitespace characters only.");
            Contract.Requires<ArgumentException>(messagingPattern.Any(c => !char.IsWhiteSpace(c)), "The argument "+nameof(messagingPattern)+" cannot be empty or consist of whitespace characters only.");
            Contract.Ensures(Contract.Result<bool>() && IsInitialized);

            throw new System.NotImplementedException();
        }

        public Task<bool> InitializeAsync(
            ServiceHost host,
            string messagingPattern,
            int maxWaitTime)
        {
            Contract.Requires<ArgumentNullException>(host != null, nameof(host));
            Contract.Requires<ArgumentNullException>(messagingPattern!=null, nameof(messagingPattern));
            Contract.Requires<ArgumentException>(messagingPattern.Length > 0, "The argument "+nameof(messagingPattern)+" cannot be empty or consist of whitespace characters only.");
            Contract.Requires<ArgumentException>(messagingPattern.Any(c => !char.IsWhiteSpace(c)), "The argument "+nameof(messagingPattern)+" cannot be empty or consist of whitespace characters only.");
            Contract.Ensures(Contract.Result<Task<bool>>() != null);
            Contract.Ensures(Contract.Result<Task<bool>>().Result && IsInitialized);

            throw new System.NotImplementedException();
        }

        #endregion
    }
}
