import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import PrivateRoute from './components/PrivateRoute';
import Sidebar from './components/Sidebar';
import Header from './components/Header';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import KycPending from './pages/KycPending';
import PropertiesPending from './pages/PropertiesPending';
import PropertyUpdatesPending from './pages/PropertyUpdatesPending';
import TokenRequestsPending from './pages/TokenRequestsPending';
import './App.css';

const AuthenticatedLayout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  return (
    <div className="app">
      <Sidebar />
      <div className="main-content">
        <Header />
        <div className="content-area">
          {children}
        </div>
      </div>
    </div>
  );
};

const AppRoutes = () => {
  const { token } = useAuth();

  return (
    <Routes>
      <Route path="/login" element={
        token ? <Navigate to="/dashboard" /> : <Login />
      } />
      
      <Route path="/" element={
        <PrivateRoute>
          <AuthenticatedLayout>
            <Navigate to="/dashboard" />
          </AuthenticatedLayout>
        </PrivateRoute>
      } />
      
      <Route path="/dashboard" element={
        <PrivateRoute>
          <AuthenticatedLayout>
            <Dashboard />
          </AuthenticatedLayout>
        </PrivateRoute>
      } />
      
      <Route path="/kyc" element={
        <PrivateRoute>
          <AuthenticatedLayout>
            <KycPending />
          </AuthenticatedLayout>
        </PrivateRoute>
      } />
      
      <Route path="/properties" element={
        <PrivateRoute>
          <AuthenticatedLayout>
            <PropertiesPending />
          </AuthenticatedLayout>
        </PrivateRoute>
      } />
      
      <Route path="/property-updates" element={
        <PrivateRoute>
          <AuthenticatedLayout>
            <PropertyUpdatesPending />
          </AuthenticatedLayout>
        </PrivateRoute>
      } />
      
      <Route path="/token-requests" element={
        <PrivateRoute>
          <AuthenticatedLayout>
            <TokenRequestsPending />
          </AuthenticatedLayout>
        </PrivateRoute>
      } />
    </Routes>
  );
};

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;