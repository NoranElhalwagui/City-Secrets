const express = require('express');
const cors = require('cors');
const jwt = require('jsonwebtoken');
const app = express();
const PORT = 5000;

// Middleware
app.use(cors({
  origin: ['http://localhost:3000', 'http://localhost:3001'],
  credentials: true
}));
app.use(express.json());

// In-memory storage (for demo purposes)
let users = [
  {
    userId: 1,
    username: 'Admin',
    email: 'admin@gmail.com',
    password: '121212',
    fullName: 'Administrator',
    isAdmin: true,
    emailVerified: true,
    createdAt: new Date().toISOString(),
    lastLoginAt: null
  }
];

let places = [];
let reviews = [];

// JWT Secret
const JWT_SECRET = 'your-secret-key';

// Helper function to generate JWT
function generateToken(user) {
  return jwt.sign(
    {
      userId: user.userId,
      username: user.username,
      email: user.email,
      isAdmin: user.isAdmin
    },
    JWT_SECRET,
    { expiresIn: '24h' }
  );
}

// Auth Routes
app.post('/api/auth/register', (req, res) => {
  try {
    const { username, email, password } = req.body;

    // Check if user already exists
    const existingUser = users.find(u => u.email === email || u.username === username);
    if (existingUser) {
      return res.status(400).json({
        success: false,
        message: existingUser.email === email ? 'Email already exists' : 'Username already exists'
      });
    }

    // Create new user
    const newUser = {
      userId: users.length + 1,
      username,
      email,
      password, // In real app, hash this!
      fullName: '',
      isAdmin: false,
      emailVerified: true,
      createdAt: new Date().toISOString(),
      lastLoginAt: null
    };

    users.push(newUser);

    // Generate tokens
    const accessToken = generateToken(newUser);
    const refreshToken = 'refresh-' + accessToken;

    res.json({
      success: true,
      message: 'Registration successful',
      accessToken,
      refreshToken,
      user: {
        userId: newUser.userId,
        username: newUser.username,
        email: newUser.email,
        fullName: newUser.fullName,
        isAdmin: newUser.isAdmin,
        emailVerified: newUser.emailVerified,
        createdAt: newUser.createdAt,
        lastLoginAt: newUser.lastLoginAt
      }
    });
  } catch (error) {
    res.status(500).json({ success: false, message: 'Server error' });
  }
});

app.post('/api/auth/login', (req, res) => {
  try {
    const { email, password } = req.body;

    const user = users.find(u => u.email === email && u.password === password);
    if (!user) {
      return res.status(401).json({
        success: false,
        message: 'Invalid credentials'
      });
    }

    // Update last login
    user.lastLoginAt = new Date().toISOString();

    // Generate tokens
    const accessToken = generateToken(user);
    const refreshToken = 'refresh-' + accessToken;

    res.json({
      success: true,
      message: 'Login successful',
      accessToken,
      refreshToken,
      user: {
        userId: user.userId,
        username: user.username,
        email: user.email,
        fullName: user.fullName,
        isAdmin: user.isAdmin,
        emailVerified: user.emailVerified,
        createdAt: user.createdAt,
        lastLoginAt: user.lastLoginAt
      }
    });
  } catch (error) {
    res.status(500).json({ success: false, message: 'Server error' });
  }
});

// Places endpoint
app.get('/api/places', (req, res) => {
  // Return some sample places
  const samplePlaces = [
    {
      id: 1,
      name: 'Hidden Cafe',
      description: 'A secret coffee spot with amazing pastries',
      location: 'Cairo',
      images: ['https://images.unsplash.com/photo-1509042239860-f550ce710b93'],
      averageRating: 4.5,
      reviewCount: 12,
      isHiddenGem: true
    },
    {
      id: 2,
      name: 'Rooftop Garden',
      description: 'Beautiful garden on top of an old building',
      location: 'Giza',
      images: ['https://images.unsplash.com/photo-1416879595882-3373a0480b5b'],
      averageRating: 4.2,
      reviewCount: 8,
      isHiddenGem: true
    }
  ];

  res.json(samplePlaces);
});

// Reviews endpoint
app.get('/api/reviews', (req, res) => {
  // Return some sample reviews
  const sampleReviews = [
    {
      id: 1,
      placeId: 1,
      userId: 1,
      username: 'explorer123',
      rating: 5,
      comment: 'Amazing hidden gem! The coffee is incredible.',
      images: [],
      createdAt: new Date().toISOString(),
      helpfulCount: 3
    }
  ];

  res.json(sampleReviews);
});

// Start server
app.listen(PORT, () => {
  console.log(`Mock backend server running on http://localhost:${PORT}`);
  console.log('Available endpoints:');
  console.log('POST /api/auth/register');
  console.log('POST /api/auth/login');
  console.log('GET /api/places');
  console.log('GET /api/reviews');
});