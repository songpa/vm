﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using System.Collections.Specialized;

namespace vm.Aspects.Wcf.Tests
{
    public class MockLoggingExceptionHandler : IExceptionHandler
    {
        protected virtual string Category => "Default Category";

        public MockLoggingExceptionHandler(NameValueCollection attributes) { }

        public Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            if (Logger.IsLoggingEnabled())
            {
                IDictionary<string, object> properties = new Dictionary<string, object>();
                properties.Add("HandlingInstance ID:", handlingInstanceId);
                Logger.Write(exception, this.Category, properties);
            }
            return exception;
        }
    }

    [ConfigurationElementType(typeof(CustomHandlerData))]
    public class MockUnhandledLoggingExceptionHandler : MockLoggingExceptionHandler
    {
        public MockUnhandledLoggingExceptionHandler(NameValueCollection attributes) : base(attributes) { }

        protected override string Category => "UnhandledLogs Category";
    }

    [ConfigurationElementType(typeof(CustomHandlerData))]
    public class MockHandledLoggingExceptionHandler : MockLoggingExceptionHandler
    {
        public MockHandledLoggingExceptionHandler(NameValueCollection attributes) : base(attributes) { }

        protected override string Category => "HandledLogs Category";
    }
}
