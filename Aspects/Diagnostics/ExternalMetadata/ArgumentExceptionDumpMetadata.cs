﻿#pragma warning disable 1591

namespace vm.Aspects.Diagnostics.ExternalMetadata
{
    public abstract class ArgumentExceptionDumpMetadata
    {
        [Dump(false)]
        public object Message;

        [Dump(0)]
        public object ParamName;
    }
}
