
namespace APass.Core.Services
{
    public static class SecureSessionService
    {
        private static byte[] _dek = null;
        private static System.Timers.Timer _timer;

        public static void StoreDEK(byte[] dek)
        {
            ClearDEK(); //clear existing DEK and stop any running timer
            _dek = dek;

            if (_timer != null)
            {
                _timer.Stop();
            }
            else
            {
                _timer = new System.Timers.Timer(600000); //set timer to 10 minutes
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
                _timer.Stop(); //stop the timer when DEK is cleared
            }
        }
    }
}

