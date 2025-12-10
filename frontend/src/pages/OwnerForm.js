import React from "react";
import "../App.css";
import Sidebar from "../components/Sidebar";

export default function OwnerForm({
  setPage,
  handleAddGemRequest,
  handleImagePreview,
  previewImages,
  pendingRequests,
  editRequestIndex,
  setEditRequestIndex
}) {

return(
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
}
