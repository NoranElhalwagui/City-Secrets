import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./AddPlacePage.css";

export default function AddPlacePage() {
  const navigate = useNavigate();

  const [place, setPlace] = useState({
    name: "",
    description: "",
    address: "",
    latitude: "",
    longitude: "",
    category: "",
    averagePrice: "",
    priceRange: "",
    phoneNumber: "",
    website: "",
    email: "",
    openingTime: "",
    closingTime: "",
    location: "",
    details: "",
    images: [],
  });

  const [previews, setPreviews] = useState([]);
  const [submitted, setSubmitted] = useState(false);

  const handleImageChange = (e) => {
    const files = Array.from(e.target.files);
    setPlace({ ...place, images: files });

    const urls = files.map((file) => URL.createObjectURL(file));
    setPreviews(urls);
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!place.location || place.images.length === 0) {
      alert("Please select a location and upload at least one image.");
      return;
    }

    const newPlace = {
      ...place,
      id: Date.now(), // fake unique ID
      createdBy: "User",
      status: "pending",
      images: previews, // store image previews for admin view
    };

    const existingPending =
      JSON.parse(localStorage.getItem("pendingPlaces")) || [];

    localStorage.setItem(
      "pendingPlaces",
      JSON.stringify([...existingPending, newPlace])
    );

    setSubmitted(true);
  };

  if (submitted) {
    return (
      <div className="add-place-container">
        <div className="success-card">
          <h1>Thank you for submitting your place ✨</h1>
          <p>
            Your request has been sent to our admin team for review.
            Once approved, it will appear for everyone to explore.
          </p>
          <button
            className="primary-btn"
            onClick={() => navigate("/explore")}
          >
            Continue Exploring
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="add-place-container">
      <form className="add-place-card wide-form" onSubmit={handleSubmit}>
        <h1>Share Your Hidden Gem!</h1>

        {/* LOCATION */}
        <select
          required
          defaultValue=""
          onChange={(e) => setPlace({ ...place, location: e.target.value })}
        >
          <option value="" disabled>Select a Location</option>
          <option value="Cairo">Cairo</option>
          <option value="Giza">Giza</option>
        </select>

        {/* IMAGE UPLOAD */}
        <div className="image-upload">
          {previews.length > 0 ? (
            <div className="preview-grid">
              {previews.map((src, i) => (
                <img key={i} src={src} alt={`preview ${i}`} />
              ))}
            </div>
          ) : (
            <label className="upload-placeholder">
              📸 Upload photos (multiple allowed)
              <input
                type="file"
                accept="image/*"
                multiple
                hidden
                onChange={handleImageChange}
              />
            </label>
          )}
        </div>

        {/* BASIC INFO */}
        <input
          type="text"
          placeholder="Place Name"
          required
          onChange={(e) => setPlace({ ...place, name: e.target.value })}
        />

        <textarea
          placeholder="Description"
          required
          onChange={(e) => setPlace({ ...place, description: e.target.value })}
        />

        <input
          type="text"
          placeholder="Address"
          required
          onChange={(e) => setPlace({ ...place, address: e.target.value })}
        />

        {/* COORDINATES */}
        <div className="coord-inputs">
          <input
            type="number"
            placeholder="Latitude"
            required
            onChange={(e) => setPlace({ ...place, latitude: e.target.value })}
          />
          <input
            type="number"
            placeholder="Longitude"
            required
            onChange={(e) => setPlace({ ...place, longitude: e.target.value })}
          />
        </div>

        {/* CATEGORY */}
        <select
          required
          defaultValue=""
          onChange={(e) => setPlace({ ...place, category: e.target.value })}
        >
          <option value="" disabled>Select a Category</option>
          <option value="Restaurant">Restaurant</option>
          <option value="Cafe">Cafe</option>
          <option value="Park">Park</option>
          <option value="Museum">Museum</option>
          <option value="Shop">Shop</option>
        </select>

        {/* PRICING */}
        <input
          type="number"
          placeholder="Average Price"
          required
          onChange={(e) => setPlace({ ...place, averagePrice: e.target.value })}
        />

        <input
          type="text"
          placeholder="Price Range"
          onChange={(e) => setPlace({ ...place, priceRange: e.target.value })}
        />

        {/* CONTACT */}
        <input
          type="text"
          placeholder="Phone Number"
          onChange={(e) => setPlace({ ...place, phoneNumber: e.target.value })}
        />

        <input
          type="email"
          placeholder="Email"
          onChange={(e) => setPlace({ ...place, email: e.target.value })}
        />

        <input
          type="text"
          placeholder="Website"
          onChange={(e) => setPlace({ ...place, website: e.target.value })}
        />

        {/* TIME */}
        <div className="time-selects">
          <input
            type="time"
            required
            onChange={(e) => setPlace({ ...place, openingTime: e.target.value })}
          />
          <input
            type="time"
            required
            onChange={(e) => setPlace({ ...place, closingTime: e.target.value })}
          />
        </div>

        <textarea
          placeholder="Additional Details"
          onChange={(e) => setPlace({ ...place, details: e.target.value })}
        />

        <button type="submit" className="primary-btn full">
          Send to Admin
        </button>
      </form>
    </div>
  );
}
