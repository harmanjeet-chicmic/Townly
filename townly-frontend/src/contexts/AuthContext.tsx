import React, { createContext, useContext, useState, useEffect } from 'react';
import { useAccount, useSignMessage, useDisconnect } from 'wagmi';
import api from '../services/api';

interface AuthContextType {
    user: any;
    token: string | null;
    login: () => Promise<void>;
    logout: () => void;
    isAuthenticated: boolean;
    isAuthenticating: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
    const { address, isConnected, chainId } = useAccount();
    const { signMessageAsync } = useSignMessage();
    const { disconnect } = useDisconnect();

    const [user, setUser] = useState<any>(null);
    const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
    const [isAuthenticating, setIsAuthenticating] = useState(false);

    useEffect(() => {
        if (!isConnected && token) {
            logout();
        }
    }, [isConnected]);

    const login = async () => {
        if (!address) return;

        try {
            setIsAuthenticating(true);

            // 1. Get Nonce
            const nonceRes = await api.post('/api/v1/auth/wallet/nonce');
            const { nonce } = nonceRes.data.data;

            // 2. Sign Message
            const message = `Townly Authentication\n\nNonce: ${nonce}`;
            const signature = await signMessageAsync({ message });

            // 3. Verify Signature
            const verifyRes = await api.post('/api/v1/auth/wallet/verify', {
                walletAddress: address,
                signature,
                nonce,
                message,
                chainId: chainId || 1 // Fallback to Ethereum mainnet if chainId is missing
            });

            const { accessToken, user: userData } = verifyRes.data.data;

            if (accessToken) {
                localStorage.setItem('token', accessToken);
                setToken(accessToken);
                setUser(userData || { walletAddress: address });
            }

        } catch (error) {
            console.error('Authentication failed', error);
            disconnect();
        } finally {
            setIsAuthenticating(false);
        }
    };

    const logout = () => {
        localStorage.removeItem('token');
        setToken(null);
        setUser(null);
        disconnect();
    };

    return (
        <AuthContext.Provider value={{
            user,
            token,
            login,
            logout,
            isAuthenticated: !!token,
            isAuthenticating
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) throw new Error('useAuth must be used within an AuthProvider');
    return context;
};
