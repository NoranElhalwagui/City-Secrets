// pages/admin/AdminDashboard.jsx
import React from "react";
import "./AdminDashboard.css";

export default function AdminDashboard() {
  return (
    <div className="admin-container">
      <h1 className="page-title">Admin Dashboard</h1>

      <div className="stats">
        <div className="stat-card">Users: 120</div>
        <div className="stat-card">Places: 45</div>
        <div className="stat-card">Reviews: 300</div>
      </div>

      <div className="admin-actions">
        <button>Approve Places</button>
        <button>Manage Users</button>
        <button>Flagged Reviews</button>
      </div>
    </div>
  );
}
