import React, { useEffect, useState } from "react";
import "../App.css";
import Sidebar from "../components/Sidebar";
import cityBack from '../assets/backg.jpg';



export default function LandingPage({ setPage }) {
return (
    <div className="App">
        <Sidebar setPage={setPage} />
      <section className="hero">
        <div className="background-blur" style={{ backgroundImage: `url(${cityBack})` }}></div>
        <div className="background-overlay"></div>
        <header className="hero-header">
          <p>Are you an Adventurer, Hidden Gem Owner, or Admin?</p>
          <div className="button-container">
            <button className="option-button" onClick={() => setPage('explorer')}>Adventurer</button>
            <button className="option-button" onClick={() => setPage('ownerForm')}>Hidden Gem Owner</button>
            <button className="option-button" onClick={() => setPage('adminLogin')}>Admin Login</button>
          </div>
        </header>
      </section>
    </div>
      );
}