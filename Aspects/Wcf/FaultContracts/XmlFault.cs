﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using vm.Aspects.Wcf.FaultContracts.Metadata;

namespace vm.Aspects.Wcf.FaultContracts
{
    /// <summary>
    /// Class XmlFault. This class cannot be inherited. Corresponds to <see cref="T:XmlException"/>
    /// </summary>
    [DataContract(Namespace="urn:vm.Aspects.Wcf")]
    [MetadataType(typeof(XmlFaultMetadata))]
    public sealed class XmlFault : Fault
    {
        /// <summary>
        /// Gets or sets the XML document's source URI.
        /// </summary>
        [DataMember]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification="Matches the XmlException property.")]
        public string SourceUri { get; set; }

        /// <summary>
        /// Gets or sets the XML document's line number where the problem was encountered.
        /// </summary>
        [DataMember]
        public int LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the XML document's line position where the problem was encountered.
        /// </summary>
        [DataMember]
        public int LinePosition { get; set; }
    }
}