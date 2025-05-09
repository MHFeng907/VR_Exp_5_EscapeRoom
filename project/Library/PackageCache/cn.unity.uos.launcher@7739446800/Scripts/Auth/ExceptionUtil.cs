using System;
using Newtonsoft.Json;
using Unity.UOS.Models;
using UnityEngine.Networking;

namespace Unity.UOS.Common.UOSLauncher.Scripts.Auth
{
    public class ExceptionUtil
    {
        private const string NonceTokenExpired = "nonce timestamp expired";

        public static readonly Action<UnityWebRequest> ExceptionHandler = request =>
        {
            var error = JsonConvert.DeserializeObject<UosServiceError>(request.downloadHandler.text);
            if (NonceTokenExpired.Equals(error.message))
            {
                throw new AuthException(ErrorCode.NonceTimestampExpired, error.message);
            }
            throw new AuthException(ErrorCode.GenerateTokenFailed, error.message);
        };
    }
}