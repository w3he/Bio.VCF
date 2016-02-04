using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Bio.VCF.Test
{
    [TestFixture]
    public class ParserTests
    {

        public ParserTests()
        {
            
        }

        [SetUp]
        public void SetupTests()
        {
        }

        [Test]
        public void ParseVcf([Values("testData/NA12878.knowledgebase.snapshot.20131119.b37.vcf.gz")] string fname)
        {
            Assert.IsTrue(File.Exists(fname));

            VCFParser vcp = new VCFParser(fname);
            var j = vcp.First();
            var ii = j.Genotypes;
            var i = vcp.Select(x => x.NoCallCount).Count();

            Console.WriteLine("Count: {0}", i);

        }
    }
}
