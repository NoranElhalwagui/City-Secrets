// pages/HomePage.jsx
import React from "react";
import { Link } from "react-router-dom";
import { Telescope } from "lucide-react";
import "./HomePage.css";

import cairo1 from "../assets/cairo.jpg";
import cairo2 from "../assets/backg.jpg";
import cairo3 from "../assets/giza.jpg";
import giza1 from "../assets/giza.jpg";
import giza2 from "../assets/backg.jpg";
import giza3 from "../assets/cairo.jpg";

export default function HomePage() {
  const cairoImages = [cairo1, cairo2, cairo3, cairo1, cairo2, cairo3];
  const gizaImages = [giza1, giza2, giza3, giza1, giza2, giza3];

  return (
    <div className="homepage-container">
      <canvas id="particle-bg" className="particle-bg"></canvas>

      <div className="homepage-content">
        {/* LEFT SIDE */}
        <div className="left-side">
          <div className="logo">
            <Telescope size={50} className="logo-icon" />
            <span className="logo-text">City Secrets</span>
          </div>

          <h1 className="title">Discover Hidden Gems</h1>
          <p className="description">
            Tired of the usual tourist spots? Dive into your city's secret corners,
            uncover local treasures, and explore places that deserve the hype.
          </p>

          <p className="tagline">
            ✨ Every street has a story. Every café has a secret. Let us show you the unseen!
          </p>

          <div className="login-section">
            <div className="line-text">
              Join our family to explore hidden spots or add your own secret place! 🌟
            </div>
            <Link to="/login">
              <button className="login-btn">
                Login <span className="arrow">→</span>
              </button>
            </Link>
            <Link to="/admin">
              <button
                className="login-btn"
                style={{
                  marginTop: "12px",
                  backgroundColor: "#b91c1c",
                  opacity: 0.85,
                }}
              >
                DEV: Admin Dashboard
              </button>
            </Link>
          </div>
        </div>

        {/* RIGHT SIDE */}
        <div className="right-side cities-side">
          {/* Cairo Box */}
          <div className="city-box">
            <h2 className="city-title">Cairo Secrets</h2>
            <div className="image-grid">
              {cairoImages.map((img, i) => (
                <img key={i} src={img} alt="Cairo secret" className="city-image" />
              ))}
            </div>
          </div>

          {/* Giza Box */}
          <div className="city-box">
            <h2 className="city-title">Giza Secrets</h2>
            <div className="image-grid">
              {gizaImages.map((img, i) => (
                <img key={i} src={img} alt="Giza secret" className="city-image" />
              ))}
            </div>
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
        for (let i = 0; i < 120; i++) {
          particles.push({
            x: Math.random() * canvas.width,
            y: Math.random() * canvas.height,
            radius: Math.random() * 2 + 1,
            dx: (Math.random() - 0.5) * 0.8,
            dy: (Math.random() - 0.5) * 0.8
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
            ctx.fillStyle = "rgba(255, 215, 0, 0.8)";
            ctx.fill();
          });
          requestAnimationFrame(animate);
        }
        animate();
      `}</script>
    </div>
  );
}
