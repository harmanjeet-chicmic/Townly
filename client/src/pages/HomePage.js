import React from 'react';
import './HomePage.css';

const HomePage = () => {
  return (
    <div className="homepage">
      <section className="hero-section">
        <div className="hero-content">
          <h1 className="hero-title">
            Invest in Real Estate
            <span className="gradient-text"> with Blockchain</span>
          </h1>
          <p className="hero-subtitle">
            Fractional ownership, transparent transactions, and passive income through tokenized real estate investments
          </p>
          <div className="hero-cta">
            <button className="btn btn-primary">Explore Properties</button>
            <button className="btn btn-secondary">How It Works</button>
          </div>
        </div>
        <div className="hero-stats">
          <div className="stat-item">
            <h3>$50M+</h3>
            <p>Total Value Locked</p>
          </div>
          <div className="stat-item">
            <h3>10,000+</h3>
            <p>Active Investors</p>
          </div>
          <div className="stat-item">
            <h3>15%</h3>
            <p>Average ROI</p>
          </div>
        </div>
      </section>
    </div>
  );
};

export default HomePage;