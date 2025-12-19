import React, { useState } from 'react';
import './PlaceCard.css';
import { 
  FaHeart, 
  FaStar, 
  FaMapMarkerAlt, 
  FaEye, 
  FaComment,
  FaShare,
  FaBookmark,
  FaCrown
} from 'react-icons/fa';

export default function PlaceCard({ place, onLike, onBookmark, onShare }) {
  const [isLiked, setIsLiked] = useState(false);
  const [isBookmarked, setIsBookmarked] = useState(false);
  const [imageError, setImageError] = useState(false);

  const handleLike = () => {
    setIsLiked(!isLiked);
    onLike?.(place.id);
  };

  const handleBookmark = () => {
    setIsBookmarked(!isBookmarked);
    onBookmark?.(place.id);
  };

  const handleShare = () => {
    onShare?.(place);
  };

  const fallbackImage = 'https://images.unsplash.com/photo-1513584684374-8bab748fbf90?auto=format&fit=crop&w=800';

  return (
    <div className="place-card">
      {/* Image with Overlay */}
      <div className="card-image">
        <img 
          src={imageError ? fallbackImage : (place.images?.[0] || fallbackImage)} 
          alt={place.name}
          onError={() => setImageError(true)}
          loading="lazy"
        />
        
        {/* Top Badges */}
        <div className="card-badges">
          {place.isHiddenGem && (
            <span className="badge gem">
              <FaCrown /> Hidden Gem
            </span>
          )}
          {place.isVerified && (
            <span className="badge verified">
              ✓ Verified
            </span>
          )}
        </div>

        {/* Action Buttons */}
        <div className="card-actions">
          <button 
            className={`action-btn ${isBookmarked ? 'bookmarked' : ''}`}
            onClick={handleBookmark}
            aria-label={isBookmarked ? "Remove bookmark" : "Bookmark"}
          >
            <FaBookmark />
          </button>
          <button 
            className="action-btn share"
            onClick={handleShare}
            aria-label="Share"
          >
            <FaShare />
          </button>
        </div>
      </div>

      {/* Content */}
      <div className="card-content">
        <div className="card-header">
          <h3 className="place-title">{place.name}</h3>
          <div className="place-rating">
            <FaStar className="star-icon" />
            <span>{place.rating?.toFixed(1) || '4.5'}</span>
          </div>
        </div>

        <div className="place-location">
          <FaMapMarkerAlt />
          <span>{place.location || 'Cairo, Egypt'}</span>
        </div>

        <p className="place-description">
          {place.description?.length > 120 
            ? `${place.description.substring(0, 120)}...` 
            : place.description || 'Discover this amazing hidden gem!'}
        </p>

        <div className="place-stats">
          <div className="stat">
            <FaEye />
            <span>{place.views?.toLocaleString() || '1.2K'} views</span>
          </div>
          <div className="stat">
            <FaComment />
            <span>{place.reviews?.length || '45'} reviews</span>
          </div>
          <div className="stat">
            <FaHeart />
            <span>{place.likes?.toLocaleString() || '256'} likes</span>
          </div>
        </div>

        {/* Bottom Actions */}
        <div className="card-footer">
          <button 
            className={`like-btn ${isLiked ? 'liked' : ''}`}
            onClick={handleLike}
          >
            <FaHeart />
            <span>{isLiked ? 'Liked' : 'Like'}</span>
          </button>
          <button className="explore-btn">
            Explore Now
          </button>
        </div>
      </div>
    </div>
  );
}