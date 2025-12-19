import React, { useEffect, useState } from "react";
import "./AdminDashboard.css";
import Sidebar from "../components/Sidebar";
import axios from "axios";
import {
  FaUsers,
  FaMapMarkerAlt,
  FaStar,
  FaFlag,
  FaCheckCircle,
  FaTimesCircle,
  FaTrash,
  FaEdit,
  FaBan,
  FaChartLine
} from "react-icons/fa";

export default function AdminDashboard({ setPage }) {
  const [stats, setStats] = useState({
    totalUsers: 0,
    totalPlaces: 0,
    pendingApprovals: 0,
    flaggedContent: 0
  });
  const [pendingPlaces, setPendingPlaces] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [user, setUser] = useState(null);

  useEffect(() => {
    checkAdminAccess();
    fetchDashboardData();
  }, []);

  const checkAdminAccess = () => {
    const token = localStorage.getItem('accessToken');
    const storedUser = JSON.parse(localStorage.getItem('user') || '{}');
    
    if (!token || storedUser.role !== 'Admin') {
      alert('Admin access required');
      setPage('home');
      return;
    }
    
    setUser(storedUser);
  };

  const fetchDashboardData = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('accessToken');
      
      // Fetch dashboard stats
      const statsResponse = await axios.get('http://localhost:5000/api/admin/dashboard', {
        headers: { Authorization: `Bearer ${token}` }
      });
      setStats(statsResponse.data);

      // Fetch pending places
      const placesResponse = await axios.get('http://localhost:5000/api/admin/pending-places', {
        headers: { Authorization: `Bearer ${token}` }
      });
      setPendingPlaces(placesResponse.data);

      // Fetch all users
      const usersResponse = await axios.get('http://localhost:5000/api/admin/users', {
        headers: { Authorization: `Bearer ${token}` }
      });
      setUsers(usersResponse.data);

    } catch (error) {
      console.error('Error fetching admin data:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleApprovePlace = async (placeId) => {
    try {
      const token = localStorage.getItem('accessToken');
      await axios.post(`http://localhost:5000/api/admin/places/${placeId}/verify`, {}, {
        headers: { Authorization: `Bearer ${token}` }
      });
      fetchDashboardData();
    } catch (error) {
      console.error('Error approving place:', error);
    }
  };

  const handleRejectPlace = async (placeId) => {
    if (window.confirm('Are you sure you want to reject this place?')) {
      try {
        const token = localStorage.getItem('accessToken');
        await axios.delete(`http://localhost:5000/api/admin/places/${placeId}`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        fetchDashboardData();
      } catch (error) {
        console.error('Error rejecting place:', error);
      }
    }
  };

  const handleBanUser = async (userId) => {
    if (window.confirm('Are you sure you want to ban this user?')) {
      try {
        const token = localStorage.getItem('accessToken');
        await axios.post(`http://localhost:5000/api/admin/users/${userId}/ban`, {}, {
          headers: { Authorization: `Bearer ${token}` }
        });
        fetchDashboardData();
      } catch (error) {
        console.error('Error banning user:', error);
      }
    }
  };

  const handleMakeAdmin = async (userId) => {
    try {
      const token = localStorage.getItem('accessToken');
      await axios.post(`http://localhost:5000/api/admin/users/${userId}/make-admin`, {}, {
        headers: { Authorization: `Bearer ${token}` }
      });
      fetchDashboardData();
    } catch (error) {
      console.error('Error making user admin:', error);
    }
  };

  if (loading) {
    return (
      <div className="admin-loading">
        <div className="spinner"></div>
        <p>Loading Admin Dashboard...</p>
      </div>
    );
  }

  return (
    <div className="admin-container">
      <Sidebar setPage={setPage} />
      
      <div className="admin-content">
        <div className="admin-header">
          <h1>Admin Dashboard</h1>
          <p>Welcome back, {user?.username || 'Admin'}!</p>
        </div>

        {/* Stats Cards */}
        <div className="stats-grid">
          <div className="stat-card">
            <div className="stat-icon users">
              <FaUsers />
            </div>
            <div className="stat-info">
              <h3>{stats.totalUsers}</h3>
              <p>Total Users</p>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon places">
              <FaMapMarkerAlt />
            </div>
            <div className="stat-info">
              <h3>{stats.totalPlaces}</h3>
              <p>Total Places</p>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon pending">
              <FaStar />
            </div>
            <div className="stat-info">
              <h3>{stats.pendingApprovals}</h3>
              <p>Pending Approvals</p>
            </div>
          </div>

          <div className="stat-card">
            <div className="stat-icon flagged">
              <FaFlag />
            </div>
            <div className="stat-info">
              <h3>{stats.flaggedContent}</h3>
              <p>Flagged Content</p>
            </div>
          </div>
        </div>

        <div className="admin-main">
          {/* Left Column: Pending Approvals */}
          <div className="admin-column">
            <div className="admin-card">
              <div className="card-header">
                <h3>Pending Place Approvals</h3>
                <span className="badge">{pendingPlaces.length}</span>
              </div>
              
              {pendingPlaces.length === 0 ? (
                <p className="empty-state">No pending approvals</p>
              ) : (
                <div className="pending-list">
                  {pendingPlaces.map(place => (
                    <div key={place.id} className="pending-item">
                      <div className="place-info">
                        <h4>{place.name}</h4>
                        <p className="location">
                          <FaMapMarkerAlt /> {place.location}
                        </p>
                        <p className="description">
                          {place.description.substring(0, 100)}...
                        </p>
                        <small>
                          Submitted by: {place.submittedBy?.username || 'Unknown'} • 
                          {new Date(place.createdAt).toLocaleDateString()}
                        </small>
                      </div>
                      
                      <div className="place-actions">
                        <button 
                          className="approve-btn"
                          onClick={() => handleApprovePlace(place.id)}
                        >
                          <FaCheckCircle /> Approve
                        </button>
                        <button 
                          className="reject-btn"
                          onClick={() => handleRejectPlace(place.id)}
                        >
                          <FaTimesCircle /> Reject
                        </button>
                        <button className="view-btn">
                          View Details
                        </button>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>

          {/* Right Column: User Management */}
          <div className="admin-column">
            <div className="admin-card">
              <div className="card-header">
                <h3>User Management</h3>
                <span className="badge">{users.length}</span>
              </div>
              
              <div className="user-list">
                {users.map(user => (
                  <div key={user.id} className="user-item">
                    <div className="user-avatar">
                      {user.username.charAt(0).toUpperCase()}
                    </div>
                    
                    <div className="user-details">
                      <h4>{user.username}</h4>
                      <p className="user-email">{user.email}</p>
                      <div className="user-meta">
                        <span className={`role ${user.role?.toLowerCase()}`}>
                          {user.role || 'User'}
                        </span>
                        <span className="status">
                          {user.isBanned ? '❌ Banned' : '✅ Active'}
                        </span>
                      </div>
                    </div>
                    
                    <div className="user-actions">
                      {user.role !== 'Admin' && (
                        <button 
                          className="make-admin-btn"
                          onClick={() => handleMakeAdmin(user.id)}
                        >
                          Make Admin
                        </button>
                      )}
                      
                      {!user.isBanned ? (
                        <button 
                          className="ban-btn"
                          onClick={() => handleBanUser(user.id)}
                        >
                          <FaBan /> Ban
                        </button>
                      ) : (
                        <button className="unban-btn">
                          Unban
                        </button>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            </div>

            {/* Quick Actions */}
            <div className="admin-card quick-actions">
              <h3>Quick Actions</h3>
              <div className="actions-grid">
                <button className="action-btn" onClick={() => {/* Analytics */}}>
                  <FaChartLine />
                  <span>View Analytics</span>
                </button>
                <button className="action-btn" onClick={() => {/* Edit Categories */}}>
                  <FaEdit />
                  <span>Edit Categories</span>
                </button>
                <button className="action-btn" onClick={() => {/* Cleanup */}}>
                  <FaTrash />
                  <span>Cleanup Data</span>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}