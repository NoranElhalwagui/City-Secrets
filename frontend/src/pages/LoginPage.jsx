// pages/LoginPage.jsx
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Telescope } from "lucide-react";
import "./LoginPage.css";

export default function LoginPage() {
  const navigate = useNavigate();

  // UI state
  const [mode, setMode] = useState("login"); // login / register / forgot / reset
  const [loggedIn, setLoggedIn] = useState(false);

  // Form state
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [resetEmail, setResetEmail] = useState("");

  const [userName, setUserName] = useState("");

  // Simulate login/register
  const handleLogin = (e) => {
    e.preventDefault();
    setLoggedIn(true);
    setUserName(email.split("@")[0]);
  };

  const handleRegister = (e) => {
    e.preventDefault();
    setLoggedIn(true);
    setUserName(name);
  };

  const handleReset = (e) => {
    e.preventDefault();
    alert(`Password reset link sent to ${resetEmail}`);
    setMode("login");
  };

  const startExplore = () => navigate("/"); // back to homepage

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
            {mode === "login" && (
              <>
                <h2 className="encourage-line">Sign in and start your hidden journey</h2>
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
                <div className="toggle-links">
                  <span onClick={() => setMode("register")}>Don't have an account? Sign Up</span>
                  <span onClick={() => setMode("forgot")}>Forgot password?</span>
                </div>
              </>
            )}

            {mode === "register" && (
              <>
                <h2 className="encourage-line">Join and start a journey full of new places</h2>
                <form onSubmit={handleRegister}>
                  <input
                    type="text"
                    placeholder="Name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                    required
                  />
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
                  <button type="submit">Register</button>
                </form>
                <div className="toggle-links">
                  <span onClick={() => setMode("login")}>Already have an account? Sign in</span>
                </div>
              </>
            )}

            {mode === "forgot" && (
              <>
                <h2 className="encourage-line">Forgot your password?</h2>
                <form onSubmit={() => setMode("reset")}>
                  <input
                    type="email"
                    placeholder="Enter your email"
                    value={resetEmail}
                    onChange={(e) => setResetEmail(e.target.value)}
                    required
                  />
                  <button type="submit">Send Reset Link</button>
                </form>
                <div className="toggle-links">
                  <span onClick={() => setMode("login")}>Back to Login</span>
                </div>
              </>
            )}

            {mode === "reset" && (
              <>
                <h2 className="encourage-line">Reset your password</h2>
                <form onSubmit={handleReset}>
                  <input
                    type="password"
                    placeholder="New password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    required
                  />
                  <button type="submit">Reset Password</button>
                </form>
                <div className="toggle-links">
                  <span onClick={() => setMode("login")}>Back to Login</span>
                </div>
              </>
            )}
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
