// pages/AddPlacePage.jsx
import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "./AddPlacePage.css";

export default function AddPlacePage() {
  const navigate = useNavigate();
  const [name, setName] = useState("");
  const [category, setCategory] = useState("");
  const [description, setDescription] = useState("");
  const [image, setImage] = useState(null);
  const [categories, setCategories] = useState([]);

  // Load categories from API (mock here)
  useEffect(() => {
    setCategories(["Food", "Parks", "Museum", "Library"]);
  }, []);

  const handleSubmit = (e) => {
    e.preventDefault();

    const newPlace = {
      name,
      category,
      description,
      image,
      status: "pending", // sent to admin for approval
    };

    console.log("Submitting new place:", newPlace);

    alert("Place request sent to admin for approval!");
    navigate("/explore");
  };

  return (
    <div className="add-place-container">
      <h1>Add Your Own Place</h1>
      <form onSubmit={handleSubmit}>
        <input
          type="text"
          placeholder="Place Name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
        <select value={category} onChange={(e) => setCategory(e.target.value)} required>
          <option value="">Select Category</option>
          {categories.map((c, i) => (
            <option key={i} value={c}>{c}</option>
          ))}
        </select>
        <textarea
          placeholder="Description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          required
        />
        <input type="file" onChange={(e) => setImage(e.target.files[0])} required />
        <button type="submit">Submit for Approval</button>
      </form>
    </div>
  );
}
