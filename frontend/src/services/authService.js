// src/services/authService.js

const API_BASE_URL = "http://localhost:5293";

export async function login(email, password) {
  const body = {
    email,
    password,
    ipAddress: "",
    userAgent: navigator.userAgent || "",
  };

  const res = await fetch(`${API_BASE_URL}/api/Auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });

  if (!res.ok) {
    let message = `Login failed with status ${res.status}`;
    try {
      const err = await res.json();
      message = err.message || message;
    } catch {}
    return { success: false, message };
  }

  const data = await res.json();

  return {
    success: true,
    accessToken: data.accessToken,
    refreshToken: data.refreshToken,
    user: {
      username: data.user?.userName || data.userName || "",
      isAdmin: data.user?.isAdmin ?? data.isAdmin ?? false,
      ...data.user,
    },
  };
}

export async function register({ username, fullName, email, password }) {
  // Match Swagger exactly:
  // {
  //   "username": "string",
  //   "email": "user@example.com",
  //   "password": "string",
  //   "fullName": "string"
  // }
  const body = {
    username,
    email,
    password,
    fullName,
  };

  const res = await fetch(`${API_BASE_URL}/api/Auth/register`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });

  if (!res.ok) {
    let message = `Register failed with status ${res.status}`;

    try {
      const text = await res.text();

      let err;
      try {
        err = JSON.parse(text);
      } catch {
        err = null;
      }

      if (err) {
        if (err.errors) {
          const allErrors = Object.values(err.errors).flat();
          if (allErrors.length > 0) {
            message = allErrors.join("\n");
          }
        } else if (err.title) {
          message = err.title;
        } else if (err.message) {
          message = err.message;
        }
      } else if (text) {
        message = text;
      }
    } catch {
      // ignore parse issues, keep default message
    }

    return { success: false, message };
  }

  const data = await res.json();

  return {
    success: true,
    accessToken: data.accessToken,
    refreshToken: data.refreshToken,
    user: {
      username: data.user?.userName || data.userName || username,
      isAdmin: data.user?.isAdmin ?? data.isAdmin ?? false,
      ...data.user,
    },
  };
}

export async function forgotPassword(email) {
  const body = { email }; // adjust if Swagger shows a different shape

  const res = await fetch(`${API_BASE_URL}/api/Auth/forgot-password`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(body),
  });

  if (!res.ok) {
    let message = `Forgot password failed with status ${res.status}`;
    try {
      const err = await res.json();
      message = err.message || message;
    } catch {}
    throw new Error(message);
  }

  return true;
}