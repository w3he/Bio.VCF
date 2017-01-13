using System;
using System.Collections.Generic;
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

        public VCFParserFeatureSteps()
        {
            Console.WriteLine($"cwd: {Directory.GetCurrentDirectory()}");
            //reset the current working directory
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            Console.WriteLine($"cwd: {Directory.GetCurrentDirectory()}");
        }

        [Given(@"A VCF file, (.*)")]
        public void GivenAVCFFile(string file)
        {
            Console.WriteLine($"cwd: {Directory.GetCurrentDirectory()}");
            VcfFile = Path.Combine("testData", file);
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
            var fusions = new HashSet<string>();
            var cnvGains = new HashSet<string>();
            var cnvLosses = new HashSet<string>();

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

                if (variant.Type == VariantContext.VariantType.SYMBOLIC)
                {
                    if (variant.HasAttribute("CNVSYMBOL"))
                    {
                        if (variant.HasAttribute("SVTYPE"))
                        {
                            switch ((string) variant.Attributes["SVTYPE"])
                            {
                                case "DUP":
                                    cnvGains.Add(variant.Attributes["CNVSYMBOL"] as string);
                                    break;
                                case "DEL":
                                    cnvLosses.Add(variant.Attributes["CNVSYMBOL"] as string);
                                    break;
                                default:
                                    throw new ApplicationException("Unexpected SVTYPE: " + variant.Attributes["SVTYPE"]);
                                    break;
                            }
                        }

                        Console.WriteLine("{0}: {1} / {2}", variant.Type, variant.Attributes["CNVSYMBOL"],
                            variant.HasAttribute("SVTYPE") ? variant.Attributes["SVTYPE"] : variant.Attributes["VTYPE"]);
                    }

                    if (variant.HasAttribute("FUSIONSYMBOL"))
                    {
                        string fusion = (string) variant.Attributes["FUSIONSYMBOL"];
                        fusions.Add(fusion);
                        Console.WriteLine("FUSION SYMBOL: '{0}'", variant.Attributes["FUSIONSYMBOL"]);
                    }

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

            foreach (var pair in fusions)
            {
                Console.WriteLine("Gene fusions: {0}", pair);
            }

            foreach (var cnv in cnvGains)
            {
                Console.WriteLine("GAINS: {0}", cnv);
            }

            foreach (var cnv in cnvLosses)
            {
                Console.WriteLine("LOSSES: {0}", cnv);
            }

        }
    }
}
