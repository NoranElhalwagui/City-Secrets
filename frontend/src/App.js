import { useRef, useState } from 'react';
import cityBack from './assets/backg.jpg';
import cairoImg from './assets/cairo.jpg';
import gizaImg from './assets/giza.jpg';
import './App.css';

function App() {
  const [page, setPage] = useState('landing'); // landing, explorer, ownerForm
  const [activeCity, setActiveCity] = useState(null);
  const [hiddenGems, setHiddenGems] = useState([]);
  const [previewImages, setPreviewImages] = useState([]);

  // Refs for city sections
  const cairoSectionRef = useRef(null);
  const gizaSectionRef = useRef(null);

  // Refs for sub-sections
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

    // Prepare data for API
    const placeData = {
      name: form.name.value,
      description: form.description.value,
      address: form.location.value,
      latitude: form.city.value === "Cairo" ? 30.0444 : 30.0131, // Default coordinates for Cairo/Giza
      longitude: form.city.value === "Cairo" ? 31.2357 : 31.2089,
      averagePrice: 0, // Default price, can be updated later
      categoryId: 1 // Default category, should be selected from form in future
    };

    try {
      // Send to API
      const response = await fetch("https://localhost:7164/api/places", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(placeData)
      });

      if (!response.ok) {
        throw new Error("Failed to submit place");
      }

      // Add to local state on success
      setHiddenGems([...hiddenGems, newGem]);
      setPage('explorer');
      setActiveCity(newGem.city.toLowerCase());
      setPreviewImages([]);
    } catch (error) {
      console.error("Error submitting place:", error);
      alert("Failed to submit place. Please try again.");
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

  // ------------------- Admin Functions -------------------
  const handleAdminCreatePlace = async (placeData) => {
    try {
      const response = await fetch("https://localhost:7164/api/admin/places", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: placeData.name,
          description: placeData.description,
          address: placeData.address,
          latitude: placeData.latitude,
          longitude: placeData.longitude,
          averagePrice: placeData.averagePrice || 0,
          categoryId: placeData.categoryId || 1
        })
      });

      if (!response.ok) {
        throw new Error("Failed to create place");
      }

      const result = await response.json();
      return result;
    } catch (error) {
      console.error("Error creating place as admin:", error);
      throw error;
    }
  };

  const handleAdminUpdatePlace = async (placeId, placeData) => {
    try {
      const response = await fetch(`https://localhost:7164/api/admin/places/${placeId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          name: placeData.name,
          description: placeData.description,
          address: placeData.address,
          latitude: placeData.latitude,
          longitude: placeData.longitude,
          averagePrice: placeData.averagePrice || 0,
          categoryId: placeData.categoryId || 1
        })
      });

      if (!response.ok) {
        throw new Error("Failed to update place");
      }

      const result = await response.json();
      return result;
    } catch (error) {
      console.error("Error updating place as admin:", error);
      throw error;
    }
  };

  const handleAdminDeletePlace = async (placeId, hardDelete = false) => {
    try {
      const response = await fetch(`https://localhost:7164/api/admin/places/${placeId}?hardDelete=${hardDelete}`, {
        method: "DELETE",
        headers: { "Content-Type": "application/json" }
      });

      if (!response.ok) {
        throw new Error("Failed to delete place");
      }

      const result = await response.json();
      return result;
    } catch (error) {
      console.error("Error deleting place as admin:", error);
      throw error;
    }
  };

  const handleAdminGetAllPlaces = async (includeDeleted = false) => {
    try {
      const response = await fetch(`https://localhost:7164/api/admin/places?includeDeleted=${includeDeleted}`, {
        method: "GET",
        headers: { "Content-Type": "application/json" }
      });

      if (!response.ok) {
        throw new Error("Failed to fetch places");
      }

      const result = await response.json();
      return result;
    } catch (error) {
      console.error("Error fetching places as admin:", error);
      throw error;
    }
  };

  const handleAdminSetHiddenGem = async (placeId, isHiddenGem, score) => {
    try {
      const response = await fetch(`https://localhost:7164/api/admin/places/${placeId}/hidden-gem`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          isHiddenGem: isHiddenGem,
          score: score
        })
      });

      if (!response.ok) {
        throw new Error("Failed to set hidden gem status");
      }

      const result = await response.json();
      return result;
    } catch (error) {
      console.error("Error setting hidden gem status:", error);
      throw error;
    }
  };

  // ------------------- Landing Page -------------------
  if (page === 'landing') {
    return (
      <div className="App">
        <section className="hero">
          <div className="background-blur" style={{ backgroundImage: `url(${cityBack})` }}></div>
          <div className="background-overlay"></div>
          <header className="hero-header">
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