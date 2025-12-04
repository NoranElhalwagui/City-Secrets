import { useRef, useState } from 'react';
import HomePage from "./pages/HomePage";
import cityBack from './assets/backg.jpg';
import cairoImg from './assets/cairo.jpg';
import gizaImg from './assets/giza.jpg';
import './App.css'; 
import Sidebar from "./components/Sidebar";

function App() {
  const [page, setPage] = useState('myHome'); // myHome, landing, explorer, ownerForm, adminLogin, adminPage
  const [activeCity, setActiveCity] = useState(null);
  const [hiddenGems, setHiddenGems] = useState([]);
  const [previewImages, setPreviewImages] = useState([]);
  const [pendingRequests, setPendingRequests] = useState([]);
  const [admin, setAdmin] = useState(null);
  const [editRequestIndex, setEditRequestIndex] = useState(null);

  // Hardcoded admins
  const admins = [
    { id: "admin1", pass: "1234" },
    { id: "admin2", pass: "2345" },
    { id: "admin3", pass: "3456" },
    { id: "admin4", pass: "4567" },
  ];

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

  // ------------------- Handlers -------------------
  const handleCityClick = (city) => {
    setActiveCity(city);
    const ref = city === 'cairo' ? cairoSectionRef : gizaSectionRef;
    if (ref.current) ref.current.scrollIntoView({ behavior: 'smooth' });
  };

  const handleCategoryClick = (city, category) => {
    const ref = city === 'cairo' ? cairoRefs[category] : gizaRefs[category];
    if (ref.current) ref.current.scrollIntoView({ behavior: 'smooth' });
  };

  const handleImagePreview = (e) => {
    setPreviewImages(Array.from(e.target.files).map(file => URL.createObjectURL(file)));
  };

  // ------------------- Owner submits gem -------------------
  const handleAddGemRequest = (e) => {
    e.preventDefault();
    const form = e.target;
    const newGem = {
      name: form.name.value,
      city: form.city.value,
      location: form.location.value,
      description: form.description.value,
      images: Array.from(form.images.files),
    };

    // If editing a request
    if (editRequestIndex !== null) {
      const updatedRequests = [...pendingRequests];
      updatedRequests[editRequestIndex] = newGem;
      setPendingRequests(updatedRequests);
      setEditRequestIndex(null);
      alert("Request updated successfully!");
    } else {
      setPendingRequests([...pendingRequests, newGem]);
      alert("Your gem request has been sent to admin for approval!");
    }

    setPage('landing');
    setPreviewImages([]);
  };

  // ------------------- Admin actions -------------------
  const handleApproveRequest = (index) => {
    const gem = pendingRequests[index];
    setHiddenGems([...hiddenGems, gem]);
    const newRequests = [...pendingRequests];
    newRequests.splice(index, 1);
    setPendingRequests(newRequests);
  };

  const handleDeleteRequest = (index) => {
    const newRequests = [...pendingRequests];
    newRequests.splice(index, 1);
    setPendingRequests(newRequests);
  };

  const handleEditRequest = (index) => {
    const request = pendingRequests[index];
    setEditRequestIndex(index);
    setPreviewImages(request.images.map(file => URL.createObjectURL(file)));
    setPage('ownerForm');
  };

  // ------------------- Admin login -------------------
  const handleAdminLogin = (e) => {
    e.preventDefault();
    const id = e.target.id.value;
    const pass = e.target.pass.value;
    const found = admins.find(a => a.id === id && a.pass === pass);
    if (found) {
      setAdmin(found);
      setPage('adminPage');
    } else {
      alert("Invalid ID or password!");
    }
  };

  // ------------------- Landing Page -------------------
  if (page === 'myHome') return <div className="App"><HomePage setPage={setPage} /></div>;

  if (page === 'landing') return (
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

  // ------------------- Owner Form -------------------
  if (page === 'ownerForm') return (
    <div className="App">
      <Sidebar setPage={setPage} />
      <section className="city-section">
        <div className="top-buttons">
          <button className="option-button" onClick={() => { setPage('landing'); setEditRequestIndex(null); }}>Go Back</button>
        </div>
        <h2>{editRequestIndex !== null ? "Edit Your Hidden Gem Request" : "Add Your Hidden Gem"}</h2>
        <form onSubmit={handleAddGemRequest} className="owner-form">
          <input name="name" placeholder="Hidden Gem Name" defaultValue={editRequestIndex !== null ? pendingRequests[editRequestIndex].name : ""} required />
          <select name="city" defaultValue={editRequestIndex !== null ? pendingRequests[editRequestIndex].city : ""} required>
            <option value="">Select City</option>
            <option value="Cairo">Cairo</option>
            <option value="Giza">Giza</option>
          </select>
          <input name="location" placeholder="Location / Street" defaultValue={editRequestIndex !== null ? pendingRequests[editRequestIndex].location : ""} required />
          <input name="images" type="file" multiple onChange={handleImagePreview} />
          {previewImages.length > 0 && (
            <div className="preview-container">
              {previewImages.map((src, i) => (
                <img key={i} src={src} alt="preview" className="gem-thumb" />
              ))}
            </div>
          )}
          <textarea name="description" placeholder="Description" rows={4} defaultValue={editRequestIndex !== null ? pendingRequests[editRequestIndex].description : ""} required></textarea>
          <button type="submit" className="option-button">{editRequestIndex !== null ? "Update Request" : "Send Request"}</button>
        </form>
      </section>
    </div>
  );

  // ------------------- Admin Login -------------------
  if (page === 'adminLogin') return (
    <div className="App">
      <section className="city-section">
        <h2>Admin Login</h2>
        <form onSubmit={handleAdminLogin}>
          <input name="id" placeholder="Admin ID" required />
          <input name="pass" placeholder="Password" type="password" required />
          <button type="submit" className="option-button">Login</button>
        </form>
      </section>
    </div>
  );

  // ------------------- Admin Page -------------------
  if (page === 'adminPage') return (
    <div className="App">
      <Sidebar setPage={setPage} />
      <section className="city-section">
        <div className="top-buttons">
          <button className="option-button" onClick={() => { setAdmin(null); setPage('landing'); }}>Logout</button>
        </div>
        <h2>Pending Hidden Gem Requests</h2>
        {pendingRequests.length === 0 && <p>No pending requests.</p>}
        {pendingRequests.map((req, i) => (
          <div key={i} className="hidden-gem">
            <h4>{req.name}</h4>
            <p>{req.description}</p>
            <p><strong>City:</strong> {req.city}</p>
            <p><strong>Location:</strong> {req.location}</p>
            <div className="gem-images">
              {req.images.map((file, idx) => (
                <img key={idx} src={URL.createObjectURL(file)} alt={req.name} className="gem-thumb" />
              ))}
            </div>
            <button className="option-button" onClick={() => handleApproveRequest(i)}>Approve</button>
            <button className="option-button delete-btn" onClick={() => handleDeleteRequest(i)}>Delete</button>
            <button className="option-button" onClick={() => handleEditRequest(i)}>Edit</button>
          </div>
        ))}
      </section>
    </div>
  );

  // ------------------- Explorer Page -------------------
  return (
    <div className="App">
      <Sidebar setPage={setPage} />
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
              </div>
            ))}
          </div>
        </section>
      )}
    </div>
  );
}

export default App;
