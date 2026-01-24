import React, { useEffect, useState } from "react";

export default function PendingPage() {
  const [pendingPlaces, setPendingPlaces] = useState([]);

  useEffect(() => {
    const data =
      JSON.parse(localStorage.getItem("pendingPlaces")) || [];
    setPendingPlaces(data.filter(p => p.status === "pending"));
  }, []);

  const updateStatus = (id, status) => {
    const all =
      JSON.parse(localStorage.getItem("pendingPlaces")) || [];

    const updated = all.map(p =>
      p.id === id ? { ...p, status } : p
    );

    localStorage.setItem(
      "pendingPlaces",
      JSON.stringify(updated)
    );

    setPendingPlaces(updated.filter(p => p.status === "pending"));
  };

  return (
    <div className="admin-container">
      <h2>Pending Place Requests</h2>

      {pendingPlaces.length === 0 && <p>No pending requests</p>}

      {pendingPlaces.map(place => (
        <div key={place.id} className="pending-card">
          <h3>{place.name}</h3>

          <p><b>Location:</b> {place.location}</p>
          <p><b>Category:</b> {place.category}</p>
          <p><b>Description:</b> {place.description}</p>
          <p><b>Address:</b> {place.address}</p>
          <p><b>Email:</b> {place.email}</p>

          <div className="admin-actions">
            <button onClick={() => updateStatus(place.id, "approved")}>
              ✅ Approve
            </button>
            <button onClick={() => updateStatus(place.id, "rejected")}>
              ❌ Reject
            </button>
          </div>
        </div>
      ))}
    </div>
  );
}
