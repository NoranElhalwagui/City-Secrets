import React from 'react';
import './LoadingSpinner.css';

export default function LoadingSpinner({ size = 'medium', color = 'primary', message = 'Loading...' }) {
  const sizeClass = {
    small: 'spinner-small',
    medium: 'spinner-medium',
    large: 'spinner-large'
  }[size];

  const colorClass = {
    primary: 'spinner-primary',
    secondary: 'spinner-secondary',
    white: 'spinner-white'
  }[color];

  return (
    <div className="loading-container">
      <div className={`spinner ${sizeClass} ${colorClass}`}>
        <div className="spinner-circle"></div>
        <div className="spinner-circle"></div>
        <div className="spinner-circle"></div>
      </div>
      {message && <p className="loading-message">{message}</p>}
    </div>
  );
}