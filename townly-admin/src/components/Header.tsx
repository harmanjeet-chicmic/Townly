import React from 'react';
import './Header.css';

const Header: React.FC = () => {
  return (
    <header className="header">
      <div className="header-left">
        <h1>Welcome back, Admin</h1>
        <p>Here's what's happening with your platform today</p>
      </div>
      <div className="header-right">
        <div className="admin-profile">
          <span className="admin-avatar">👤</span>
          <span className="admin-name">Admin User</span>
        </div>
      </div>
    </header>
  );
};

export default Header;