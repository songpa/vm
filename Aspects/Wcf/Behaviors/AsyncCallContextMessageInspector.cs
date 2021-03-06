﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace vm.Aspects.Wcf.Behaviors
{
    class AsyncCallContextMessageInspector : IDispatchMessageInspector
    {
        public const string CallContextSlotName = nameof(AsyncCallContext);

        /// <summary>
        /// Called after an inbound message has been received but before the message is dispatched to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>The object used to correlate state. This object is passed back in the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.BeforeSendReply(System.ServiceModel.Channels.Message@,System.Object)" /> method.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The AsyncCallContext will be disposed in BeforeSendReply")]
        public object AfterReceiveRequest(
            ref Message request,
            IClientChannel channel,
            InstanceContext instanceContext)
        {
            ClearCallContext();

            CallContext.LogicalSetData(CallContextSlotName, new AsyncCallContext());

            return null;
        }

        /// <summary>
        /// Called after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">The reply message. This value is null if the operation is one way.</param>
        /// <param name="correlationState">The correlation object returned from the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(System.ServiceModel.Channels.Message@,System.ServiceModel.IClientChannel,System.ServiceModel.InstanceContext)" /> method.</param>
        public void BeforeSendReply(
            ref Message reply,
            object correlationState)
        {
            ClearCallContext();
        }

        static bool ClearCallContext()
        {
            var context = CallContext.LogicalGetData(CallContextSlotName) as AsyncCallContext;

            if (context == null)
                return false;

            context.Clear();
            CallContext.FreeNamedDataSlot(CallContextSlotName);
            return true;
        }
    }
}
