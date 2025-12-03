import { useRef, useState } from 'react';
import cityBack from './backg.jpg';
import cairoImg from './cairo.jpg';
import gizaImg from './giza.jpg';
import './App.css';

function App() {

  const [activeCity, setActiveCity] = useState(null); 

  const cairoSectionRef = useRef(null);
  const gizaSectionRef = useRef(null);

  const cairoRefs = {
    Restaurants: useRef(null),
    Clothes: useRef(null),
    Parks: useRef(null)
  };

  const gizaRefs = {
    Restaurants: useRef(null),
    Clothes: useRef(null),
    Parks: useRef(null)
  };

  const handleCityClick = (city) => {
    setActiveCity(city);
    const ref = city === 'cairo' ? cairoSectionRef : gizaSectionRef;
    if (ref.current) {
      ref.current.scrollIntoView({ behavior: 'smooth' });
    }
  };

  const handleCategoryClick = (city, category) => {
    const ref = city === 'cairo' ? cairoRefs[category] : gizaRefs[category];
    if (ref.current) {
      ref.current.scrollIntoView({ behavior: 'smooth' });
    }
  };

  // Sample data
  const cairoRestaurants = [
    { name: "Nile View Cafe", description: "Beautiful view of the Nile with Egyptian dishes." },
    { name: "Koshary El Tahrir", description: "Famous for traditional koshary." },
  ];

  const gizaRestaurants = [
    { name: "Pyramid Lounge", description: "Dine near the pyramids with authentic food." },
    { name: "Giza Garden Cafe", description: "Cozy garden seating and coffee." },
  ];

  return (
    <div className="App">
      {/* Hero Section */}
      <section className="hero">
        <div className="background-blur" style={{ backgroundImage: `url(${cityBack})` }}></div>
        <div className="background-overlay"></div>

        <header className="hero-header">
          <h1>Welcome to the City Secrets Gem</h1>
          <p>Discover hidden gems, cool places, and unique spots in your city.</p>

          <div className="button-container">
            <button className="city-button" onClick={() => handleCityClick('cairo')}>
              <img src={cairoImg} alt="Cairo" />
              <span>Explore Cairo</span>
            </button>

            <button className="city-button" onClick={() => handleCityClick('giza')}>
              <img src={gizaImg} alt="Giza" />
              <span>Explore Giza</span>
            </button>
          </div>
        </header>
      </section>

      {/* Cairo Section */}
      {activeCity === 'cairo' && (
        <section className="city-section" id="cairo" ref={cairoSectionRef}>
          <h2>Cairo Secrets</h2>
          <p>From historic streets to vibrant markets, Cairo has endless treasures to explore.</p>

          <div className="options-container">
            {["Restaurants", "Clothes", "Parks"].map(category => (
              <button
                key={category}
                className="option-button"
                onClick={() => handleCategoryClick('cairo', category)}
              >
                {category}
              </button>
            ))}
          </div>

          <div className="sub-section" ref={cairoRefs.Restaurants}>
            <h3>Restaurants in Cairo</h3>
            {cairoRestaurants.map((res, i) => (
              <p key={i}><strong>{res.name}:</strong> {res.description}</p>
            ))}
          </div>

          <div className="sub-section" ref={cairoRefs.Clothes}>
            <h3>Clothes in Cairo</h3>
            <p>Explore local fashion stores and markets for clothes in Cairo.</p>
          </div>

          <div className="sub-section" ref={cairoRefs.Parks}>
            <h3>Parks in Cairo</h3>
            <p>Relax in the city's beautiful parks and green spaces.</p>
          </div>
        </section>
      )}

      {/* Giza Section */}
      {activeCity === 'giza' && (
        <section className="city-section" id="giza" ref={gizaSectionRef}>
          <h2>Giza Secrets</h2>
          <p>Discover the pyramids, hidden streets, and local spots in Giza.</p>

          <div className="options-container">
            {["Restaurants", "Clothes", "Parks"].map(category => (
              <button
                key={category}
                className="option-button"
                onClick={() => handleCategoryClick('giza', category)}
              >
                {category}
              </button>
            ))}
          </div>

          <div className="sub-section" ref={gizaRefs.Restaurants}>
            <h3>Restaurants in Giza</h3>
            {gizaRestaurants.map((res, i) => (
              <p key={i}><strong>{res.name}:</strong> {res.description}</p>
            ))}
          </div>

          <div className="sub-section" ref={gizaRefs.Clothes}>
            <h3>Clothes in Giza</h3>
            <p>Explore local fashion stores and markets for clothes in Giza.</p>
          </div>

          <div className="sub-section" ref={gizaRefs.Parks}>
            <h3>Parks in Giza</h3>
            <p>Relax in the city's beautiful parks and green spaces.</p>
          </div>
        </section>
      )}
    </div>
  );
}

export default App;
