import React, { useEffect, useState } from "react";
import "./HomePage.css";
import backImage from "../assets/backg.jpg";
// Sidebar removed from HomePage (rendered by App.js when needed)
import axios from "axios";
import cairoImage from "../assets/cairo.jpg";
import gizaImage from "../assets/giza.jpg";

export default function HomePage({ setPage, onLoginSuccess }) {
  const [fade, setFade] = useState(false);
  const [registerData, setRegisterData] = useState({
    username: "",
    email: "",
    password: "",
    confirmPassword: ""
  });
  const [showChoices, setShowChoices] = useState(false);
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
      // Register
      await axios.post('http://localhost:5000/api/auth/register', {
        username: registerData.username,
        email: registerData.email,
        password: registerData.password
      });

      // Auto-login after register so user can immediately continue
      const loginRes = await axios.post('http://localhost:5000/api/auth/login', {
        email: registerData.email,
        password: registerData.password
      });

      localStorage.setItem('accessToken', loginRes.data.accessToken);
      localStorage.setItem('refreshToken', loginRes.data.refreshToken);
      localStorage.setItem('user', JSON.stringify(loginRes.data.user));

      setUser(loginRes.data.user);
      setMessage({ text: `Welcome, ${loginRes.data.user.username}!`, type: 'success' });
      setShowChoices(true);

      // notify parent about login success (App.js)
      if (onLoginSuccess) onLoginSuccess(loginRes.data.user);

      setRegisterData({ username: "", email: "", password: "", confirmPassword: "" });

    } catch (error) {
      setMessage({ text: error.response?.data?.message || "Registration failed. Please try again.", type: "error" });
    } finally {
      setLoading(false);
    }
  };

  const handleLoginClick = () => {
    setPage("login"); // You need to create a LoginPage.js
  };

  const handleAdminLogin = () => {
    const adminUser = { username: 'Admin', email: 'admin@gmail.com', role: 'Admin' };
    localStorage.setItem('accessToken', 'local-admin-token');
    localStorage.setItem('user', JSON.stringify(adminUser));
    setUser(adminUser);
    setMessage({ text: 'Logged in as Admin', type: 'success' });
    if (onLoginSuccess) onLoginSuccess(adminUser);
    setTimeout(() => setPage('admin'), 700);
  };

  const handleLogout = () => {
    localStorage.clear();
    setUser(null);
    setMessage({ text: "Logged out successfully", type: "success" });
  };

  return (
    <div className={`home-container fade-page ${fade ? "active" : ""}`}>
      
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

                  {/* Admin quick-login (local) */}
                  <div style={{ marginTop: 8 }}>
                    <button type="button" className="admin-login-btn" onClick={handleAdminLogin}>
                      Admin Quick Login
                    </button>
                  </div>

                  {/* After successful registration show choices */}
                  {showChoices && (
                    <div className="choice-grid" style={{ marginTop: 20 }}>
                      <h3 style={{ textAlign: 'center', color: '#ffdd59' }}>Choose where to go next</h3>
                      <div className="choice-cards" style={{ display: 'flex', gap: 10, marginTop: 10, flexWrap: 'wrap' }}>
                        <div className="choice-card" style={{ cursor: 'pointer', width: 120 }} onClick={() => { localStorage.setItem('exploreLocation', 'cairo'); setPage('explore'); }}>
                          <img src={cairoImage} alt="Cairo" style={{ width: '100%', borderRadius: 8 }} />
                          <div style={{ textAlign: 'center', marginTop: 6 }}>Explore Cairo</div>
                        </div>

                        <div className="choice-card" style={{ cursor: 'pointer', width: 120 }} onClick={() => { localStorage.setItem('exploreLocation', 'giza'); setPage('explore'); }}>
                          <img src={gizaImage} alt="Giza" style={{ width: '100%', borderRadius: 8 }} />
                          <div style={{ textAlign: 'center', marginTop: 6 }}>Explore Giza</div>
                        </div>

                        <div className="choice-card" style={{ cursor: 'pointer', width: 160 }} onClick={() => setPage('hidden-gem')}>
                          <div style={{ background: 'rgba(255,255,255,0.04)', padding: 12, borderRadius: 8, textAlign: 'center' }}>
                            <strong>Hidden Gem Owner?</strong>
                            <div style={{ marginTop: 8 }}>Submit your place</div>
                          </div>
                        </div>

                        <div className="choice-card" style={{ cursor: 'pointer', width: 120 }} onClick={() => { if (user) setPage('feed'); else setMessage({ text: 'Please login to view the feed', type: 'error' }); }}>
                          <div style={{ background: 'rgba(255,255,255,0.04)', padding: 12, borderRadius: 8, textAlign: 'center' }}>Check the Feed</div>
                        </div>
                      </div>
                    </div>
                  )}

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