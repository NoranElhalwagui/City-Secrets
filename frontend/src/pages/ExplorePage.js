import React, { useEffect, useState } from "react";
import "./ExplorePage.css";
import Sidebar from "../components/Sidebar";
import axios from "axios";
import { FaMapMarkerAlt, FaFilter, FaSearch, FaHeart, FaStar } from "react-icons/fa";

export default function ExplorePage({ setPage }) {
  const [places, setPlaces] = useState([]);
  const [filteredPlaces, setFilteredPlaces] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");
  const [filters, setFilters] = useState({
    location: "all",
    category: "all",
    sortBy: "rating"
  });

  useEffect(() => {
    fetchPlaces();
  }, []);

  useEffect(() => {
    filterAndSortPlaces();
  }, [places, filters, searchTerm]);

  const fetchPlaces = async () => {
    try {
      setLoading(true);
      const response = await axios.get('http://localhost:5000/api/places');
      setPlaces(response.data);
      setFilteredPlaces(response.data);
    } catch (error) {
      console.error('Error fetching places:', error);
    } finally {
      setLoading(false);
    }
  };

  const filterAndSortPlaces = () => {
    let result = [...places];

    // Apply search filter
    if (searchTerm) {
      const term = searchTerm.toLowerCase();
      result = result.filter(place => 
        place.name.toLowerCase().includes(term) ||
        place.description.toLowerCase().includes(term) ||
        place.location.toLowerCase().includes(term)
      );
    }

    // Apply location filter
    if (filters.location !== "all") {
      result = result.filter(place => 
        place.location.toLowerCase() === filters.location.toLowerCase()
      );
    }

    // Apply category filter
    if (filters.category !== "all") {
      result = result.filter(place => 
        place.categoryId?.toString() === filters.category
      );
    }

    // Apply sorting
    switch (filters.sortBy) {
      case "rating":
        result.sort((a, b) => (b.averageRating || 0) - (a.averageRating || 0));
        break;
      case "newest":
        result.sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt));
        break;
      case "popular":
        result.sort((a, b) => (b.reviewCount || 0) - (a.reviewCount || 0));
        break;
      default:
        break;
    }

    setFilteredPlaces(result);
  };

  const handleFilterChange = (filterName, value) => {
    setFilters(prev => ({
      ...prev,
      [filterName]: value
    }));
  };

  const handlePlaceClick = (placeId) => {
    // Navigate to place details or show modal
    console.log('Viewing place:', placeId);
  };

  const handleAddToFavorites = async (placeId) => {
    try {
      const token = localStorage.getItem('accessToken');
      await axios.post(`http://localhost:5000/api/places/${placeId}/favorite`, {}, {
        headers: { Authorization: `Bearer ${token}` }
      });
      // Update UI
    } catch (error) {
      console.error('Error adding to favorites:', error);
    }
  };

  return (
    <div className="explore-container">
      <Sidebar setPage={setPage} />
      
      <div className="explore-content">
        <div className="explore-header">
          <h1>Explore Hidden Gems</h1>
          <p>Discover amazing places around you</p>
        </div>

        {/* Search and Filters */}
        <div className="explore-controls">
          <div className="search-bar">
            <FaSearch className="search-icon" />
            <input
              type="text"
              placeholder="Search places by name, description, or location..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>

          <div className="filter-controls">
            <div className="filter-group">
              <FaFilter />
              <select 
                value={filters.location}
                onChange={(e) => handleFilterChange('location', e.target.value)}
              >
                <option value="all">All Locations</option>
                <option value="cairo">Cairo</option>
                <option value="giza">Giza</option>
                <option value="alexandria">Alexandria</option>
                <option value="luxor">Luxor</option>
              </select>
            </div>

            <div className="filter-group">
              <select 
                value={filters.category}
                onChange={(e) => handleFilterChange('category', e.target.value)}
              >
                <option value="all">All Categories</option>
                <option value="1">Cafes & Restaurants</option>
                <option value="2">Historical Sites</option>
                <option value="3">Nature Spots</option>
                <option value="4">Art Galleries</option>
                <option value="5">Hidden Gems</option>
              </select>
            </div>

            <div className="filter-group">
              <select 
                value={filters.sortBy}
                onChange={(e) => handleFilterChange('sortBy', e.target.value)}
              >
                <option value="rating">Top Rated</option>
                <option value="newest">Newest</option>
                <option value="popular">Most Popular</option>
              </select>
            </div>
          </div>
        </div>

        {/* Places Grid */}
        {loading ? (
          <div className="loading-spinner">
            <div className="spinner"></div>
            <p>Loading places...</p>
          </div>
        ) : filteredPlaces.length === 0 ? (
          <div className="no-results">
            <h3>No places found</h3>
            <p>Try adjusting your search or filters</p>
          </div>
        ) : (
          <div className="places-grid">
            {filteredPlaces.map(place => (
              <div key={place.id} className="place-card">
                <div className="place-image">
                  <img 
                    src={place.images?.[0] || "https://images.unsplash.com/photo-1513584684374-8bab748fbf90"} 
                    alt={place.name} 
                  />
                  {place.isHiddenGem && (
                    <span className="gem-badge">✨ Hidden Gem</span>
                  )}
                  <button 
                    className="favorite-btn"
                    onClick={() => handleAddToFavorites(place.id)}
                  >
                    <FaHeart />
                  </button>
                </div>

                <div className="place-info">
                  <h3>{place.name}</h3>
                  
                  <div className="place-meta">
                    <span className="location">
                      <FaMapMarkerAlt /> {place.location}
                    </span>
                    {place.averageRating > 0 && (
                      <span className="rating">
                        <FaStar /> {place.averageRating.toFixed(1)}
                      </span>
                    )}
                  </div>

                  <p className="place-description">
                    {place.description?.length > 150 
                      ? `${place.description.substring(0, 150)}...` 
                      : place.description}
                  </p>

                  <div className="place-stats">
                    <span>👁️ {place.viewCount || 0} views</span>
                    <span>💬 {place.reviewCount || 0} reviews</span>
                  </div>

                  <div className="place-actions">
                    <button 
                      className="view-btn"
                      onClick={() => handlePlaceClick(place.id)}
                    >
                      View Details
                    </button>
                    <button className="map-btn">
                      Show on Map
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Map Integration Section */}
        <div className="map-section">
          <h2>Places on Map</h2>
          <div className="map-placeholder">
            <p>🌍 Interactive map would appear here</p>
            <small>Showing {filteredPlaces.length} places in your area</small>
          </div>
        </div>
      </div>
    </div>
  );
}