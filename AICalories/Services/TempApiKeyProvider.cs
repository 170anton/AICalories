using System;
using System.Runtime.InteropServices;
using System.Security;

namespace AICalories.Services
{
	public class TempApiKeyProvider
	{
        public static SecureString GetApiKey()
        {
            SecureString secureApiKey = new SecureString();

            AppendToSecureString(secureApiKey, "sk-proj-QgMAb");
            AppendToSecureString(secureApiKey, "ggM8w9pUhiyP2BvT3BlbkFJC");
            AppendToSecureString(secureApiKey, "p1pm1hywYBWL1QNkG1M");

            secureApiKey.MakeReadOnly();

            return secureApiKey;
        }

        private static void AppendToSecureString(SecureString secureString, string value)
        {
            foreach (char c in value)
            {
                secureString.AppendChar(c);
            }
        }

        public static string ConvertSecureStringToString(SecureString secureString)
        {
            if (secureString == null) throw new ArgumentNullException(nameof(secureString));

            IntPtr bstr = IntPtr.Zero;
            try
            {
                bstr = Marshal.SecureStringToBSTR(secureString);
                return Marshal.PtrToStringBSTR(bstr);
            }
            finally
            {
                // Ensure the memory is freed
                if (bstr != IntPtr.Zero)
                {
                    Marshal.FreeBSTR(bstr);
                }
            }
        }
    }
}

