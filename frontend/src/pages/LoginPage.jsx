import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Telescope } from "lucide-react";
import { login, register, forgotPassword } from "../services/authService";
import "./LoginPage.css";

export default function LoginPage() {
  const navigate = useNavigate();

  const [mode, setMode] = useState("login");
  const [loggedIn, setLoggedIn] = useState(false);

  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [resetEmail, setResetEmail] = useState("");

  const [userName, setUserName] = useState("");
  const [isAdmin, setIsAdmin] = useState(false);

  const [loading, setLoading] = useState(false);

  // ================= LOGIN =================
  const handleLogin = async (e) => {
    e.preventDefault();
    if (loading) return;

    setLoading(true);

    try {
      const result = await login(email, password);

      if (!result.success) {
        alert(result.message || "Login failed");
        return;
      }

      // ✅ Save tokens
      localStorage.setItem("accessToken", result.accessToken);
      localStorage.setItem("refreshToken", result.refreshToken);
      localStorage.setItem("user", JSON.stringify(result.user));

      setUserName(result.user?.username || "User");

      /**
       * ✅ IMPORTANT CHANGE
       * We DO NOT guess admin from frontend flags
       * We let backend decide access
       */
      navigate("/admin");
      setLoggedIn(true);
    } catch (err) {
      alert(err?.message || "An unexpected error occurred during login");
    } finally {
      setLoading(false);
    }
  };

  // ================= REGISTER =================
  const handleRegister = async (e) => {
    e.preventDefault();
    if (loading) return;

    setLoading(true);

    try {
      const result = await register({
        username: name,
        fullName: name,
        email,
        password,
      });

      if (!result.success) {
        alert(result.message || "Registration failed");
        setMode("login");
        return;
      }

      localStorage.setItem("accessToken", result.accessToken);
      localStorage.setItem("refreshToken", result.refreshToken);
      localStorage.setItem("user", JSON.stringify(result.user));

      setUserName(result.user?.username || "User");
      setLoggedIn(true);

      navigate("/explore");
    } catch (err) {
      alert(err?.message || "Registration failed");
    } finally {
      setLoading(false);
    }
  };

  // ================= FORGOT PASSWORD =================
  const handleReset = async (e) => {
    e.preventDefault();
    if (loading) return;

    setLoading(true);

    try {
      await forgotPassword(resetEmail);
      alert("If this email exists, a reset link has been sent.");
      setMode("login");
    } catch (err) {
      alert(err?.message || "Something went wrong");
    } finally {
      setLoading(false);
    }
  };

  const goToExplore = () => navigate("/explore");
  const goToAdminDashboard = () => navigate("/admin");

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
                <h2 className="encourage-line">
                  Sign in and start your hidden journey
                </h2>

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
                  <button type="submit" disabled={loading}>
                    Login
                  </button>
                </form>

                <div className="toggle-links">
                  <span onClick={() => setMode("register")}>
                    Don't have an account? Sign Up
                  </span>
                  <span onClick={() => setMode("forgot")}>
                    Forgot password?
                  </span>
                </div>
              </>
            )}

            {mode === "register" && (
              <>
                <h2 className="encourage-line">
                  Join and start a journey full of new places
                </h2>

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
                  <button type="submit" disabled={loading}>
                    Register
                  </button>
                </form>

                <div className="toggle-links">
                  <span onClick={() => setMode("login")}>
                    Already have an account? Sign in
                  </span>
                </div>
              </>
            )}

            {mode === "forgot" && (
              <>
                <h2 className="encourage-line">Forgot your password?</h2>

                <form onSubmit={handleReset}>
                  <input
                    type="email"
                    placeholder="Enter your email"
                    value={resetEmail}
                    onChange={(e) => setResetEmail(e.target.value)}
                    required
                  />
                  <button type="submit" disabled={loading}>
                    Send Reset Link
                  </button>
                </form>

                <div className="toggle-links">
                  <span onClick={() => setMode("login")}>
                    Back to Login
                  </span>
                </div>
              </>
            )}
          </div>
        ) : (
          <div className="welcome-box">
            <h2>Welcome {userName}!</h2>

            <button className="start-btn" onClick={goToAdminDashboard}>
              Continue
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
