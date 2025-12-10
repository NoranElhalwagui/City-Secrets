import { useRef, useState } from 'react';
import HomePage from "./pages/HomePage";
import Landing from "./pages/Landing";
import AdminLogin from "./pages/AdminLogin";
import Explorer from "./pages/Explorer";
import AdminDashboard from './pages/AdminDashboard';
import OwnerForm from "./pages/OwnerForm";
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

  // homepage and landing and adminlogin and explorer and admindashboard and ownerform pages
  if (page === 'myHome') return <div className="App"><HomePage setPage={setPage} /></div>;

  if (page === 'landing') return <div className="App"><Landing setPage={setPage} /></div>

  if (page === 'adminLogin') return (<div className="App"><AdminLogin setPage={setPage}/></div>);

  if (page === "ownerForm") return (
    <OwnerForm
      setPage={setPage}
      handleAddGemRequest={handleAddGemRequest}
      handleImagePreview={handleImagePreview}
      previewImages={previewImages}
      pendingRequests={pendingRequests}
      editRequestIndex={editRequestIndex}
      setEditRequestIndex={setEditRequestIndex}
    />
  );


  if (page === "adminPage") return (
    <AdminDashboard
      setPage={setPage}
      setAdmin={setAdmin}
      pendingRequests={pendingRequests}
      handleApproveRequest={handleApproveRequest}
      handleDeleteRequest={handleDeleteRequest}
      handleEditRequest={handleEditRequest}
    />
  );


  if (page === "explorer") return (
    <Explorer
      setPage={setPage}
      activeCity={activeCity}
      handleCityClick={handleCityClick}
      handleCategoryClick={handleCategoryClick}
      cairoSectionRef={cairoSectionRef}
      gizaSectionRef={gizaSectionRef}
      cairoRefs={cairoRefs}
      gizaRefs={gizaRefs}
      cairoRestaurants={cairoRestaurants}
      gizaRestaurants={gizaRestaurants}
      hiddenGems={hiddenGems}
    />
  );



}

export default App;
