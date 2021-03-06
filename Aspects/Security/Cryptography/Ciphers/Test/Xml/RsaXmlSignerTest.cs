﻿using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vm.Aspects.Security.Cryptography.Ciphers.Tests;

namespace vm.Aspects.Security.Cryptography.Ciphers.Xml.Tests
{
    [TestClass]
    [DeploymentItem("..\\..\\Xml\\TestOrder.xml")]
    public class RsaXmlSignerTest : GenericXmlSignerTest<RsaXmlSigner>
    {
        public override IXmlSigner GetSigner(SignatureLocation signatureLocation = SignatureLocation.Enveloped) => new RsaXmlSigner(CertificateFactory.GetSigningCertificate()) { SignatureLocation = signatureLocation };

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RsaXmlSignerNullCertTest()
        {
            using (var target = new RsaXmlSigner(null))
                Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void RsaXmlSignerSha512Test()
        {
            using (var target = new RsaXmlSigner(new X509Certificate2(), Algorithms.Hash.Sha512))
                Assert.Fail();
        }

        #region IsDisposed tests
        [TestMethod]
        public void IsDisposedTest()
        {
            var target = (RsaXmlSigner)GetSigner();

            Assert.IsNotNull(target);

            using (target as IDisposable)
                Assert.IsFalse(target.IsDisposed);
            Assert.IsTrue(target.IsDisposed);

            // should do nothing:
            target.Dispose();
        }

        [TestMethod]
        public void FinalizerTest()
        {
            var target = new WeakReference<RsaXmlSigner>((RsaXmlSigner)GetSigner());

            GC.Collect();

            RsaXmlSigner collected;

            Assert.IsFalse(target.TryGetTarget(out collected));
        }
        #endregion
    }
}
