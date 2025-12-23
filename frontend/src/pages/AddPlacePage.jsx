import React, { useState } from "react";
import "./AddPlacePage.css";

export default function AddPlacePage() {
  const [place, setPlace] = useState({
    name: "",
    category: "",
    description: "",
    image: null,
  });

  const handleSubmit = (e) => {
    e.preventDefault();

    console.log("Submitted to admin:", place);

    alert("Your place was sent to admin for verification ✅");
  };

  return (
    <div className="add-place-container">
      <h1>Add Your Own Place</h1>

      <form onSubmit={handleSubmit} className="add-place-form">
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
          <option value="Museum">Museum</option>
          <option value="Cafe">Cafe</option>
        </select>

        <textarea
          placeholder="Description"
          required
          onChange={(e) =>
            setPlace({ ...place, description: e.target.value })
          }
        />

        <input
          type="file"
          onChange={(e) =>
            setPlace({ ...place, image: e.target.files[0] })
          }
        />

        <button type="submit" className="primary-btn">
          Send to Admin
        </button>
      </form>
    </div>
  );
}
