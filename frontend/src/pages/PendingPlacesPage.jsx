import React, { useEffect, useState } from "react";
import "./PendingPlacesPage.css";

export default function PendingPlacesPage() {
  const [pendingPlaces, setPendingPlaces] = useState([]);

  useEffect(() => {
    const stored = JSON.parse(localStorage.getItem("pendingPlaces")) || [];
    setPendingPlaces(stored);
  }, []);

  const approvePlace = (id) => {
    const approved = pendingPlaces.find(p => p.id === id);
    const remaining = pendingPlaces.filter(p => p.id !== id);

    const approvedPlaces =
      JSON.parse(localStorage.getItem("approvedPlaces")) || [];

    localStorage.setItem(
      "approvedPlaces",
      JSON.stringify([...approvedPlaces, { ...approved, status: "approved" }])
    );

    localStorage.setItem(
      "pendingPlaces",
      JSON.stringify(remaining)
    );

    setPendingPlaces(remaining);
  };

  const rejectPlace = (id) => {
    const remaining = pendingPlaces.filter(p => p.id !== id);

    localStorage.setItem(
      "pendingPlaces",
      JSON.stringify(remaining)
    );

    setPendingPlaces(remaining);
  };

  return (
    <div className="admin-container">
      <h1>Pending Places</h1>

      {pendingPlaces.length === 0 && (
        <p>No pending places 🎉</p>
      )}

      {pendingPlaces.map(place => (
        <div key={place.id} className="pending-card">
          <h3>{place.name}</h3>
          <p>{place.description}</p>
          <p><strong>Location:</strong> {place.location}</p>
          <p><strong>Category:</strong> {place.category}</p>

          <div className="actions">
            <button className="approve" onClick={() => approvePlace(place.id)}>
              Approve
            </button>
            <button className="reject" onClick={() => rejectPlace(place.id)}>
              Reject
            </button>
          </div>
        </div>
      ))}
    </div>
  );
}
