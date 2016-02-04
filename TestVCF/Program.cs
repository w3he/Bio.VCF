using System;
using System.Linq;

namespace Bio.VCF.Test
{
	class Program
	{

		static void Main (string[] args)
		{
			var fname = "testData/NA12878.knowledgebase.snapshot.20131119.b37.vcf.gz";
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch ();
			sw.Start ();
			VCFParser vcp = new VCFParser (fname);
			var j = vcp.First ();
			var ii = j.Genotypes.Count;
			var i = vcp.Select (x => x.NoCallCount).Count ();
			sw.Stop ();

            Console.WriteLine("Count: {0}/{1}", ii,i);

			Console.WriteLine ("Elapsed: {0}", sw.Elapsed);

		    var anyKey = Console.ReadKey();
		}
	}
}
