import React from 'react';
import { NavLink } from 'react-router-dom';
import {
  LayoutDashboard,
  UserCheck,
  Building2,
  RefreshCcw,
  Coins,
  LogOut
} from 'lucide-react';
import { useAuth } from '../contexts/AuthContext';
import './Sidebar.css';

const Sidebar: React.FC = () => {
  const { logout } = useAuth();

  const menuItems = [
    { path: '/dashboard', icon: <LayoutDashboard size={20} />, label: 'Dashboard' },
    { path: '/kyc', icon: <UserCheck size={20} />, label: 'KYC Reviews' },
    { path: '/properties', icon: <Building2 size={20} />, label: 'Properties' },
    { path: '/property-updates', icon: <RefreshCcw size={20} />, label: 'Property Updates' },
    { path: '/token-requests', icon: <Coins size={20} />, label: 'Token Requests' },
  ];

  return (
    <div className="sidebar">
      <div className="sidebar-header">
        <div className="logo">
          <div className="logo-icon">T</div>
          <h2>Townly<span>Admin</span></h2>
        </div>
      </div>

      <nav className="sidebar-nav">
        {menuItems.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}
          >
            <span className="nav-icon">{item.icon}</span>
            <span className="nav-label">{item.label}</span>
          </NavLink>
        ))}
      </nav>

      <div className="sidebar-footer">
        <button onClick={logout} className="logout-btn">
          <span className="nav-icon"><LogOut size={20} /></span>
          <span className="nav-label">Logout</span>
        </button>
      </div>
    </div>
  );
};

export default Sidebar;