export function buildSiweMessage({ domain, wallet, chainId, nonce }) {
  const issuedAt = new Date().toISOString();

  const expiresAt = new Date(Date.now() + 5 * 60 * 1000).toISOString();

  return `${domain} wants you to sign in with your Ethereum account:
${wallet}

URI: https://${domain}
Version: 1
Chain ID: ${chainId}
Nonce: ${nonce}
Issued At: ${issuedAt}
Expiration Time: ${expiresAt}`;
}
