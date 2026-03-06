import "./WalletLogin.css";
import { ethers } from "ethers";
import { requestNonce, verifyWallet } from "../../services/authService";
import { buildSiweMessage } from "../../utils/siweMessage";

export default function WalletLogin() {

  const connectWallet = async () => {
    try {

      if (!window.ethereum) {
        alert("Please install MetaMask");
        return;
      }

      const provider = new ethers.BrowserProvider(window.ethereum);

      await provider.send("eth_requestAccounts", []);

      const signer = await provider.getSigner();

      const walletAddress = await signer.getAddress();

      const network = await provider.getNetwork();

      const chainId = Number(network.chainId);

      const nonce = await requestNonce();

      const domain = window.location.host;

      const message = buildSiweMessage({
        domain,
        wallet: walletAddress,
        chainId,
        nonce,
      });

      const signature = await signer.signMessage(message);

      const result = await verifyWallet({
        walletAddress,
        chainId,
        signature,
        message,
      });

      localStorage.setItem("token", result.accessToken);

      alert("Wallet connected!");

    } catch (error) {
      console.error(error);
      alert("Wallet connection failed");
    }
  };

  return (
    <div className="wallet-container">
      <div className="wallet-card">
        <h1>Townly</h1>
        <p>Invest in Real Estate using Web3</p>

        <button onClick={connectWallet}>
          Connect Wallet
        </button>
      </div>
    </div>
  );
}