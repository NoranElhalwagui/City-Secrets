import React, { useEffect, useState } from "react";
import "./HiddenGemOwnerPage.css";
import Sidebar from "../components/Sidebar";
import axios from "axios";
import { 
  FaUpload, 
  FaMapMarkerAlt, 
  FaPhone, 
  FaGlobe, 
  FaClock, 
  FaCheckCircle,
  FaTimesCircle,
  FaSpinner
} from "react-icons/fa";

export default function HiddenGemOwnerPage({ setPage }) {
  const [formData, setFormData] = useState({
    name: "",
    description: "",
    location: "",
    address: "",
    phone: "",
    website: "",
    email: "",
    openingHours: "",
    categoryId: "5", // Hidden Gems category
    images: [],
    isHiddenGem: true,
    submittedBy: ""
  });
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState({ text: "", type: "" });
  const [submissions, setSubmissions] = useState([]);
  const [user, setUser] = useState(null);

  useEffect(() => {
    fetchUserData();
    fetchUserSubmissions();
  }, []);

  const fetchUserData = () => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      axios.get('http://localhost:5000/api/auth/me', {
        headers: { Authorization: `Bearer ${token}` }
      })
      .then(response => {
        setUser(response.data);
        setFormData(prev => ({
          ...prev,
          submittedBy: response.data.id,
          email: response.data.email
        }));
      })
      .catch(error => console.error('Error fetching user:', error));
    }
  };

  const fetchUserSubmissions = async () => {
    try {
      const token = localStorage.getItem('accessToken');
      const response = await axios.get('http://localhost:5000/api/places?submittedBy=me', {
        headers: { Authorization: `Bearer ${token}` }
      });
      setSubmissions(response.data);
    } catch (error) {
      console.error('Error fetching submissions:', error);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleImageUpload = (e) => {
    const files = Array.from(e.target.files);
    if (files.length + formData.images.length > 5) {
      setMessage({ 
        text: "Maximum 5 images allowed", 
        type: "error" 
      });
      return;
    }

    const imagePreviews = files.map(file => ({
      file,
      preview: URL.createObjectURL(file)
    }));

    setFormData(prev => ({
      ...prev,
      images: [...prev.images, ...imagePreviews]
    }));
  };

  const removeImage = (index) => {
    setFormData(prev => ({
      ...prev,
      images: prev.images.filter((_, i) => i !== index)
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (!user) {
      setMessage({ 
        text: "Please login to submit a place", 
        type: "error" 
      });
      return;
    }

    setLoading(true);
    setMessage({ text: "", type: "" });

    try {
      const token = localStorage.getItem('accessToken');
      const data = new FormData();

      // Append all form data
      Object.keys(formData).forEach(key => {
        if (key !== 'images') {
          data.append(key, formData[key]);
        }
      });

      // Append images
      formData.images.forEach(image => {
        data.append('images', image.file);
      });

      // Submit to admin for approval
      const response = await axios.post('http://localhost:5000/api/places', data, {
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'multipart/form-data'
        }
      });

      setMessage({ 
        text: "Place submitted successfully! It's now pending admin approval.", 
        type: "success" 
      });

      // Reset form
      setFormData({
        name: "",
        description: "",
        location: "",
        address: "",
        phone: "",
        website: "",
        email: user.email,
        openingHours: "",
        categoryId: "5",
        images: [],
        isHiddenGem: true,
        submittedBy: user.id
      });

      // Refresh submissions list
      fetchUserSubmissions();

      // Notify admin via another endpoint (optional)
      await axios.post(`http://localhost:5000/api/admin/notify-new-submission`, {
        placeId: response.data.id,
        placeName: response.data.name,
        submittedBy: user.username
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });

    } catch (error) {
      setMessage({ 
        text: error.response?.data?.message || "Submission failed. Please try again.", 
        type: "error" 
      });
    } finally {
      setLoading(false);
    }
  };

  const getStatusBadge = (status) => {
    switch (status?.toLowerCase()) {
      case 'approved':
        return { icon: <FaCheckCircle />, color: '#2ecc71', text: 'Approved' };
      case 'rejected':
        return { icon: <FaTimesCircle />, color: '#e74c3c', text: 'Rejected' };
      case 'pending':
        return { icon: <FaSpinner />, color: '#f39c12', text: 'Pending Review' };
      default:
        return { icon: <FaSpinner />, color: '#95a5a6', text: 'Pending' };
    }
  };

  return (
    <div className="owner-container">
      <Sidebar setPage={setPage} />
      
      <div className="owner-content">
        <div className="owner-header">
          <h1>Share Your Hidden Gem</h1>
          <p>Submit your unique place for review and join our exclusive network</p>
        </div>

        <div className="owner-main">
          {/* Left: Submission Form */}
          <div className="submission-form">
            <h2>Place Submission Form</h2>
            
            {message.text && (
              <div className={`message ${message.type}`}>
                {message.text}
              </div>
            )}

            <form onSubmit={handleSubmit}>
              <div className="form-grid">
                <div className="form-group">
                  <label>Place Name *</label>
                  <input
                    type="text"
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                    placeholder="e.g., The Secret Garden Cafe"
                    required
                  />
                </div>

                <div className="form-group">
                  <label>Location *</label>
                  <select
                    name="location"
                    value={formData.location}
                    onChange={handleChange}
                    required
                  >
                    <option value="">Select Location</option>
                    <option value="Cairo">Cairo</option>
                    <option value="Giza">Giza</option>
                    <option value="Alexandria">Alexandria</option>
                    <option value="Luxor">Luxor</option>
                    <option value="Aswan">Aswan</option>
                  </select>
                </div>

                <div className="form-group">
                  <label>Full Address *</label>
                  <input
                    type="text"
                    name="address"
                    value={formData.address}
                    onChange={handleChange}
                    placeholder="Street address, district, city"
                    required
                  />
                </div>

                <div className="form-group">
                  <label>Contact Phone</label>
                  <input
                    type="tel"
                    name="phone"
                    value={formData.phone}
                    onChange={handleChange}
                    placeholder="+20 123 456 7890"
                  />
                </div>

                <div className="form-group">
                  <label>Website/Social Media</label>
                  <input
                    type="url"
                    name="website"
                    value={formData.website}
                    onChange={handleChange}
                    placeholder="https://example.com"
                  />
                </div>

                <div className="form-group">
                  <label>Opening Hours</label>
                  <input
                    type="text"
                    name="openingHours"
                    value={formData.openingHours}
                    onChange={handleChange}
                    placeholder="e.g., 9 AM - 11 PM daily"
                  />
                </div>
              </div>

              <div className="form-group">
                <label>Description *</label>
                <textarea
                  name="description"
                  value={formData.description}
                  onChange={handleChange}
                  placeholder="Describe your place. What makes it special? What services do you offer? What's unique about it?"
                  rows="4"
                  required
                />
              </div>

              <div className="form-group">
                <label>Upload Images (Max 5)</label>
                <div className="image-upload-area">
                  <label className="upload-label">
                    <FaUpload className="upload-icon" />
                    <span>Click to upload images</span>
                    <input
                      type="file"
                      accept="image/*"
                      multiple
                      onChange={handleImageUpload}
                      style={{ display: 'none' }}
                    />
                    <small>Recommended: 1200x800 pixels, max 2MB each</small>
                  </label>
                  
                  <div className="image-previews">
                    {formData.images.map((image, index) => (
                      <div key={index} className="image-preview">
                        <img src={image.preview} alt={`Preview ${index}`} />
                        <button 
                          type="button" 
                          className="remove-image"
                          onClick={() => removeImage(index)}
                        >
                          ×
                        </button>
                      </div>
                    ))}
                  </div>
                </div>
              </div>

              <div className="form-note">
                <p>
                  <strong>Note:</strong> All submissions are reviewed by our admin team. 
                  You'll be notified via email when your place is approved or if additional 
                  information is required. Approval usually takes 24-48 hours.
                </p>
              </div>

              <button 
                type="submit" 
                className="submit-btn"
                disabled={loading || !user}
              >
                {loading ? (
                  <>
                    <FaSpinner className="spinning" /> Submitting...
                  </>
                ) : !user ? (
                  "Please Login to Submit"
                ) : (
                  "Submit for Review"
                )}
              </button>
            </form>
          </div>

          {/* Right: Submission History & Stats */}
          <div className="submission-sidebar">
            <div className="owner-info">
              <h3>Owner Information</h3>
              {user ? (
                <>
                  <p><strong>Name:</strong> {user.username}</p>
                  <p><strong>Email:</strong> {user.email}</p>
                  <p><strong>Status:</strong> Verified Owner</p>
                  <p><strong>Submissions:</strong> {submissions.length} places</p>
                </>
              ) : (
                <p>Please login to view your information</p>
              )}
            </div>

            <div className="submission-history">
              <h3>Your Submissions</h3>
              {submissions.length === 0 ? (
                <p className="no-submissions">No submissions yet</p>
              ) : (
                <div className="submissions-list">
                  {submissions.map(submission => {
                    const status = getStatusBadge(submission.status);
                    return (
                      <div key={submission.id} className="submission-item">
                        <div className="submission-header">
                          <h4>{submission.name}</h4>
                          <span 
                            className="status-badge"
                            style={{ color: status.color }}
                          >
                            {status.icon} {status.text}
                          </span>
                        </div>
                        <p className="submission-location">
                          <FaMapMarkerAlt /> {submission.location}
                        </p>
                        <p className="submission-description">
                          {submission.description.substring(0, 80)}...
                        </p>
                        <small>
                          Submitted: {new Date(submission.createdAt).toLocaleDateString()}
                        </small>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>

            <div className="owner-benefits">
              <h3>Benefits of Listing</h3>
              <ul>
                <li>✨ Increased visibility to thousands of users</li>
                <li>⭐ Featured in our "Hidden Gems" section</li>
                <li>📱 Dedicated place page with reviews</li>
                <li>📈 Analytics and visitor insights</li>
                <li>🎯 Targeted audience interested in unique places</li>
                <li>🤝 Priority support for featured places</li>
              </ul>
            </div>

            <button 
              className="explore-btn"
              onClick={() => setPage("explore")}
            >
              Explore Other Places
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}