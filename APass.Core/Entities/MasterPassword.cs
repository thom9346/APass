using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APass.Core.Entities
{
    public class MasterPassword
    {
        public int Id { get; set; }
        public byte[] EncryptedDEK { get; set; }
        public byte[] Salt { get; set; }
    }
}
