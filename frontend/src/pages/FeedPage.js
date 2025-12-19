import React, { useEffect, useState } from "react";
import "./FeedPage.css";
import Sidebar from "../components/Sidebar";
import axios from "axios";
import { FaHeart, FaComment, FaPaperPlane, FaTrash } from "react-icons/fa";

export default function FeedPage({ setPage }) {
  const [posts, setPosts] = useState([]);
  const [newPost, setNewPost] = useState({
    image: null,
    imagePreview: "",
    placeName: "",
    description: "",
    location: ""
  });
  const [newComment, setNewComment] = useState("");
  const [loading, setLoading] = useState(false);
  const [activeCommentPost, setActiveCommentPost] = useState(null);
  const [user, setUser] = useState(null);

  useEffect(() => {
    fetchPosts();
    fetchUserData();
  }, []);

  const fetchUserData = () => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      axios.get('http://localhost:5000/api/auth/me', {
        headers: { Authorization: `Bearer ${token}` }
      })
      .then(response => setUser(response.data))
      .catch(error => console.error('Error fetching user:', error));
    }
  };

  const fetchPosts = async () => {
    try {
      const response = await axios.get('http://localhost:5000/api/places?includeReviews=true');
      // Transform places to post format
      const transformedPosts = response.data.map(place => ({
        id: place.id,
        userId: place.userId || 1,
        username: place.username || "Explorer",
        userAvatar: place.userAvatar || `https://i.pravatar.cc/150?u=${place.id}`,
        image: place.images?.[0] || "https://images.unsplash.com/photo-1513584684374-8bab748fbf90",
        placeName: place.name,
        description: place.description || "Amazing place!",
        location: place.location || "Cairo",
        likes: Math.floor(Math.random() * 1000),
        comments: place.reviews?.map(review => ({
          id: review.id,
          username: review.username || "User",
          text: review.content
        })) || [],
        liked: false,
        timestamp: new Date().toISOString()
      }));
      setPosts(transformedPosts);
    } catch (error) {
      console.error('Error fetching posts:', error);
    }
  };

  const handleLike = async (postId) => {
    setPosts(posts.map(post => 
      post.id === postId 
        ? { ...post, likes: post.liked ? post.likes - 1 : post.likes + 1, liked: !post.liked }
        : post
    ));
  };

  const handleAddComment = async (postId) => {
    if (!newComment.trim() || !user) return;

    try {
      const token = localStorage.getItem('accessToken');
      const response = await axios.post(`http://localhost:5000/api/places/${postId}/reviews`, {
        content: newComment,
        rating: 5
      }, {
        headers: { Authorization: `Bearer ${token}` }
      });

      setPosts(posts.map(post => 
        post.id === postId 
          ? { 
              ...post, 
              comments: [...post.comments, {
                id: response.data.id,
                username: user.username,
                text: newComment
              }] 
            }
          : post
      ));
      setNewComment("");
      setActiveCommentPost(null);
    } catch (error) {
      console.error('Error adding comment:', error);
    }
  };

  const handleCreatePost = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      const token = localStorage.getItem('accessToken');
      const formData = new FormData();
      formData.append('name', newPost.placeName);
      formData.append('description', newPost.description);
      formData.append('location', newPost.location);
      formData.append('categoryId', 1);
      if (newPost.image) {
        formData.append('image', newPost.image);
      }

      await axios.post('http://localhost:5000/api/places', formData, {
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'multipart/form-data'
        }
      });

      fetchPosts();
      setNewPost({
        image: null,
        imagePreview: "",
        placeName: "",
        description: "",
        location: ""
      });
      document.getElementById('postModal').style.display = 'none';
    } catch (error) {
      console.error('Error creating post:', error);
      alert('Failed to create post. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleDeletePost = async (postId) => {
    if (window.confirm("Are you sure you want to delete this post?")) {
      try {
        const token = localStorage.getItem('accessToken');
        await axios.delete(`http://localhost:5000/api/places/${postId}`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        fetchPosts();
      } catch (error) {
        console.error('Error deleting post:', error);
      }
    }
  };

  const openPostModal = () => {
    document.getElementById('postModal').style.display = 'flex';
  };

  const closePostModal = () => {
    document.getElementById('postModal').style.display = 'none';
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      setNewPost(prev => ({
        ...prev,
        image: file,
        imagePreview: URL.createObjectURL(file)
      }));
    }
  };

  return (
    <div className="feed-container">
      <Sidebar setPage={setPage} />
      
      <div className="feed-content">
        <div className="feed-header">
          <h1>Discover & Share</h1>
          <p>Explore hidden gems shared by our community</p>
          <button className="add-review-btn" onClick={openPostModal}>
            + Add a Review
          </button>
        </div>

        <div className="posts-grid">
          {posts.map(post => (
            <div key={post.id} className="post-card">
              <div className="post-header">
                <img src={post.userAvatar} alt={post.username} className="post-avatar" />
                <div>
                  <h3>{post.username}</h3>
                  <p className="post-location">{post.location}</p>
                </div>
                {user?.role === 'Admin' && (
                  <button 
                    className="delete-post-btn"
                    onClick={() => handleDeletePost(post.id)}
                  >
                    <FaTrash />
                  </button>
                )}
              </div>

              <img src={post.image} alt={post.placeName} className="post-image" />

              <div className="post-actions">
                <button 
                  className={`like-btn ${post.liked ? 'liked' : ''}`}
                  onClick={() => handleLike(post.id)}
                >
                  <FaHeart /> {post.likes}
                </button>
                <button 
                  className="comment-btn"
                  onClick={() => setActiveCommentPost(activeCommentPost === post.id ? null : post.id)}
                >
                  <FaComment /> {post.comments.length}
                </button>
                <button className="share-btn">
                  <FaPaperPlane />
                </button>
              </div>

              <div className="post-content">
                <h4>{post.placeName}</h4>
                <p>{post.description}</p>
                <small>{new Date(post.timestamp).toLocaleDateString()}</small>
              </div>

              <div className="post-comments">
                {post.comments.slice(0, 2).map(comment => (
                  <div key={comment.id} className="comment">
                    <strong>{comment.username}:</strong> {comment.text}
                  </div>
                ))}
                
                {activeCommentPost === post.id && (
                  <div className="add-comment">
                    <input
                      type="text"
                      placeholder="Add a comment..."
                      value={newComment}
                      onChange={(e) => setNewComment(e.target.value)}
                      onKeyPress={(e) => e.key === 'Enter' && handleAddComment(post.id)}
                    />
                    <button onClick={() => handleAddComment(post.id)}>Post</button>
                  </div>
                )}
                
                {post.comments.length > 2 && (
                  <button 
                    className="view-comments-btn"
                    onClick={() => setActiveCommentPost(post.id)}
                  >
                    View all {post.comments.length} comments
                  </button>
                )}
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* Create Post Modal */}
      <div id="postModal" className="modal">
        <div className="modal-content">
          <div className="modal-header">
            <h2>Share Your Hidden Gem</h2>
            <button className="close-btn" onClick={closePostModal}>×</button>
          </div>
          
          <form onSubmit={handleCreatePost}>
            <div className="form-group">
              <input
                type="text"
                placeholder="Place Name *"
                value={newPost.placeName}
                onChange={(e) => setNewPost({...newPost, placeName: e.target.value})}
                required
              />
            </div>
            
            <div className="form-group">
              <input
                type="text"
                placeholder="Location (e.g., Cairo, Giza) *"
                value={newPost.location}
                onChange={(e) => setNewPost({...newPost, location: e.target.value})}
                required
              />
            </div>
            
            <div className="form-group">
              <textarea
                placeholder="Describe your experience... *"
                value={newPost.description}
                onChange={(e) => setNewPost({...newPost, description: e.target.value})}
                rows="4"
                required
              />
            </div>
            
            <div className="form-group">
              <label className="file-upload">
                <input
                  type="file"
                  accept="image/*"
                  onChange={handleImageChange}
                />
                {newPost.imagePreview ? (
                  <img src={newPost.imagePreview} alt="Preview" className="image-preview" />
                ) : (
                  <>
                    <span>+ Upload Photo</span>
                    <small>Click to add an image of the place</small>
                  </>
                )}
              </label>
            </div>
            
            <button type="submit" className="submit-post-btn" disabled={loading}>
              {loading ? 'Posting...' : 'Share Post'}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}