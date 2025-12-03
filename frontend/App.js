import cityBack from './bk2.jpg';
import cairoImg from './cairo.jpg';
import gizaImg from './giza.jpg';
import './App.css';

function App() {

  const handleCityClick = (city) => {
    const section = document.getElementById(city);
    section.style.display = 'block';
    section.scrollIntoView({ behavior: 'smooth' });
  };

  return (
    <div className="App">
      {/* Hero section */}
      <section className="hero">
        <div
          className="background-blur"
          style={{ backgroundImage: `url(${cityBack})` }}
        ></div>

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

      {/* Cairo section */}
      <section className="city-section" id="cairo" style={{ display: 'none' }}>
        <h2>Cairo Secrets</h2>
        <p>From historic streets to vibrant markets, Cairo has endless treasures to explore.</p>
        <div className="options-container">
          <button className="option-button">Restaurants</button>
          <button className="option-button">Clothes</button>
          <button className="option-button">Parks</button>
        </div>
      </section>

      {/* Giza section */}
      <section className="city-section" id="giza" style={{ display: 'none' }}>
        <h2>Giza Secrets</h2>
        <p>Discover the pyramids, hidden streets, and local spots in Giza.</p>
        <div className="options-container">
          <button className="option-button">Restaurants</button>
          <button className="option-button">Clothes</button>
          <button className="option-button">Parks</button>
        </div>
      </section>
    </div>
  );
}

export default App;
