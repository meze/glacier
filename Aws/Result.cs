using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aws
{
    public class Result
    {
        private string _archiveId;
        private string _checksum;

        public string ArchiveId
        {
            get
            {
                return _archiveId;
            }
        }
        public string Checksum
        {
            get
            {
                return _checksum;
            }
        }

        public Result(string archiveId, string checksum)
        {
            _checksum = checksum;
            _archiveId = archiveId;
        }
    }
}
