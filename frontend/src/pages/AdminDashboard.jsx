import React, { useState } from "react";
import "./AdminDashboard.css";

export default function AdminDashboard() {
  const [pendingPlaces, setPendingPlaces] = useState([]);
  const [loading, setLoading] = useState(false);

  const token = localStorage.getItem("token");

  const fetchPendingPlaces = async () => {
    setLoading(true);
    try {
      const res = await fetch(
        "https://localhost:5001/api/admin/pending-places",
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!res.ok) throw new Error("Unauthorized");

      const data = await res.json();
      setPendingPlaces(data);
    } catch (err) {
      alert("Failed to load pending places");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="admin-container">
      <h1 className="page-title">Admin Dashboard</h1>

      {/* DASHBOARD STATS */}
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

      {/* PLACES MANAGEMENT */}
      <section className="places-management">
        <h2>Places Management</h2>
        <div className="admin-actions">
          <button onClick={fetchPendingPlaces}>
            View Pending Places
          </button>
          <button>Verify/Approve Place</button>
        </div>

        {/* PENDING PLACES LIST */}
        {loading && <p>Loading pending places...</p>}

        {pendingPlaces.map((place) => (
          <div
            key={place.id}
            className="stat-card"
            style={{ marginTop: "15px" }}
          >
            <h3>{place.name}</h3>
            <p>{place.description}</p>
            <p>
              <strong>Submitted by:</strong> {place.createdBy}
            </p>
          </div>
        ))}
      </section>

      {/* USER MANAGEMENT */}
      <section className="user-management">
        <h2>User Management</h2>
        <div className="admin-actions">
          <button>Ban User</button>
          <button>Unban User</button>
        </div>
      </section>

      {/* CATEGORIES MANAGEMENT */}
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
