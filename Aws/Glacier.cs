using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Amazon;
using Amazon.Glacier;
using Amazon.Glacier.Model;
using Amazon.Glacier.Transfer;
using System.Collections.Specialized;
using System.Configuration;
using Amazon.Runtime;
using System.IO;
using System.Threading.Tasks;

namespace Aws
{
    public class UploadEventArgs : EventArgs
    {
        public int Progress { get; set; }
    }

    public class Glacier
    {
        private int _currentPercentage = -1;
        static string vaultName = "Photos";

        public event EventHandler<UploadEventArgs> ProgressChanged;

        public Result Upload(string fileName, string description)
        {
            using (var manager = new ArchiveTransferManager(Amazon.RegionEndpoint.USEast1))
            {
                try
                {
                    var options = new UploadOptions();
                    options.StreamTransferProgress += OnProgress;
                    var uploadResult = manager.Upload(vaultName, description, fileName, options);

                    return new Result(uploadResult.ArchiveId, uploadResult.Checksum);
                }
                catch (AmazonGlacierException e)
                {
                    throw new GlacierException(e.Message, e);
                }
                catch (AmazonServiceException e)
                {
                    throw new GlacierException(e.Message, e);
                }
            }
        }

        public async Task<IList<Vault>> GetVaults()
        {
            using(var client = new AmazonGlacierClient(Amazon.RegionEndpoint.USEast1))
            {
                var request = new ListVaultsRequest();
                var result = await client.ListVaultsAsync(request);
                var vaults = new List<Vault>();

                foreach (var vault in result.VaultList)
                {
                    vaults.Add(new Vault { Name = vault.VaultName });
                }


                return vaults;
            }
        }

        protected void OnProgress(object sender, StreamTransferProgressArgs args)
        {
            if (args.PercentDone != _currentPercentage)
            {
                _currentPercentage = args.PercentDone;
                OnProgressChanged(new UploadEventArgs { Progress = _currentPercentage });
            }
        }

        protected virtual void OnProgressChanged(UploadEventArgs e)
        {
            var tmp = ProgressChanged;
            if (tmp != null)
            {
                tmp(this, e);
            }
        }
    }
}
