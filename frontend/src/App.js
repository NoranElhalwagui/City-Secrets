import React, { useState } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import HomePage from "./pages/HomePage";
import LoginPage from "./pages/LoginPage";
import ProfilePage from "./pages/ProfilePage";
import ExplorePage from "./pages/ExplorePage";
import AddPlacePage from "./pages/AddPlacePage";
import AdminDashboard from "./pages/AdminDashboard";
import CreatePostPage from "./pages/CreatePostPage";

export default function App() {
  // posts state lifted to App so ExplorePage & CreatePostPage share it
  const [posts, setPosts] = useState([
    {
      id: 1,
      user: "Alice",
      place: "Hidden Cafe",
      images: ["/demo/cafe.jpg"],
      description: "Amazing coffee and calm vibes ☕",
      likes: 12,
      likedBy: [],
      favorites: 3,
      favoritedBy: [],
      comments: ["Love this place!", "Added to my list"],
    },
    {
      id: 2,
      user: "Bob",
      place: "Secret Museum",
      images: ["/demo/museum.jpg"],
      description: "A real hidden gem!",
      likes: 20,
      likedBy: [],
      favorites: 5,
      favoritedBy: [],
      comments: [],
    },
  ]);

  const currentUser = "You"; // simulate logged-in user

  return (
    <Router>
      <Routes>
        <Route path="/" element={<HomePage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/profile" element={<ProfilePage />} />

        <Route
          path="/explore"
          element={
            <ExplorePage
              posts={posts}
              setPosts={setPosts}
              currentUser={currentUser}
            />
          }
        />

        <Route
          path="/create-post"
          element={
            <CreatePostPage
              addPost={(p) => setPosts([p, ...posts])}
              currentUser={currentUser}
            />
          }
        />

        <Route path="/add-place" element={<AddPlacePage />} />
        <Route path="/admin" element={<AdminDashboard />} />
      </Routes>
    </Router>
  );
}
