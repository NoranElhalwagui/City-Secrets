import React from "react";
import "../App.css";
import Sidebar from "../components/Sidebar";

export default function AdminDashboard({
  setPage,
  setAdmin,
  pendingRequests,
  handleApproveRequest,
  handleDeleteRequest,
  handleEditRequest
}) {
  return (
    <div className="App">
      <Sidebar setPage={setPage} />

      <section className="city-section">
        <div className="top-buttons">
          <button
            className="option-button"
            onClick={() => {
              setAdmin(null);
              setPage("landing");
            }}
          >
            Logout
          </button>
        </div>

        <h2>Pending Hidden Gem Requests</h2>

        {pendingRequests.length === 0 && <p>No pending requests.</p>}

        {pendingRequests.map((req, i) => (
          <div key={i} className="hidden-gem">
            <h4>{req.name}</h4>
            <p>{req.description}</p>
            <p>
              <strong>City:</strong> {req.city}
            </p>
            <p>
              <strong>Location:</strong> {req.location}
            </p>

            <div className="gem-images">
              {req.images.map((file, idx) => (
                <img
                  key={idx}
                  src={URL.createObjectURL(file)}
                  alt={req.name}
                  className="gem-thumb"
                />
              ))}
            </div>

            <button
              className="option-button"
              onClick={() => handleApproveRequest(i)}
            >
              Approve
            </button>

            <button
              className="option-button delete-btn"
              onClick={() => handleDeleteRequest(i)}
            >
              Delete
            </button>

            <button
              className="option-button"
              onClick={() => handleEditRequest(i)}
            >
              Edit
            </button>
          </div>
        ))}
      </section>
    </div>
  );
}
