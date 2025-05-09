namespace Unity.UOS.Common.UOSLauncher.Scripts.Auth
{
    public enum ErrorCode
    {
        NotInitialized,
        InvalidUosAppID,
        InvalidJwt,
        NeedLogin,
        RefreshFailed,
        GenerateTokenFailed,
        NonceTimestampExpired,
        InvalidOperation
    }

    public class AuthException : System.Exception
    {
        public ErrorCode ErrorCode { get; }

        public AuthException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}