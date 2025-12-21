import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Telescope } from "lucide-react";
import "./LoginPage.css";

export default function LoginPage() {
  const navigate = useNavigate();

  const [mode, setMode] = useState("login"); // login / register / forgot / reset
  const [loggedIn, setLoggedIn] = useState(false);

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [resetEmail, setResetEmail] = useState("");

  const [userName, setUserName] = useState("");
  const [isReturning, setIsReturning] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);

  // Hardcoded admin credentials
  const adminUser = {
    email: "Admin@gmail.com",
    password: "121212",
    name: "Admin",
  };

  // Local storage users (simulate backend)
  const getUsers = () => {
    const data = localStorage.getItem("users");
    return data ? JSON.parse(data) : [];
  };

  const saveUser = (user) => {
    const users = getUsers();
    users.push(user);
    localStorage.setItem("users", JSON.stringify(users));
  };

  const findUserByEmail = (email) => {
    const users = getUsers();
    return users.find((u) => u.email === email);
  };

  // Login
  const handleLogin = (e) => {
    e.preventDefault();

    // Check if admin
    if (email === adminUser.email && password === adminUser.password) {
      setUserName(adminUser.name);
      setIsAdmin(true);
      setLoggedIn(true);
      return;
    }

    // Normal user
    const existingUser = findUserByEmail(email);
    if (existingUser && existingUser.password === password) {
      setUserName(existingUser.name);
      setIsReturning(true);
      setLoggedIn(true);
    } else {
      alert("Invalid email or password!");
    }
  };

  // Register
  const handleRegister = (e) => {
    e.preventDefault();
    if (findUserByEmail(email) || (email === adminUser.email)) {
      alert("This email is already registered. Please log in.");
      setMode("login");
      return;
    }
    saveUser({ name, email, password });
    setUserName(name);
    setIsReturning(false);
    setLoggedIn(true);
  };

  // Reset
  const handleReset = (e) => {
    e.preventDefault();
    alert(`Password reset link sent to ${resetEmail}`);
    setMode("login");
  };

  const startExplore = () => navigate("/");

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
            <h2>
              {isAdmin
                ? "Welcome Admin! Access your Admin Dashboard below"
                : isReturning
                ? `Welcome back ${userName}! Continue your exploring journey`
                : `Welcome ${userName}! We are delighted to have you in our secret family`}
            </h2>

            <button className="start-btn" onClick={startExplore}>
              {isAdmin ? "Go to Admin Dashboard" : "Start to Explore"}
            </button>

            {isAdmin && (
              <div className="admin-dashboard">
                <h3>Admin Dashboard</h3>
                <p>Here you can manage users, posts, and hidden gems.</p>
                {/* Add more admin features here */}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
