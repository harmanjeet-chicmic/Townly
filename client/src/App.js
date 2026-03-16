import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { WalletProvider } from './context/WalletContext';
import Navbar from './components/common/Navbar';
import HomePage from './pages/HomePage';
import MarketplacePage from './pages/MarketplacePage';
import PropertyDetailPage from './pages/PropertyDetailPage';
import './styles/global.css';

function App() {
  return (
    <Router>
      <WalletProvider>
        <div className="app">
          <Navbar />
          <main className="main-content">
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/marketplace" element={<MarketplacePage />} />
              <Route path="/property/:id" element={<PropertyDetailPage />} />
            </Routes>
          </main>
        </div>
      </WalletProvider>
    </Router>
  );
}

export default App;