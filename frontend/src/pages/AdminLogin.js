import React from "react";
import "../App.css";
import Sidebar from "../components/Sidebar";

export default function AdminLogin({ handleAdminLogin, setPage }) {
  return (
    <div className="App">
      <Sidebar setPage={setPage} />

      <section className="city-section">
        <h2>Admin Login</h2>

        <form onSubmit={handleAdminLogin}>
          <input name="id" placeholder="Admin ID" required />
          <input name="pass" placeholder="Password" type="password" required />

          <button type="submit" className="option-button">
            Login
          </button>
        </form>
      </section>
    </div>
  );
}
