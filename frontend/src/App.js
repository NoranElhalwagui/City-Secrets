import { useRef, useState } from 'react';
import HomePage from "./pages/HomePage";
import cityBack from './assets/backg.jpg';
import cairoImg from './assets/cairo.jpg';
import gizaImg from './assets/giza.jpg';
import './App.css'; 
import { useNavigate } from "react-router-dom";

function App() {
  const [page, setPage] = useState('myHome'); 
  const [activeCity, setActiveCity] = useState(null);
  const [hiddenGems, setHiddenGems] = useState([]);
  const [previewImages, setPreviewImages] = useState([]);

  const cairoSectionRef = useRef(null);
  const gizaSectionRef = useRef(null);

  const cairoRefs = {
    Restaurants: useRef(null),
    Clothes: useRef(null),
    Parks: useRef(null),
    HiddenGems: useRef(null),
  };

  const gizaRefs = {
    Restaurants: useRef(null),
    Clothes: useRef(null),
    Parks: useRef(null),
    HiddenGems: useRef(null),
  };

  const cairoRestaurants = [
    { name: "Nile View Cafe", description: "Beautiful view of the Nile with Egyptian dishes." },
    { name: "Koshary El Tahrir", description: "Famous for traditional koshary." },
  ];

  const gizaRestaurants = [
    { name: "Pyramid Lounge", description: "Dine near the pyramids with authentic food." },
    { name: "Giza Garden Cafe", description: "Cozy garden seating and coffee." },
  ];

  const handleCityClick = (city) => {
    setActiveCity(city);
    const ref = city === 'cairo' ? cairoSectionRef : gizaSectionRef;
    if (ref.current) ref.current.scrollIntoView({ behavior: 'smooth' });
  };

  const handleCategoryClick = (city, category) => {
    const ref = city === 'cairo' ? cairoRefs[category] : gizaRefs[category];
    if (ref.current) ref.current.scrollIntoView({ behavior: 'smooth' });
  };

  const handleAddGem = async (e) => {
    e.preventDefault();
    const form = e.target;
    const newGem = {
      name: form.name.value,
      city: form.city.value,
      location: form.location.value,
      description: form.description.value,
      images: Array.from(form.images.files),
    };

    // 1️⃣ Update state
    setHiddenGems([...hiddenGems, newGem]);
    setPage('explorer');
    setActiveCity(newGem.city.toLowerCase());
    setPreviewImages([]);

    // 2️⃣ Send POST request to backend
    try {
      const response = await fetch("https://localhost:7164/api/places", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: newGem.name,
          description: newGem.description,
          address: newGem.location,
          latitude: 0, // replace with actual value if available
          longitude: 0, // replace with actual value if available
          averagePrice: 0, // optional
          categoryId: 1, // adjust if you have categories
        }),
      });

      if (!response.ok) throw new Error("Failed to add gem to backend");

      const data = await response.json();
      console.log("Backend response:", data);
    } catch (err) {
      console.error(err);
    }
  };

  const handleImagePreview = (e) => {
    setPreviewImages(Array.from(e.target.files).map(file => URL.createObjectURL(file)));
  };

  const handleDeleteGem = (index) => {
    const newGems = [...hiddenGems];
    newGems.splice(index, 1);
    setHiddenGems(newGems);
  };

  // ------------------- Home Page -------------------
  if (page === 'myHome') {
    return (
      <div className="App">
        <HomePage setPage={setPage} />
        <div className="top-buttons">
          <button className="option-button" onClick={() => setPage('landing')}>Go Back</button>
        </div>
      </div>
    );
  }

  // ------------------- Landing Page -------------------
  if (page === 'landing') {
    return (
      <div className="App">
        <section className="hero">
          <div className="background-blur" style={{ backgroundImage: `url(${cityBack})` }}></div>
          <div className="background-overlay"></div>

          <header className="hero-header">
            <div className="top-buttons">
              <button className="option-button" onClick={() => setPage('myHome')}>Go Back</button>
            </div>

            <h1>Welcome to City Secrets</h1>
            <p>Are you an Adventurer or a Hidden Gem Owner?</p>

            <div className="button-container">
              <button className="option-button" onClick={() => setPage('explorer')}>Adventurer</button>
              <button className="option-button" onClick={() => setPage('ownerForm')}>Hidden Gem Owner</button>
            </div>
          </header>
        </section>
      </div>
    );
  }

  // ------------------- Owner Form -------------------
  if (page === 'ownerForm') {
    return (
      <div className="App">
        <section className="city-section">
          <div className="top-buttons">
            <button className="option-button" onClick={() => setPage('landing')}>Go Back</button>
          </div>

          <h2>Add Your Hidden Gem and Make it Shine</h2>

          <form onSubmit={handleAddGem} className="owner-form">
            <input name="name" placeholder="Hidden Gem Name" required />
            <select name="city" required>
              <option value="">Select City</option>
              <option value="Cairo">Cairo</option>
              <option value="Giza">Giza</option>
            </select>
            <input name="location" placeholder="Location / Street" required />
            <input name="images" type="file" multiple onChange={handleImagePreview} />

            {previewImages.length > 0 && (
              <div className="preview-container">
                {previewImages.map((src, i) => (
                  <img key={i} src={src} alt="preview" className="gem-thumb" />
                ))}
              </div>
            )}

            <textarea name="description" placeholder="Description" rows={4} required></textarea>
            <button type="submit" className="option-button">Add Hidden Gem</button>
          </form>
        </section>
      </div>
    );
  }

  // ------------------- Explorer Page -------------------
  return (
    <div className="App">
      <section className="hero">
        <div className="background-blur" style={{ backgroundImage: `url(${cityBack})` }}></div>
        <div className="background-overlay"></div>

        <header className="hero-header">
          <div className="top-buttons">
            <button className="option-button" onClick={() => setPage('landing')}>Go Back</button>
          </div>

          <h1>Explore Hidden Gems</h1>
          <p>Discover hidden treasures in Cairo and Giza!</p>

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
      {(!activeCity || activeCity === 'cairo') && (
        <section className="city-section" id="cairo" ref={cairoSectionRef}>
          <h2>Cairo Secrets</h2>
          <p>From historic streets to vibrant markets, Cairo has endless treasures to explore.</p>

          <div className="options-container">
            {["Restaurants", "Clothes", "Parks", "HiddenGems"].map(cat => (
              <button
                key={cat}
                className="option-button"
                onClick={() => handleCategoryClick('cairo', cat)}
              >
                {cat === "HiddenGems" ? "Hidden Gems" : cat}
              </button>
            ))}
          </div>

          <div className="sub-section" ref={cairoRefs.Restaurants}>
            <h3>Restaurants in Cairo</h3>
            {cairoRestaurants.map((res, i) => (
              <p key={i}><strong>{res.name}:</strong> {res.description}</p>
            ))}
          </div>

          <div className="sub-section" ref={cairoRefs.HiddenGems}>
            <h3>Hidden Gems in Cairo</h3>
            {hiddenGems.filter(g => g.city === "Cairo").map((gem, i) => (
              <div key={i} className="hidden-gem">
                <h4>{gem.name}</h4>
                <p>{gem.description}</p>
                <p><strong>Location:</strong> {gem.location}</p>

                <div className="gem-images">
                  {gem.images.map((file, idx) => (
                    <img key={idx} src={URL.createObjectURL(file)} alt={gem.name} className="gem-thumb" />
                  ))}
                </div>

                <button className="option-button delete-btn" onClick={() => handleDeleteGem(i)}>Delete</button>
              </div>
            ))}
          </div>
        </section>
      )}

      {/* Giza Section */}
      {(!activeCity || activeCity === 'giza') && (
        <section className="city-section" id="giza" ref={gizaSectionRef}>
          <h2>Giza Secrets</h2>
          <p>Discover the pyramids, hidden streets, and local spots in Giza.</p>

          <div className="options-container">
            {["Restaurants", "Clothes", "Parks", "HiddenGems"].map(cat => (
              <button
                key={cat}
                className="option-button"
                onClick={() => handleCategoryClick('giza', cat)}
              >
                {cat === "HiddenGems" ? "Hidden Gems" : cat}
              </button>
            ))}
          </div>

          <div className="sub-section" ref={gizaRefs.Restaurants}>
            <h3>Restaurants in Giza</h3>
            {gizaRestaurants.map((res, i) => (
              <p key={i}><strong>{res.name}:</strong> {res.description}</p>
            ))}
          </div>

          <div className="sub-section" ref={gizaRefs.HiddenGems}>
            <h3>Hidden Gems in Giza</h3>
            {hiddenGems.filter(g => g.city === "Giza").map((gem, i) => (
              <div key={i} className="hidden-gem">
                <h4>{gem.name}</h4>
                <p>{gem.description}</p>
                <p><strong>Location:</strong> {gem.location}</p>

                <div className="gem-images">
                  {gem.images.map((file, idx) => (
                    <img key={idx} src={URL.createObjectURL(file)} alt={gem.name} className="gem-thumb" />
                  ))}
                </div>

                <button className="option-button delete-btn" onClick={() => handleDeleteGem(i)}>Delete</button>
              </div>
            ))}
          </div>
        </section>
      )}
    </div>
  );
}

export default App;
