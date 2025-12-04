import React, { useState } from "react";
import "./Sidebar.css";
import { FaBars, FaSignInAlt, FaUserPlus, FaInfoCircle } from "react-icons/fa";

export default function Sidebar({ setPage }) {
  const [isOpen, setIsOpen] = useState(false);

  const toggleDropdown = () => setIsOpen(!isOpen);
  const closeDropdown = () => setIsOpen(false);

  return (
    <div className="dropdown-container">
      
      {/* Hamburger Icon */}
      <button className="dropdown-toggle" onClick={toggleDropdown}>
        <FaBars size={22} />
      </button>

      {/* Dropdown Menu */}
      <div className={`dropdown-menu ${isOpen ? "show" : ""}`}>

        <button className="dropdown-item" onClick={() => { setPage("landing"); closeDropdown(); }}>
          <FaSignInAlt />
          Enter App
        </button>

        <button className="dropdown-item" onClick={() => { alert("About Page"); closeDropdown(); }}>
          <FaInfoCircle />
          About
        </button>

        <button className="dropdown-item" onClick={() => { alert("Other page"); closeDropdown(); }}>
          <FaUserPlus />
          More
        </button>

      </div>
    </div>
  );
}
