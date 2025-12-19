import React from "react";
import "../App.css";
import Sidebar from "../components/Sidebar";
import cityBack from "../assets/backg.jpg";

export default function Explorer({
  setPage,
  activeCity,
  handleCityClick,
  handleCategoryClick,
  cairoSectionRef,
  gizaSectionRef,
  cairoRefs,
  gizaRefs,
  cairoRestaurants,
  gizaRestaurants,
  hiddenGems,
}) {
  return (
    <div className="App">
      <Sidebar setPage={setPage} />

      {/* Hero Section */}
      <section className="hero">
        <div
          className="background-blur"
          style={{ backgroundImage: `url(${cityBack})` }}
        ></div>

        <div className="background-overlay"></div>

        <header className="hero-header">
          <div className="top-buttons">
            <button className="option-button" onClick={() => setPage("landing")}>
              Go Back
            </button>
          </div>

          <h1>Explore Hidden Gems</h1>
          <p>Discover hidden treasures in Cairo and Giza!</p>

          <div className="button-container">
            <button className="city-button" onClick={() => handleCityClick("cairo")}>
              <img src={require("../assets/cairo.jpg")} alt="Cairo" />
              <span>Explore Cairo</span>
            </button>

            <button className="city-button" onClick={() => handleCityClick("giza")}>
              <img src={require("../assets/giza.jpg")} alt="Giza" />
              <span>Explore Giza</span>
            </button>
          </div>
        </header>
      </section>

      {/* Cairo Section */}
      {(!activeCity || activeCity === "cairo") && (
        <section className="city-section" id="cairo" ref={cairoSectionRef}>
          <h2>Cairo Secrets</h2>
          <p>From historic streets to vibrant markets, Cairo has endless treasures to explore.</p>

          <div className="options-container">
            {["Restaurants", "Clothes", "Parks", "HiddenGems"].map((cat) => (
              <button
                key={cat}
                className="option-button"
                onClick={() => handleCategoryClick("cairo", cat)}
              >
                {cat === "HiddenGems" ? "Hidden Gems" : cat}
              </button>
            ))}
          </div>

          <div className="sub-section" ref={cairoRefs.Restaurants}>
            <h3>Restaurants in Cairo</h3>
            {cairoRestaurants.map((res, i) => (
              <p key={i}>
                <strong>{res.name}:</strong> {res.description}
              </p>
            ))}
          </div>

          <div className="sub-section" ref={cairoRefs.HiddenGems}>
            <h3>Hidden Gems in Cairo</h3>

            {hiddenGems
              .filter((g) => g.city === "Cairo")
              .map((gem, i) => (
                <div key={i} className="hidden-gem">
                  <h4>{gem.name}</h4>
                  <p>{gem.description}</p>
                  <p>
                    <strong>Location:</strong> {gem.location}
                  </p>

                  <div className="gem-images">
                    {gem.images.map((file, idx) => (
                      <img
                        key={idx}
                        src={URL.createObjectURL(file)}
                        alt={gem.name}
                        className="gem-thumb"
                      />
                    ))}
                  </div>
                </div>
              ))}
          </div>
        </section>
      )}

      {/* Giza Section */}
      {(!activeCity || activeCity === "giza") && (
        <section className="city-section" id="giza" ref={gizaSectionRef}>
          <h2>Giza Secrets</h2>
          <p>Discover the pyramids, hidden streets, and local spots in Giza.</p>

          <div className="options-container">
            {["Restaurants", "Clothes", "Parks", "HiddenGems"].map((cat) => (
              <button
                key={cat}
                className="option-button"
                onClick={() => handleCategoryClick("giza", cat)}
              >
                {cat === "HiddenGems" ? "Hidden Gems" : cat}
              </button>
            ))}
          </div>

          <div className="sub-section" ref={gizaRefs.Restaurants}>
            <h3>Restaurants in Giza</h3>
            {gizaRestaurants.map((res, i) => (
              <p key={i}>
                <strong>{res.name}:</strong> {res.description}
              </p>
            ))}
          </div>

          <div className="sub-section" ref={gizaRefs.HiddenGems}>
            <h3>Hidden Gems in Giza</h3>

            {hiddenGems
              .filter((g) => g.city === "Giza")
              .map((gem, i) => (
                <div key={i} className="hidden-gem">
                  <h4>{gem.name}</h4>
                  <p>{gem.description}</p>
                  <p>
                    <strong>Location:</strong> {gem.location}
                  </p>

                  <div className="gem-images">
                    {gem.images.map((file, idx) => (
                      <img
                        key={idx}
                        src={URL.createObjectURL(file)}
                        alt={gem.name}
                        className="gem-thumb"
                      />
                    ))}
                  </div>
                </div>
              ))}
          </div>
        </section>
      )}
    </div>
  );
}
