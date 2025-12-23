import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./ExplorePage.css";

export default function ExplorePage({ posts, setPosts, currentUser }) {
  const navigate = useNavigate();
  const [newComment, setNewComment] = useState("");

  const handleLike = (id) => {
    setPosts(
      posts.map((p) => {
        if (p.id === id) {
          if (p.likedBy.includes(currentUser)) return p;
          return {
            ...p,
            likes: p.likes + 1,
            likedBy: [...p.likedBy, currentUser],
          };
        }
        return p;
      })
    );
  };

  const handleFavorite = (id) => {
    setPosts(
      posts.map((p) => {
        if (p.id === id) {
          if (p.favoritedBy.includes(currentUser)) return p;
          return {
            ...p,
            favorites: p.favorites + 1,
            favoritedBy: [...p.favoritedBy, currentUser],
          };
        }
        return p;
      })
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
        <button
          className="primary-btn"
          onClick={() => navigate("/create-post")}
        >
          Make Your Own Post
        </button>

        <button
          className="secondary-btn"
          onClick={() => navigate("/add-place")}
        >
          Add Your Own Place
        </button>
      </div>

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
