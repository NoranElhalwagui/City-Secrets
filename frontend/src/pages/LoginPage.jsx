// pages/LoginPage.jsx
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Telescope } from "lucide-react";
import "./LoginPage.css";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loggedIn, setLoggedIn] = useState(false);
  const [userName, setUserName] = useState("");
  const navigate = useNavigate();

  const handleLogin = (e) => {
    e.preventDefault();
    setLoggedIn(true);
    const name = email.split("@")[0];
    setUserName(name);
  };

  const startExplore = () => {
    navigate("/"); // go back to homepage
  };

  return (
    <div className="loginpage-container">
      <canvas id="particle-bg-login" className="particle-bg-login"></canvas>

      <div className="loginpage-content">
        <div className="logo">
          <Telescope size={40} className="logo-icon" />
          <span className="logo-text">City Secrets</span>
        </div>

        {!loggedIn ? (
          <div className="login-box">
            <h2 className="encourage-line">Join and start a journey full of new places</h2>
            <form onSubmit={handleLogin}>
              <input
                type="email"
                placeholder="Email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
              />
              <input
                type="password"
                placeholder="Password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
              <button type="submit">Login</button>
            </form>
          </div>
        ) : (
          <div className="welcome-box">
            <h2>Welcome {userName}!</h2>
            <p>We are delighted to have you in our secret family.</p>
            <button className="start-btn" onClick={startExplore}>Start to Explore</button>
          </div>
        )}
      </div>

      <script>{`
        const canvas = document.getElementById("particle-bg-login");
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
