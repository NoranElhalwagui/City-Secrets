// pages/ExplorePage.jsx
import React from "react";
import "./ExplorePage.css";

export default function ExplorePage() {
  const places = [
    { name: "Hidden Cafe", category: "Food", rating: 4.5 },
    { name: "Secret Museum", category: "Museum", rating: 4.8 },
  ];

  return (
    <div className="explore-container">
      <h1 className="page-title">Explore Places</h1>

      {/* Search */}
      <div className="search-box">
        <input placeholder="Search places..." />
        <select>
          <option>All Categories</option>
          <option>Food</option>
          <option>Parks</option>
        </select>
        <button>Search</button>
      </div>

      {/* Filters */}
      <div className="filters">
        <button>Price</button>
        <button>Rating</button>
        <button>Distance</button>
      </div>

      {/* Places */}
      <div className="places-grid">
        {places.map((p, i) => (
          <div key={i} className="place-card">
            <h3>{p.name}</h3>
            <p>{p.category}</p>
            <p>⭐ {p.rating}</p>
          </div>
        ))}
      </div>

      {/* Map */}
      <div className="map-placeholder">
        Map View Placeholder
      </div>
    </div>
  );
}
