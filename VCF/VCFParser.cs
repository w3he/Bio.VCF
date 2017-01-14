using System.IO;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;

namespace Bio.VCF
{
	
    /// <summary>
    /// A Class to parse VCF Files
    /// </summary>
	public class VCFParser :IDisposable, IEnumerable<VariantContext>, IEnumerator<VariantContext>
	{
		private readonly VCFCodec vcfCodec = new VCFCodec();
		private readonly StreamReader reader;
        private string line = null;
        private VariantContext pCurrent;
        public FileInfo VcfFile { get; set; }
        // TODO: Add a c'tor that reads intervals.
        public VCFParser(FileInfo vcfFile)
		{
            VcfFile = vcfFile;
            if (vcfFile.Extension == ".gz")
            {
                FileStream fs = vcfFile.OpenRead();
                GZipStream gz = new GZipStream(fs, CompressionMode.Decompress);
                reader = new StreamReader(gz);
            }
            else
            {
                reader = new StreamReader(vcfFile.OpenRead(),System.Text.Encoding.ASCII,true,4000000);
            }
			VCFHeader header = vcfCodec.readHeader(reader);
			if (header == null)
			{
				throw new ApplicationException("The file " + vcfFile.FullName + " did not have a VCF header");
			}
			Header = header;
		}
        
        public VCFParser(string fileName) :this(new FileInfo(fileName))
        {}

		public void Close()
		{
            reader.Close();
		}
		public VCFHeader Header
		{
			get; private set;
		}
        #region IDisposable Members
        // Track whether Dispose has been called. 
        private bool disposed = false;

        // Implement IDisposable. 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                disposed = true;

            }
        }

        #endregion

        #region IEnumerable<VariantContext> Members

        public IEnumerator<VariantContext> GetEnumerator()
        {
            while (MoveNext())
            {
                yield return Current;
            }
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEnumerator<VariantContext> Members

        public VariantContext Current
        {
            get 
            {
                return pCurrent;
            }
        }

        #endregion

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get { return pCurrent; }
        }

	    public bool MoveNext()
	    {
	        line = reader.ReadLine();
	        if (line == null)
	            return false;

	        pCurrent = vcfCodec.decode(line);
	        line = null;
	        return true;
	    }

	    public void Reset()
        {
            throw new NotImplementedException();
        }
        #endregion
    }

}