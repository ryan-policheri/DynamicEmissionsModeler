namespace DotNetCommon.Security
{
    public interface ICredentialProvider
    {
        string DecryptValue(string value);
        string EncryptValue(string value);
    }
}