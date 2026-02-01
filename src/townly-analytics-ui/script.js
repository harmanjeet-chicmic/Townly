// Configuration
const API_BASE_URL = 'http://localhost:5168/api/v1/auth';
const CHAIN_ID = 1; // Ethereum Mainnet

// DOM Elements
const connectMetamaskBtn = document.getElementById('connectMetamask');
const signatureSection = document.getElementById('signatureSection');
const signMessageEl = document.getElementById('signMessage');
const signBtn = document.getElementById('signBtn');
const messageEl = document.getElementById('message');
const loadingEl = document.getElementById('loading');

// State
let walletAddress = null;
let nonce = null;
let provider = null;
let signer = null;

// Event Listeners
connectMetamaskBtn.addEventListener('click', connectMetaMask);
signBtn.addEventListener('click', signAndVerify);

// Main Wallet Connection
async function connectMetaMask() {
    try {
        showLoading(true);
        clearMessage();
        console.log('Starting MetaMask connection...');
        
        // Check if MetaMask is installed
        if (!window.ethereum) {
            throw new Error('Please install MetaMask extension!');
        }

        console.log('MetaMask detected, requesting accounts...');
        
        // Request account access
        const accounts = await window.ethereum.request({ 
            method: 'eth_requestAccounts' 
        });
        
        console.log('Accounts received:', accounts);
        
        if (!accounts || accounts.length === 0) {
            throw new Error('No accounts found. Please unlock MetaMask.');
        }
        
        walletAddress = accounts[0];
        console.log('Wallet address:', walletAddress);
        
        provider = new ethers.providers.Web3Provider(window.ethereum);
        signer = provider.getSigner();
        
        // Check network
        const network = await provider.getNetwork();
        console.log('Current network chainId:', network.chainId);
        
        if (network.chainId !== CHAIN_ID) {
            showMessage(`Please switch to Ethereum Mainnet (Chain ID: ${CHAIN_ID})`, 'error');
            showLoading(false);
            return;
        }
        
        showMessage(`Wallet connected: ${formatAddress(walletAddress)}`, 'success');
        console.log('Wallet connected successfully, requesting nonce...');
        
        // Request nonce from backend
        await requestNonce();
        
    } catch (error) {
        console.error('Connection error:', error);
        showMessage(error.message || 'Failed to connect wallet', 'error');
        showLoading(false);
    }
}

// Request nonce from backend
async function requestNonce() {
    try {
        showMessage('Requesting authentication challenge...', 'success');
        console.log('Sending nonce request...');
        
        const response = await fetch(`${API_BASE_URL}/wallet/nonce`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                walletAddress: walletAddress,
                chainId: CHAIN_ID
            })
        });

        console.log('Response status:', response.status);
        
        const result = await response.json();
        console.log('API Response:', result);
        
        // Check if response is successful
        if (response.ok && result.statusCode >= 200 && result.statusCode < 300) {
            nonce = result.data.nonce;
            console.log('Nonce received:', nonce);
            showSignatureRequest();
        } else {
            throw new Error(result.message || `Server error: ${result.statusCode}`);
        }
        
    } catch (error) {
        console.error('Nonce request error:', error);
        showMessage(error.message || 'Failed to get authentication challenge', 'error');
        showLoading(false);
    }
}

// Show signature request with nonce
function showSignatureRequest() {
    // Generate message exactly as backend expects
    const message = `Sign this message to authenticate with RealEstateInvesting\n\nNonce: ${nonce}\nWallet: ${walletAddress}\nChain ID: ${CHAIN_ID}\nTimestamp: ${new Date().toISOString()}`;
    
    console.log('Message to sign:', message);
    signMessageEl.textContent = message;
    signatureSection.style.display = 'block';
    showLoading(false);
}

// Sign message and verify with backend
async function signAndVerify() {
    try {
        showLoading(true);
        clearMessage();
        
        if (!signer || !nonce) {
            throw new Error('Wallet not connected or authentication challenge missing');
        }

        // Get the exact same message from the display
        const message = signMessageEl.textContent;
        
        showMessage('Signing message...', 'success');
        console.log('Signing message...');
        
        // Sign the message
        const signature = await signer.signMessage(message);
        console.log('Signature generated');
        
        // Verify signature with backend
        showMessage('Verifying signature...', 'success');
        console.log('Sending verification request...');
        
        const response = await fetch(`${API_BASE_URL}/wallet/verify`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                walletAddress: walletAddress,
                chainId: CHAIN_ID,
                message: message,
                signature: signature
            })
        });

        const result = await response.json();
        console.log('Verify response:', result);
        
        // Check if verification successful
        if (response.ok && result.statusCode >= 200 && result.statusCode < 300) {
            // Store JWT token and user data
            localStorage.setItem('jwt_token', result.data.accessToken);
            localStorage.setItem('wallet_address', walletAddress);
            localStorage.setItem('token_expires', result.data.expiresAt);
            localStorage.setItem('auth_timestamp', new Date().toISOString());
            
            console.log('Auth successful, token stored');
            
            showMessage('✅ Authentication successful! Redirecting to marketplace...', 'success');
            
            // Redirect to marketplace after 2 seconds
            setTimeout(() => {
                window.location.href = 'marketplace.html';
            }, 2000);
            
        } else {
            throw new Error(result.message || `Authentication failed: ${result.statusCode}`);
        }
        
    } catch (error) {
        console.error('Signature error:', error);
        showMessage(error.message || 'Failed to authenticate', 'error');
        showLoading(false);
    }
}

// Utility Functions
function showMessage(message, type = 'info') {
    messageEl.textContent = message;
    messageEl.className = `message ${type}`;
    messageEl.style.display = 'block';
}

function clearMessage() {
    messageEl.textContent = '';
    messageEl.className = 'message';
    messageEl.style.display = 'none';
}

function showLoading(show) {
    loadingEl.style.display = show ? 'block' : 'none';
}

function formatAddress(address) {
    return `${address.substring(0, 6)}...${address.substring(address.length - 4)}`;
}

// Check if user is already logged in
function checkExistingLogin() {
    const token = localStorage.getItem('jwt_token');
    const expires = localStorage.getItem('token_expires');
    
    if (token && expires) {
        const expiryDate = new Date(expires);
        if (expiryDate > new Date()) {
            console.log('Found valid token, redirecting...');
            window.location.href = 'marketplace.html';
        } else {
            console.log('Token expired, clearing storage');
            localStorage.clear();
        }
    }
}

// Initialize
console.log('Wallet auth script loaded');
checkExistingLogin();

// Listen for account changes (MetaMask)
if (window.ethereum) {
    window.ethereum.on('accountsChanged', (accounts) => {
        console.log('Accounts changed:', accounts);
        if (accounts.length === 0) {
            showMessage('Wallet disconnected', 'error');
            signatureSection.style.display = 'none';
            localStorage.clear();
            walletAddress = null;
            nonce = null;
        } else if (accounts[0] !== walletAddress) {
            showMessage('Account changed. Please re-authenticate.', 'error');
            signatureSection.style.display = 'none';
            localStorage.clear();
            walletAddress = accounts[0];
        }
    });
}