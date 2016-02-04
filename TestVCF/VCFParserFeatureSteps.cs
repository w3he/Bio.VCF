using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Bio.VCF.Test
{
    [Binding]
    public class VCFParserFeatureSteps
    {
        private string VcfFile { get; set; }
        private VCFParser Vcp { get; set; }

        [Given(@"A VCF file, (.*)")]
        public void GivenAVCFFile(string file)
        {
            VcfFile = Path.Combine("testData\\", file);
            //ScenarioContext.Current.Pending();

            Assert.IsTrue(File.Exists(VcfFile));

            
        }
        
        [When(@"I open the file")]
        public void WhenIOpenTheFile()
        {
            //ScenarioContext.Current.Pending();

             Vcp = new VCFParser(VcfFile);
            Assert.IsNotNull(Vcp);
        }
        
        [Then(@"count the number of variants in the file")]
        public void ThenCountTheNumberOfVariantsInTheFile()
        {
            //ScenarioContext.Current.Pending();
            foreach (var variant in Vcp)
            {
                var gc = variant.Genotypes.Count;
                var ac = variant.Alleles.Count;

                //if (variant.Chr != "X" && variant.Chr != "Y") continue;

                if (variant.Type== VariantContext.VariantType.INDEL) continue;

                Console.WriteLine(
                    "Variant: {{ type: '{0}', genotypes: {1}, alleles: {2}, chromosome: '{3}', start: {4}, end: {5}, \n\t    attributes: {6}}}",
                    variant.Type, gc, ac, variant.Chr, variant.Start, variant.End, JsonConvert.SerializeObject(variant.Attributes));
                Console.WriteLine();

                if (variant.HasAttribute("FUSIONENTREZ"))
                {
                    Console.WriteLine("FUSION ENTREZ: '{0}'", variant.Attributes["FUSIONENTREZ"]);
                }

                if (variant.HasAttribute("FUSIONSYMBOL"))
                {
                    Console.WriteLine("FUSION SYMBOL: '{0}'", variant.Attributes["FUSIONSYMBOL"]);
                }

                Console.WriteLine("Genotypes:");
                foreach (var genotype in variant.Genotypes)
                {
                    Console.WriteLine("Sample: '{0}', GenotypeString: '{1}', extended: {2}, AD: {3}, GQ: {4}, DP: {5}", genotype.SampleName, genotype.GenotypeString,
                        JsonConvert.SerializeObject(genotype.ExtendedAttributes), 
                        genotype.HasAD? JsonConvert.SerializeObject(genotype.AD) : "[]",
                        genotype.GQ, 
                        genotype.DP);
                    //Console.WriteLine();
                }

                Console.WriteLine("Alleles:");
                foreach (var allele in variant.Alleles)
                {
                    Console.WriteLine("allele: {0}", JsonConvert.SerializeObject(allele));
                    //Console.WriteLine();
                    
                }

//                foreach (string s in variant.SampleNames)
//                {
//                    var sampleContext = variant.SubContextFromSample(s);
//                    //var samples = sampleContext.SampleNames;
//                    Console.WriteLine("{0}: {1}", s, JsonConvert.SerializeObject(sampleContext));
//                    Console.WriteLine();
//                }


                Console.WriteLine();
                //Console.WriteLine("In JSON: {0}", JsonConvert.SerializeObject(variant));
            }
            var i = Vcp.Select(x => x.NoCallCount).Count();

            Console.WriteLine("No calls: {0}", i);

        }
    }
}
