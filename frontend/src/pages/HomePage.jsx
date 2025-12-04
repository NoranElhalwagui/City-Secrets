import React from "react";
import './HomePage.css';
import backImage from '../assets/backg.jpg';
import Sidebar from "../components/Sidebar";



export default function HomePage({ setPage }) {
  return (
    
    <div className="home-container">
    <Sidebar setPage={setPage} />

    <div className="home-hero">
    <div className="home-background" style={{ backgroundImage: `url(${backImage})` }} ></div>

      <div className="home-overlay"></div>

      <div className="home-content">
        <h1 className="home-title">Welcome to City Secrets</h1>
        <p className="home-subtitle">Your journey starts here.</p>

        <button
          className="home-button"
          onClick={() => setPage("landing")}
        >
          Enter App
        </button>
      </div>
    </div>
    </div>
  );
}
