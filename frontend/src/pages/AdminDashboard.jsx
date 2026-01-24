import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./AdminDashboard.css";

export default function AdminDashboard() {
  const navigate = useNavigate();

  const [stats, setStats] = useState({
    users: 0,
    places: 0,
    reviews: 0,
    pending: 0,
  });

  useEffect(() => {
    const pendingPlaces =
      JSON.parse(localStorage.getItem("pendingPlaces")) || [];
    const approvedPlaces =
      JSON.parse(localStorage.getItem("approvedPlaces")) || [];

    setStats({
      users: 120, // mock number
      places: approvedPlaces.length,
      reviews: 300, // mock number
      pending: pendingPlaces.length,
    });
  }, []);

  return (
    <div className="admin-container">
      <h1 className="page-title">Admin Dashboard</h1>

      {/* DASHBOARD STATS */}
      <section className="dashboard-stats">
        <h2>Dashboard Stats</h2>
        <div className="stats">
          <div className="stat-card">Users: {stats.users}</div>
          <div className="stat-card">Approved Places: {stats.places}</div>
          <div className="stat-card">Pending Places: {stats.pending}</div>
          <div className="stat-card">Reviews: {stats.reviews}</div>
        </div>
      </section>

      {/* PLACES MANAGEMENT */}
      <section className="places-management">
        <h2>Places Management</h2>
        <div className="admin-actions">
          <button
            onClick={() => navigate("/admin/pending-places")}
          >
            View Pending Places
          </button>

          <button
            onClick={() => navigate("/explore")}
          >
            View Approved Places
          </button>
        </div>
      </section>

      {/* USER MANAGEMENT */}
      <section className="user-management">
        <h2>User Management</h2>
        <div className="admin-actions">
          <button disabled>Ban User</button>
          <button disabled>Unban User</button>
        </div>
        
      </section>

      {/* CATEGORIES MANAGEMENT */}
      <section className="categories-management">
        <h2>Categories Management</h2>
        <div className="admin-actions">
          <button disabled>List Categories</button>
          <button disabled>Add Category</button>
          <button disabled>Edit Category</button>
          <button disabled>Delete Category</button>
          <button disabled>Manage Subcategories</button>
        </div>
        
      </section>
    </div>
  );
}
