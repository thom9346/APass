

namespace APass.Core.Entities
{
    public class MasterPassword
    {
        public int Id { get; set; }
        public byte[] EncryptedDEK { get; set; }
        public byte[] Salt { get; set; }
    }
}
