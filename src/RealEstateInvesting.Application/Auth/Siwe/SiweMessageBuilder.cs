public static class SiweMessageBuilder
{
    public static string Build(
        string domain,
        string wallet,
        long chainId,
        string nonce,
        DateTime issuedAt,
        DateTime expiresAt)
    {
        return
$@"{domain} wants you to sign in with your Ethereum account:
{wallet}

URI: https://{domain}
Version: 1
Chain ID: {chainId}
Nonce: {nonce}
Issued At: {issuedAt:O}
Expiration Time: {expiresAt:O}";
    }
}
