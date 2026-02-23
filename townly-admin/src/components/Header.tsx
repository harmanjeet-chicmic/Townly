import React from 'react';
import { Bell, Search, User } from 'lucide-react';
import './Header.css';

const Header: React.FC = () => {
  return (
    <header className="header">
      <div className="header-left">
        <div className="search-bar">
          <Search size={18} />
          <input type="text" placeholder="Search for properties, users..." />
        </div>
      </div>

      <div className="header-right">
        <button className="icon-btn">
          <Bell size={20} />
          <span className="notification-dot"></span>
        </button>

        <div className="admin-profile">
          <div className="admin-info">
            <span className="admin-name">Harmanjeet Singh</span>
            <span className="admin-role">Super Admin</span>
          </div>
          <div className="admin-avatar">
            <User size={20} />
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
