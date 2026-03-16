import React, { createContext, useState, useContext, useEffect } from 'react';
import { ethers } from 'ethers';
import { requestNonce, verifyWallet } from '../services/authService';
import { buildSiweMessage } from '../utils/siweMessage';

const WalletContext = createContext();

export const useWallet = () => {
  const context = useContext(WalletContext);
  if (!context) {
    throw new Error('useWallet must be used within a WalletProvider');
  }
  return context;
};

export const WalletProvider = ({ children }) => {
  const [wallet, setWallet] = useState({
    address: null,
    isConnected: false,
    balance: null,
    chainId: null,
  });
  const [isConnecting, setIsConnecting] = useState(false);
  const [error, setError] = useState(null);

  useEffect(() => {
    // Check if wallet was previously connected
    const token = localStorage.getItem('token');
    const savedAddress = localStorage.getItem('walletAddress');
    if (token && savedAddress) {
      setWallet(prev => ({
        ...prev,
        address: savedAddress,
        isConnected: true
      }));
    }
  }, []);

  const connectWallet = async () => {
    setIsConnecting(true);
    setError(null);
    
    try {
      if (!window.ethereum) {
        throw new Error("Please install MetaMask");
      }

      const provider = new ethers.BrowserProvider(window.ethereum);
      await provider.send("eth_requestAccounts", []);
      const signer = await provider.getSigner();
      const walletAddress = await signer.getAddress();
      const network = await provider.getNetwork();
      const chainId = Number(network.chainId);

      // Get balance
      const balance = await provider.getBalance(walletAddress);
      const balanceInEth = Number(ethers.formatEther(balance));

      // Get nonce from backend
      const nonce = await requestNonce();
      const domain = window.location.host;

      // Build SIWE message
      const message = buildSiweMessage({
        domain,
        wallet: walletAddress,
        chainId,
        nonce,
      });

      // Sign message
      const signature = await signer.signMessage(message);

      // Verify with backend
      const result = await verifyWallet({
        walletAddress,
        chainId,
        signature,
        message,
      });

      // Store token and wallet info
      localStorage.setItem("token", result.accessToken);
      localStorage.setItem("walletAddress", walletAddress);

      setWallet({
        address: walletAddress,
        isConnected: true,
        balance: balanceInEth,
        chainId: chainId,
      });

    } catch (err) {
      console.error("Wallet connection error:", err);
      setError(err.message || "Wallet connection failed");
    } finally {
      setIsConnecting(false);
    }
  };

  const disconnectWallet = () => {
    setWallet({
      address: null,
      isConnected: false,
      balance: null,
      chainId: null,
    });
    localStorage.removeItem('token');
    localStorage.removeItem('walletAddress');
  };

  const value = {
    wallet,
    isConnecting,
    error,
    connectWallet,
    disconnectWallet,
  };

  return (
    <WalletContext.Provider value={value}>
      {children}
    </WalletContext.Provider>
  );
};