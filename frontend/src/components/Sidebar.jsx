import React, { useState } from 'react';
import './Sidebar.css';
import { 
  FaHome, 
  FaCompass, 
  FaPlus, 
  FaUser, 
  FaSignOutAlt,
  FaUserShield,
  FaBars,
  FaTimes,
  FaHeart,
  FaMapMarkerAlt,
  FaStar
} from 'react-icons/fa';

export default function Sidebar({ setPage }) {
  const [isOpen, setIsOpen] = useState(false);
  const [activePage, setActivePage] = useState('home');
  
  // Check if user is logged in and is admin
  const isLoggedIn = localStorage.getItem('accessToken');
  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const isAdmin = user?.role === 'Admin';

  const handleNavigation = (page) => {
    setActivePage(page);
    setPage(page);
    setIsOpen(false);
  };

  const handleLogout = () => {
    localStorage.clear();
    handleNavigation('home');
    window.location.reload();
  };

  return (
    <>
      {/* Mobile Menu Toggle */}
      <button 
        className="mobile-menu-toggle"
        onClick={() => setIsOpen(!isOpen)}
      >
        {isOpen ? <FaTimes /> : <FaBars />}
      </button>

      {/* Overlay for mobile */}
      {isOpen && (
        <div 
          className="sidebar-overlay"
          onClick={() => setIsOpen(false)}
        />
      )}

      {/* Sidebar */}
      <div className={`sidebar ${isOpen ? 'open' : ''}`}>
        {/* Logo Section */}
        <div className="sidebar-logo" onClick={() => handleNavigation('home')}>
          <div className="logo-icon">
            <FaMapMarkerAlt />
          </div>
          <h2>City Secrets</h2>
          <p>Discover Hidden Gems</p>
        </div>

        {/* User Info */}
        {isLoggedIn && (
          <div className="user-info-section">
            <div className="user-avatar">
              {user.username?.charAt(0).toUpperCase() || 'U'}
            </div>
            <div className="user-details">
              <h4>{user.username || 'Explorer'}</h4>
              <span className={`user-role ${isAdmin ? 'admin' : 'user'}`}>
                {user.role || 'User'}
              </span>
            </div>
          </div>
        )}

        {/* Main Navigation */}
        <div className="sidebar-nav">
          <button 
            className={`nav-item ${activePage === 'home' ? 'active' : ''}`}
            onClick={() => handleNavigation('home')}
          >
            <FaHome className="nav-icon" />
            <span className="nav-label">Home</span>
          </button>

          <button 
            className={`nav-item ${activePage === 'explore' ? 'active' : ''}`}
            onClick={() => handleNavigation('explore')}
          >
            <FaCompass className="nav-icon" />
            <span className="nav-label">Explore</span>
          </button>

          {isLoggedIn && (
            <>
              <button 
                className={`nav-item ${activePage === 'feed' ? 'active' : ''}`}
                onClick={() => handleNavigation('feed')}
              >
                <FaHeart className="nav-icon" />
                <span className="nav-label">Feed</span>
              </button>

              <button 
                className={`nav-item ${activePage === 'hidden-gem' ? 'active' : ''}`}
                onClick={() => handleNavigation('hidden-gem')}
              >
                <FaPlus className="nav-icon" />
                <span className="nav-label">Submit Place</span>
              </button>

              {isAdmin && (
                <button 
                  className={`nav-item admin ${activePage === 'admin' ? 'active' : ''}`}
                  onClick={() => handleNavigation('admin')}
                >
                  <FaUserShield className="nav-icon" />
                  <span className="nav-label">Admin Dashboard</span>
                </button>
              )}
            </>
          )}
        </div>

        {/* Divider */}
        <div className="sidebar-divider"></div>

        {/* Quick Actions */}
        <div className="quick-actions">
          {!isLoggedIn ? (
            <button 
              className="action-btn login-btn"
              onClick={() => handleNavigation('home')}
            >
              <FaUser /> Login / Register
            </button>
          ) : (
            <button 
              className="action-btn explore-btn"
              onClick={() => handleNavigation('explore')}
            >
              <FaStar /> Explore Gems
            </button>
          )}
        </div>

        {/* Footer */}
        <div className="sidebar-footer">
          {isLoggedIn && (
            <button className="logout-btn" onClick={handleLogout}>
              <FaSignOutAlt /> Logout
            </button>
          )}
          <div className="app-info">
            <span>City Secrets v1.0</span>
          </div>
        </div>
      </div>
    </>
  );
}