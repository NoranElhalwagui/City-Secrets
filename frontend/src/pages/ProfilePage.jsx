import React, { useState } from "react";
import "./ProfilePage.css";

export default function ProfilePage() {
  const [editMode, setEditMode] = useState(false);

  return (
    <div className="profile-container">
      <h1 className="page-title">My Profile</h1>

      {/* User Info */}
      <div className="card">
        <h2>User Information</h2>
        {editMode ? (
          <>
            <input placeholder="Username" />
            <input placeholder="Email" />
            <button onClick={() => setEditMode(false)}>Save</button>
          </>
        ) : (
          <>
            <p>Username: JohnDoe</p>
            <p>Email: john@email.com</p>
            <button onClick={() => setEditMode(true)}>Edit</button>
          </>
        )}
      </div>

      {/* Favorites */}
      <div className="card">
        <h2>Favorites</h2>
        <ul>
          <li>Hidden Cafe — “Great vibe”</li>
          <li>Secret Park — “Quiet & peaceful”</li>
        </ul>
      </div>

      {/* Reviews */}
      <div className="card">
        <h2>My Reviews</h2>
        <p>⭐️⭐️⭐️⭐️ Amazing place!</p>
      </div>

      {/* Notifications */}
      <div className="card">
        <h2>Notifications</h2>
        <p>New place added near you</p>
        <button>Mark as Read</button>
      </div>
    </div>
  );
}
