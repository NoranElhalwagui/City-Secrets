import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "./CreatePostPage.css";

export default function CreatePostPage({ addPost, currentUser = "You" }) {
  const navigate = useNavigate();

  const [post, setPost] = useState({
    description: "",
    images: [],
  });

  const [previews, setPreviews] = useState([]);

  const handleImageChange = (e) => {
    const files = Array.from(e.target.files);
    setPost({ ...post, images: files });

    // create previews
    const urls = files.map((f) => URL.createObjectURL(f));
    setPreviews(urls);
  };

  const handleSubmit = (e) => {
    e.preventDefault();

    if (!post.description || post.images.length === 0) {
      alert("Please add a description and at least one image.");
      return;
    }

    // Create new post object
    const newPost = {
      id: Date.now(),
      user: currentUser,
      description: post.description,
      images: post.images.map((f) => URL.createObjectURL(f)), // simulate
      likes: 0,
      likedBy: [],
      favorites: 0,
      favoritedBy: [],
      comments: [],
    };

    // Send back to parent (ExplorePage) to add post
    addPost(newPost);

    // Reset form
    setPost({ description: "", images: [] });
    setPreviews([]);

    // Go back to feed
    navigate("/explore");
  };

  return (
    <div className="create-post-container">
      <form className="create-post-card" onSubmit={handleSubmit}>
        <h1>Create Post</h1>

        {/* IMAGE UPLOAD */}
        <div className="image-upload">
          {previews.length > 0 ? (
            <div className="preview-grid">
              {previews.map((src, i) => (
                <img key={i} src={src} alt="preview" />
              ))}
            </div>
          ) : (
            <label className="upload-placeholder">
              <span>📸 Upload photos</span>
              <input
                type="file"
                accept="image/*"
                multiple
                onChange={handleImageChange}
                hidden
              />
            </label>
          )}
        </div>

        <textarea
          placeholder="Write something about this place..."
          required
          value={post.description}
          onChange={(e) =>
            setPost({ ...post, description: e.target.value })
          }
        />

        <button type="submit" className="primary-btn">
          Post
        </button>
      </form>
    </div>
  );
}
