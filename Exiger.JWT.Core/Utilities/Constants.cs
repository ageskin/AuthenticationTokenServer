using System.Security.Cryptography;

namespace Exiger.JWT.Core.Utilities
{
    public static class Constants
    {
        public const string EXIGER_DB_CONNECTION_STRING = "Exiger.JWT.Auth.ConnectionString";
        public const string EncryptionKey256Base64ConfigurationName = "256BitsEncryptionKeyAsBase64";
        public const int EncryptionKeySize = 256;
        public const int EncryptionBlockSize = 128;

        public const PaddingMode EncryptionPaddingMode = PaddingMode.ISO10126;
        public const CipherMode EncryptionCipherMode = CipherMode.CBC;
        public const string RegexEmailAddress = @"^([a-zA-Z0-9!#\$%&'\*\+\-\/=\?\^_`{\|}~])([a-zA-Z0-9!#\$%&'\*\+\-\/=\?\^_`{\|}~\.]*)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
        public const string RegexPassword = @"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*[^a-zA-Z]).*$";
        public const int PasswordMinimumRequiredLength = 8;

        public static readonly int COMMAND_TIMEOUT_DEFAULT = 180;
        public static readonly string COMMAND_TIMEOUT = "Exiger.JWT.Core.Utilities.Constants.CommandTimeoutSeconds";
        public const string LOGIN_ATTEMPTS = "Exiger.JWT.Core.Utilities.Constants.LoginAttempts";
        public const string TIMEOUT_PERIOD_IN_MINUTES = "Exiger.JWT.Core.Utilities.Constants.TimeoutPeriodInMinutes";

        public const string ISoftDeletableColumnIsDeleted = "IsDeleted";
    }
}
