import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./AddPlacePage.css";

export default function AddPlacePage() {
  const navigate = useNavigate();

  const [place, setPlace] = useState({
    name: "",
    category: "",
    description: "",
    image: null,
  });

  const [preview, setPreview] = useState(null);
  const [submitted, setSubmitted] = useState(false);

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (!file) return;

    setPlace({ ...place, image: file });
    setPreview(URL.createObjectURL(file));
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    // later → POST to /api/admin/pending-places
    console.log("Submitted to admin:", place);

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
      <form className="add-place-card" onSubmit={handleSubmit}>
        <h1>Add Your Place</h1>
        <p className="subtitle">
          Share a hidden gem with the community
        </p>

        {/* IMAGE UPLOAD */}
        <div className="image-upload">
          {preview ? (
            <img src={preview} alt="Preview" />
          ) : (
            <label className="upload-placeholder">
              <span>📸 Upload a photo</span>
              <input
                type="file"
                accept="image/*"
                onChange={handleImageChange}
                hidden
              />
            </label>
          )}
        </div>

        <input
          type="text"
          placeholder="Place Name"
          required
          onChange={(e) =>
            setPlace({ ...place, name: e.target.value })
          }
        />

        <select
          required
          onChange={(e) =>
            setPlace({ ...place, category: e.target.value })
          }
        >
          <option value="">Select Category</option>
          <option value="Food">Food</option>
          <option value="Cafe">Cafe</option>
          <option value="Museum">Museum</option>
          <option value="Park">Park</option>
        </select>

        <textarea
          placeholder="Why should people visit this place?"
          required
          onChange={(e) =>
            setPlace({ ...place, description: e.target.value })
          }
        />

        <button type="submit" className="primary-btn full">
          Send to Admin
        </button>
      </form>
    </div>
  );
}
