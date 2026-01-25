import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./ExplorePage.css";

export default function ExplorePage({ posts, setPosts, currentUser }) {
  const navigate = useNavigate();

  const [approvedPlaces, setApprovedPlaces] = useState([]);
  const [newComment, setNewComment] = useState("");

  /* ================= LOAD APPROVED PLACES ================= */
  useEffect(() => {
    const approved =
      JSON.parse(localStorage.getItem("approvedPlaces")) || [];
    setApprovedPlaces(approved);
  }, []);

  /* ================= POST INTERACTIONS ================= */
  const handleLike = (id) => {
    setPosts(
      posts.map((p) =>
        p.id === id && !p.likedBy.includes(currentUser)
          ? {
              ...p,
              likes: p.likes + 1,
              likedBy: [...p.likedBy, currentUser],
            }
          : p
      )
    );
  };

  const handleFavorite = (id) => {
    setPosts(
      posts.map((p) =>
        p.id === id && !p.favoritedBy.includes(currentUser)
          ? {
              ...p,
              favorites: p.favorites + 1,
              favoritedBy: [...p.favoritedBy, currentUser],
            }
          : p
      )
    );
  };

  const handleAddComment = (id) => {
    if (!newComment.trim()) return;
    setPosts(
      posts.map((p) =>
        p.id === id
          ? { ...p, comments: [...p.comments, newComment] }
          : p
      )
    );
    setNewComment("");
  };

  return (
    <div className="explore-container">
      <h1 className="page-title">Explore Places</h1>

      <div className="feed-actions">
        <button className="primary-btn" onClick={() => navigate("/create-post")}>
          Make Your Own Post
        </button>

        <button className="secondary-btn" onClick={() => navigate("/add-place")}>
          Add Your Own Place
        </button>
      </div>

      {/* ================= APPROVED PLACES ================= */}
      <h2 className="section-title">Places</h2>

      {approvedPlaces.length === 0 && (
        <p className="empty-text">No places yet 🌍</p>
      )}

      <div className="places-grid">
        {approvedPlaces.map((place) => (
          <div key={place.id} className="place-card">
            <h3>{place.name}</h3>
            <p>{place.description}</p>
            <p>
              <strong>Location:</strong> {place.location}
            </p>
          </div>
        ))}
      </div>

      {/* ================= POSTS ================= */}
      <h2 className="section-title">Community Posts</h2>

      <div className="feed">
        {posts.map((post) => (
          <div key={post.id} className="post-card">
            <div className="post-header">
              <strong>{post.user}</strong> reviewed{" "}
              <strong>{post.place}</strong>
            </div>

            {post.images.map((img, i) => (
              <img key={i} src={img} alt="Post" className="post-image" />
            ))}

            <p className="post-text">{post.description}</p>

            <div className="post-actions">
              <button onClick={() => handleFavorite(post.id)}>
                ❤️ Favorite ({post.favorites})
              </button>
              <button onClick={() => handleLike(post.id)}>
                👍 Like ({post.likes})
              </button>
            </div>

            <div className="comments">
              {post.comments.map((c, i) => (
                <p key={i} className="comment">• {c}</p>
              ))}

              <div className="add-comment">
                <input
                  type="text"
                  placeholder="Write a comment..."
                  value={newComment}
                  onChange={(e) => setNewComment(e.target.value)}
                />
                <button onClick={() => handleAddComment(post.id)}>
                  Comment
                </button>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
