import React from "react";
import './HomePage.css';
import backImage from '../assets/backg.jpg';
import arkan from '../assets/arkan.jpeg';
import A from '../assets/5A.jpeg';

export default function HomePage({ setPage }) {
  return (
    <div className="home-container">
      
      {/* Blurred background */}
      <div
        className="home-background"
        style={{ backgroundImage: `url(${backImage})` }}
      ></div>
      <div className="home-overlay"></div>

      {/* Title always on top */}
      <h1 className="home-title-top">City Secrets</h1>

      {/* Main content split */}
      <div className="home-content">
        <div className="home-split">

          {/* Left side: About / Features / Benefits */}
          <div className="home-left">
            <h2>About City Secrets</h2>
            <p>
              Discover and share the hidden gems of your city. Explore unique spots, uncover treasures, and connect with fellow adventurers!
            </p>

            <h2>Features</h2>
            <ul>
              <li>Find hidden treasures and unique places.</li>
              <li>Submit your favorite spots to share with others.</li>
              <li>Read reviews and tips from other explorers.</li>
            </ul>

            <h2>Benefits of Sharing Your Hidden Gem</h2>
            <p>
              Gain recognition, help others discover amazing places, and join a community of city explorers.
            </p>

            <button className="home-button" onClick={() => setPage("landing")}>
              Let's Start
            </button>
          </div>

          {/* Right side: Hottest Places */}
          <div className="home-right">
            <h2>Hottest Places</h2>
            <div className="image-box">
              <img src={arkan} alt="Arkan" />
              <p className="image-caption">Arkan</p>
            </div>
            <div className="image-box">
              <img src={A} alt="5A" />
              <p className="image-caption">5A</p>
            </div>

            <div className="reviews-section">
              <h3>1000+ reviews from users</h3>
              <p className="review-quote">"My new favorite place, it’s amazing!!"</p>
            </div>
          </div>

        </div>
      </div>
    </div>
  );
}
