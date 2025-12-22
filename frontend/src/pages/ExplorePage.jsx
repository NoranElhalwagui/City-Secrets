// pages/ExplorePage.jsx
import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "./ExplorePage.css";

export default function ExplorePage() {
  const navigate = useNavigate();
  const [posts, setPosts] = useState([
    {
      id: 1,
      user: "Alice",
      place: "Hidden Cafe",
      category: "Food",
      rating: 4.5,
      opinion: "Great vibe and coffee! ☕",
    },
    {
      id: 2,
      user: "Bob",
      place: "Secret Museum",
      category: "Museum",
      rating: 4.8,
      opinion: "Loved the ancient artifacts!",
    },
  ]);

  // Particle background effect
  useEffect(() => {
    const canvas = document.getElementById("particle-bg-explore");
    const ctx = canvas.getContext("2d");
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;

    const particles = [];
    for (let i = 0; i < 100; i++) {
      particles.push({
        x: Math.random() * canvas.width,
        y: Math.random() * canvas.height,
        r: Math.random() * 2 + 1,
        dx: Math.random() * 0.5 - 0.25,
        dy: Math.random() * 0.5 - 0.25,
      });
    }

    function animate() {
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      particles.forEach((p) => {
        ctx.beginPath();
        ctx.arc(p.x, p.y, p.r, 0, Math.PI * 2);
        ctx.fillStyle = "yellow";
        ctx.fill();
        p.x += p.dx;
        p.y += p.dy;

        if (p.x < 0 || p.x > canvas.width) p.dx *= -1;
        if (p.y < 0 || p.y > canvas.height) p.dy *= -1;
      });
      requestAnimationFrame(animate);
    }
    animate();

    const handleResize = () => {
      canvas.width = window.innerWidth;
      canvas.height = window.innerHeight;
    };
    window.addEventListener("resize", handleResize);
    return () => window.removeEventListener("resize", handleResize);
  }, []);

  const handleAddPost = () => {
    const place = prompt("Place name:");
    const opinion = prompt("Your opinion about this place:");
    if (place && opinion) {
      const newPost = {
        id: posts.length + 1,
        user: "You",
        place,
        category: "Unknown",
        rating: 5,
        opinion,
      };
      setPosts([newPost, ...posts]);
    }
  };

  const handleAddPlace = () => {
    navigate("/add-place");
  };

  return (
    <div className="explore-container">
      <canvas id="particle-bg-explore" className="particle-bg-explore"></canvas>

      <h1 className="page-title">Explore Places</h1>

      {/* Buttons */}
      <div className="explore-buttons">
        <button onClick={handleAddPost} className="explore-btn">Add Post</button>
        <button onClick={handleAddPlace} className="explore-btn">Add Your Own Place</button>
      </div>

      {/* Posts Feed */}
      <div className="feed">
        {posts.map((p) => (
          <div key={p.id} className="feed-card">
            <h3>{p.user} reviewed {p.place}</h3>
            <p>Category: {p.category}</p>
            <p>Rating: ⭐ {p.rating}</p>
            <p className="opinion">"{p.opinion}"</p>
          </div>
        ))}
      </div>
    </div>
  );
}
