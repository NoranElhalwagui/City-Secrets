import React, { useEffect, useState } from "react";
import "./HomePage.css";
import backImage from "../assets/backg.jpg";
import Sidebar from "../components/Sidebar";
import axios from "axios";

export default function HomePage({ setPage }) {
  const [fade, setFade] = useState(false);
  const [registerData, setRegisterData] = useState({
    username: "",
    email: "",
    password: "",
    confirmPassword: ""
  });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ text: "", type: "" });
  const [user, setUser] = useState(null);

  useEffect(() => {
    setTimeout(() => setFade(true), 50);
    // Check if user is already logged in
    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      setUser(JSON.parse(storedUser));
    }
  }, []);

  const handleRegisterChange = (e) => {
    const { name, value } = e.target;
    setRegisterData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleRegisterSubmit = async (e) => {
    e.preventDefault();
    
    if (registerData.password !== registerData.confirmPassword) {
      setMessage({ text: "Passwords don't match!", type: "error" });
      return;
    }

    setLoading(true);
    setMessage({ text: "", type: "" });

    try {
      const response = await axios.post('http://localhost:5000/api/auth/register', {
        username: registerData.username,
        email: registerData.email,
        password: registerData.password
      });

      setMessage({ 
        text: "Registration successful! You can now login.", 
        type: "success" 
      });
      
      setRegisterData({
        username: "",
        email: "",
        password: "",
        confirmPassword: ""
      });

    } catch (error) {
      setMessage({ 
        text: error.response?.data?.message || "Registration failed. Please try again.", 
        type: "error" 
      });
    } finally {
      setLoading(false);
    }
  };

  const handleLoginClick = () => {
    setPage("login"); // You need to create a LoginPage.js
  };

  const handleQuickLogin = async (email, password) => {
    try {
      setLoading(true);
      const response = await axios.post('http://localhost:5000/api/auth/login', {
        email,
        password
      });
      
      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      localStorage.setItem('user', JSON.stringify(response.data.user));
      
      setUser(response.data.user);
      setMessage({ 
        text: `Welcome back, ${response.data.user.username}!`, 
        type: "success" 
      });
      
      // Redirect to feed after login
      setTimeout(() => setPage("feed"), 1000);
    } catch (error) {
      setMessage({ 
        text: "Login failed. Please check credentials.", 
        type: "error" 
      });
    } finally {
      setLoading(false);
    }
  };

  const handleLogout = () => {
    localStorage.clear();
    setUser(null);
    setMessage({ text: "Logged out successfully", type: "success" });
  };

  return (
    <div className={`home-container fade-page ${fade ? "active" : ""}`}>
      <Sidebar setPage={setPage} />
      
      <div 
        className="home-background"
        style={{ backgroundImage: `url(${backImage})` }}
      ></div>
      <div className="home-overlay"></div>

      <h1 className="home-title-top">City Secrets</h1>

      <div className="home-content">
        <div className="home-split">
          {/* LEFT SIDE - 70% */}
          <div className="home-left">
            <h2>Discover & Share Hidden Gems</h2>
            <p>
              City Secrets is your community for discovering amazing hidden 
              places. From secret cafes to underground art spots, find what 
              guidebooks miss and share your own discoveries!
            </p>

            <h3>🔥 Popular Features:</h3>
            <ul>
              <li><strong>Explore Feed:</strong> See posts from other explorers (login required to interact)</li>
              <li><strong>Interactive Map:</strong> Find hidden gems near you</li>
              <li><strong>Submit Places:</strong> Share your secret spots with the community</li>
              <li><strong>Like & Comment:</strong> Engage with other explorers' discoveries</li>
              <li><strong>Save Favorites:</strong> Build your personal collection of amazing places</li>
            </ul>

            <h3>Ready to Explore?</h3>
            <div className="navigation-buttons">
              <button className="home-button" onClick={() => setPage("explore")}>
                Browse All Places
              </button>
              <button 
                className="home-button secondary" 
                onClick={() => user ? setPage("feed") : handleLoginClick()}
              >
                {user ? "Go to Feed" : "View Community Feed"}
              </button>
              {user && (
                <button className="home-button accent" onClick={() => setPage("hidden-gem")}>
                  Submit Your Place
                </button>
              )}
            </div>
          </div>

          {/* RIGHT SIDE - 30% */}
          <div className="home-right">
            <div className="registration-box">
              {user ? (
                <div className="welcome-section">
                  <h2>Welcome back, {user.username}! 👋</h2>
                  <p>You're all set to explore and share.</p>
                  <div className="user-actions">
                    <button className="register-btn" onClick={() => setPage("feed")}>
                      Go to Community Feed
                    </button>
                    <button className="home-button" onClick={() => setPage("hidden-gem")}>
                      Submit a New Place
                    </button>
                    <button className="text-btn" onClick={handleLogout}>
                      Logout
                    </button>
                  </div>
                </div>
              ) : (
                <>
                  <h2>Join Our Community</h2>
                  
                  {message.text && (
                    <div className={`message ${message.type}`}>
                      {message.text}
                    </div>
                  )}

                  <form onSubmit={handleRegisterSubmit}>
                    <div className="form-group">
                      <input
                        type="text"
                        name="username"
                        placeholder="Username"
                        value={registerData.username}
                        onChange={handleRegisterChange}
                        required
                      />
                    </div>

                    <div className="form-group">
                      <input
                        type="email"
                        name="email"
                        placeholder="Email"
                        value={registerData.email}
                        onChange={handleRegisterChange}
                        required
                      />
                    </div>

                    <div className="form-group">
                      <input
                        type="password"
                        name="password"
                        placeholder="Password"
                        value={registerData.password}
                        onChange={handleRegisterChange}
                        required
                      />
                    </div>

                    <div className="form-group">
                      <input
                        type="password"
                        name="confirmPassword"
                        placeholder="Confirm Password"
                        value={registerData.confirmPassword}
                        onChange={handleRegisterChange}
                        required
                      />
                    </div>

                    <button 
                      type="submit" 
                      className="register-btn"
                      disabled={loading}
                    >
                      {loading ? "Registering..." : "Create Account"}
                    </button>
                  </form>

                  <div className="login-section">
                    <p>Already have an account?</p>
                    <button 
                      className="home-button"
                      onClick={handleLoginClick}
                    >
                      Login Here
                    </button>
                    
                    <div className="guest-notice">
                      <p><strong>Guest Access:</strong> You can browse places but need to login to:</p>
                      <ul>
                        <li>View the community feed</li>
                        <li>Like and comment on posts</li>
                        <li>Submit your own places</li>
                        <li>Save favorites</li>
                      </ul>
                    </div>
                  </div>
                </>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}