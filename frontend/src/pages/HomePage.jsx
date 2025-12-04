import React from "react";
import './HomePage.css';
import backImage from '../assets/backg.jpg';
import Sidebar from "../components/Sidebar";
import arkan from '../assets/arkan.jpeg';
import A from '../assets/5A.jpeg';



export default function HomePage({ setPage }) {
  return (
    
    <div className="home-container">
      <Sidebar setPage={setPage} />

      <div className="home-hero">
        <div
          className="home-background"
          style={{ backgroundImage: `url(${backImage})` }}
        ></div>

        <div className="home-overlay"></div>

        <div className="home-content">
          <h1 className="home-title">City Secrets</h1>
          <p className="home-subtitle">Your journey starts here.</p>

          <button className="home-button" onClick={() => setPage("landing")}>
            let's start  
          </button>

          {/* NEW SECTION */}
          <h2 className="discover-title">
            Discover the hottest hangout spots in Egypt
          </h2>

          <div className="image-box">
            <img src={arkan} alt="Place 1" />
            <p className="image-caption">Arkan</p>
          </div>

          <div className="image-box">
            <img src={A} alt="Place 2" />
            <p className="image-caption">5A</p>
          </div>

          <div className="reviews-section">
            <h3 className="reviews-title">
              1000+ reviews from users about places they visited
            </h3>
            <p className="review-quote">
              "My new favorite place, it’s amazingg!!"
            </p>
          </div>
        </div>

        <div className="home-sections">
          <section className="home-section">
            <h2>About City Secrets</h2>
            <p>Your text here…</p>
          </section>

          <section className="home-section">
            <h2>Features</h2>
            <p>Your text here…</p>
          </section>

          <section className="home-section">
            <h2>Why Choose Us?</h2>
            <p>Your text here…</p>
          </section>
        </div>
      </div>
    </div>
  );
}
