using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlMigratorWinform
{
    public class VersionInfo
    {
        public Int64 Version { get; set; }
        public DateTime? AppliedOn { get; set; }
        public String Description { get; set; }
    }
}
