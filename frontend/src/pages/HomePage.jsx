// pages/HomePage.jsx
import React, { useState } from "react";
import { Link } from "react-router-dom";
import { Telescope, Star } from "lucide-react";
import "./HomePage.css";

export default function HomePage() {
  const categories = ["Food", "Parks", "Museums", "Shopping"];
  const locations = ["Cairo", "Giza"];
  const trendingPlaces = ["Hidden Gem 1", "Hidden Gem 2", "Hidden Gem 3", "Hidden Gem 4"];

  const [selectedCategory, setSelectedCategory] = useState("");
  const [selectedLocation, setSelectedLocation] = useState(locations[0]);

  return (
    <div className="homepage-container">
      <canvas id="particle-bg" className="particle-bg"></canvas>

      <div className="homepage-content">
        {/* Left Side */}
        <div className="left-side">
          <div className="logo">
            <Telescope size={40} className="logo-icon" />
            <span className="logo-text">City Secrets</span>
          </div>

          <h1 className="title">City Secrets</h1>
          <p className="description">
            Explore your city’s hidden gems and trending spots with personalized recommendations.
          </p>

          {/* Category Search */}
          <div className="search-section">
            <div className="categories">
              {categories.map((cat) => (
                <button
                  key={cat}
                  className={`category-btn ${selectedCategory === cat ? "active" : ""}`}
                  onClick={() => setSelectedCategory(cat)}
                >
                  {cat}
                </button>
              ))}
            </div>
            <select
              value={selectedLocation}
              onChange={(e) => setSelectedLocation(e.target.value)}
            >
              {locations.map((loc) => (
                <option key={loc} value={loc}>{loc}</option>
              ))}
            </select>
            <button className="search-btn">Search</button>
          </div>

          {/* Bottom Login */}
          <div className="login-section">
            <div className="line-text">Join our community and explore new hidden gems</div>
            <Link to="/login">
              <button className="login-btn">
                Login
                <span className="arrow">→</span>
              </button>
            </Link>
          </div>
        </div>

        {/* Right Side - Trending */}
        <div className="right-side">
          <h2>Most Trending Places</h2>
          <div className="trending-scroll">
            {trendingPlaces.map((place, idx) => (
              <div key={idx} className="trending-item">
                {place} <Star size={16} className="star-icon" />
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Particle animation */}
      <script>{`
        const canvas = document.getElementById("particle-bg");
        const ctx = canvas.getContext("2d");
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;

        const particles = [];
        for (let i = 0; i < 100; i++) {
          particles.push({
            x: Math.random() * canvas.width,
            y: Math.random() * canvas.height,
            radius: Math.random() * 2 + 1,
            dx: (Math.random() - 0.5) * 0.5,
            dy: (Math.random() - 0.5) * 0.5
          });
        }

        function animate() {
          ctx.clearRect(0, 0, canvas.width, canvas.height);
          particles.forEach(p => {
            p.x += p.dx;
            p.y += p.dy;
            if (p.x < 0 || p.x > canvas.width) p.dx *= -1;
            if (p.y < 0 || p.y > canvas.height) p.dy *= -1;

            ctx.beginPath();
            ctx.arc(p.x, p.y, p.radius, 0, Math.PI * 2);
            ctx.fillStyle = "rgba(255, 215, 0, 0.7)";
            ctx.fill();
          });
          requestAnimationFrame(animate);
        }
        animate();
      `}</script>
    </div>
  );
}
