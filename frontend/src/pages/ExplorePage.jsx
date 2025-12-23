import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./ExplorePage.css";

export default function ExplorePage() {
  const navigate = useNavigate();

  const [posts, setPosts] = useState([
    {
      id: 1,
      user: "Alice",
      place: "Hidden Cafe",
      image: "/demo/cafe.jpg",
      opinion: "Amazing coffee and calm vibes ☕",
      likes: 12,
      comments: ["Love this place!", "Added to my list"],
    },
    {
      id: 2,
      user: "Bob",
      place: "Secret Museum",
      image: "/demo/museum.jpg",
      opinion: "A real hidden gem!",
      likes: 20,
      comments: [],
    },
  ]);

  const [newComment, setNewComment] = useState("");

  const handleLike = (id) => {
    setPosts(
      posts.map((p) =>
        p.id === id ? { ...p, likes: p.likes + 1 } : p
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

      {/* TOP ACTION BUTTONS */}
      <div className="feed-actions">
        <button
          className="primary-btn"
          onClick={() => alert("Create Post page comes next 😉")}
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

      {/* FEED */}
      <div className="feed">
        {posts.map((post) => (
          <div key={post.id} className="post-card">
            <div className="post-header">
              <strong>{post.user}</strong> reviewed{" "}
              <strong>{post.place}</strong>
            </div>

            {post.image && (
              <img
                src={post.image}
                alt={post.place}
                className="post-image"
              />
            )}

            <p className="post-text">{post.opinion}</p>

            <div className="post-actions">
              <button onClick={() => handleLike(post.id)}>
                ❤️ Favorite ({post.likes})
              </button>
            </div>

            {/* COMMENTS */}
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
