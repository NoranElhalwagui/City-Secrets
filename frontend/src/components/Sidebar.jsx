import React, { useState } from "react";
import "./Sidebar.css";
import { FaBars, FaSignInAlt, FaUserPlus, FaInfoCircle } from "react-icons/fa";

export default function Sidebar({ setPage }) {
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div className="dropdown-container">
      <button className="dropdown-toggle" onClick={() => setIsOpen(!isOpen)}>
        <FaBars />
      </button>

      <div className={`dropdown-menu ${isOpen ? "show" : ""}`}>
        <button 
          className="dropdown-item" onClick={() => setPage("myHome")}
        >
        <FaSignInAlt /> Go to Home
        </button>

        <button
          className="dropdown-item" onClick={() => alert("About Page")}
        >
          <FaInfoCircle /> About
        </button>

        <button
          className="dropdown-item" onClick={() => alert("More")}
        >
          <FaUserPlus /> More
        </button>
      </div>
    </div>
  );
}
