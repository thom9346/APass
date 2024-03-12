
namespace APass.Core.Services
{
    public static class SecureSessionService
    {
        private static byte[] _dek = null;
        private static System.Timers.Timer _timer; // Timer to clear DEK

        public static void StoreDEK(byte[] dek)
        {
            ClearDEK(); // Clear existing DEK and stop any running timer
            _dek = dek;

            // Start or reset a timer for 10 minutes
            if (_timer != null)
            {
                _timer.Stop(); // Stop the existing timer if it's running
            }
            else
            {
                _timer = new System.Timers.Timer(60000); // Set timer to 10 minutes
                _timer.Elapsed += (sender, e) => ClearDEK();
                _timer.AutoReset = false;
            }
            _timer.Start();
        }

        public static byte[] GetDEK()
        {
            if (_dek == null) throw new InvalidOperationException("DEK not set or session expired.");
            return _dek;
        }

        public static void ClearDEK()
        {
            if (_dek != null)
            {
                Array.Clear(_dek, 0, _dek.Length);
                _dek = null;
            }
            if (_timer != null)
            {
                _timer.Stop(); // Stop the timer when DEK is cleared
            }
        }
    }
}

