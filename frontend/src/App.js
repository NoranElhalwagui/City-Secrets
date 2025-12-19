import React, { useState, useEffect } from 'react';
import './App.css';
import Sidebar from './components/Sidebar';

// Import all pages
import HomePage from './pages/HomePage';
import FeedPage from './pages/FeedPage';
import ExplorePage from './pages/ExplorePage';
import HiddenGemOwnerPage from './pages/HiddenGemOwnerPage';
import AdminDashboard from './pages/AdminDashboard';

// Import auth service
import { authService } from './services';

function App() {
  const [currentPage, setCurrentPage] = useState('home');
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    checkAuthentication();
  }, []);

  const checkAuthentication = () => {
    const token = localStorage.getItem('accessToken');
    const storedUser = JSON.parse(localStorage.getItem('user') || '{}');
    
    if (token) {
      setIsAuthenticated(true);
      setUser(storedUser);
      
      // Verify token with backend
      authService.getCurrentUser()
        .then(result => {
          if (result.success) {
            setUser(result.user);
          } else {
            // Token expired or invalid
            handleLogout();
          }
        })
        .catch(() => {
          handleLogout();
        })
        .finally(() => {
          setLoading(false);
        });
    } else {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.clear();
    setIsAuthenticated(false);
    setUser(null);
    setCurrentPage('home');
  };

  const handleLoginSuccess = (userData) => {
    setIsAuthenticated(true);
    setUser(userData);
    setCurrentPage('feed'); // Redirect to feed after login
  };

  const renderPage = () => {
    if (loading) {
      return (
        <div className="loading-screen">
          <div className="spinner"></div>
          <p>Loading City Secrets...</p>
        </div>
      );
    }

    switch (currentPage) {
      case 'home':
        return <HomePage setPage={setCurrentPage} onLoginSuccess={handleLoginSuccess} />;
      
      case 'feed':
        if (!isAuthenticated) {
          return <HomePage setPage={setCurrentPage} onLoginSuccess={handleLoginSuccess} />;
        }
        return <FeedPage setPage={setCurrentPage} user={user} />;
      
      case 'explore':
        return <ExplorePage setPage={setCurrentPage} user={user} />;
      
      case 'hidden-gem':
        if (!isAuthenticated) {
          alert('Please login to submit a hidden gem!');
          return <HomePage setPage={setCurrentPage} onLoginSuccess={handleLoginSuccess} />;
        }
        return <HiddenGemOwnerPage setPage={setCurrentPage} user={user} />;
      
      case 'admin':
        if (!isAuthenticated || user?.role !== 'Admin') {
          alert('Admin access required!');
          return <HomePage setPage={setCurrentPage} onLoginSuccess={handleLoginSuccess} />;
        }
        return <AdminDashboard setPage={setCurrentPage} user={user} />;
      
      default:
        return <HomePage setPage={setCurrentPage} onLoginSuccess={handleLoginSuccess} />;
    }
  };

  return (
    <div className="App">
      <Sidebar setPage={setCurrentPage} />
      <main className="main-content">
        {renderPage()}
      </main>
    </div>
  );
}

export default App;