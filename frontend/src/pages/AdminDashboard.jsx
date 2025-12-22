import React from "react";
import "./AdminDashboard.css";

export default function AdminDashboard() {
  return (
    <div className="admin-container">
      <h1 className="page-title">Admin Dashboard</h1>

      <section className="dashboard-stats">
        <h2>Dashboard Stats</h2>
        <div className="stats">
          <div className="stat-card">Users: 120</div>
          <div className="stat-card">Places: 45</div>
          <div className="stat-card">Reviews: 300</div>
          <div className="stat-card">Analytics: View</div>
          <div className="stat-card">Logs: View</div>
        </div>
      </section>

      <section className="places-management">
        <h2>Places Management</h2>
        <div className="admin-actions">
          <button>View Pending Places</button>
          <button>Verify/Approve Place</button>
        </div>
      </section>

      <section className="reviews-moderation">
        <h2>Reviews Moderation</h2>
        <div className="admin-actions">
          <button>Flagged Reviews</button>
          <button>Remove Flagged Review</button>
          <button>Unflag Review</button>
        </div>
      </section>

      <section className="user-management">
        <h2>User Management</h2>
        <div className="admin-actions">
          <button>Ban User</button>
          <button>Unban User</button>
        </div>
      </section>

      <section className="categories-management">
        <h2>Categories Management</h2>
        <div className="admin-actions">
          <button>List Categories</button>
          <button>Add Category</button>
          <button>Edit Category</button>
          <button>Delete Category</button>
          <button>Manage Subcategories</button>
        </div>
      </section>
    </div>
  );
}
