import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Bar } from "react-chartjs-2";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";

import "./AdminDashboard.css";

// Register chart.js components
ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend);

export default function AdminDashboard() {
  const navigate = useNavigate();
  const [stats, setStats] = useState({
    places: 0,
    pending: 0,
  });

  const [approvedPlaces, setApprovedPlaces] = useState([]);

  useEffect(() => {
    const pending = JSON.parse(localStorage.getItem("pendingPlaces")) || [];
    const approved = JSON.parse(localStorage.getItem("approvedPlaces")) || [];

    setApprovedPlaces(approved);

    setStats({
      places: approved.length,
      pending: pending.length,
    });
  }, []);

  // Data for the chart
  const chartData = {
    labels: approvedPlaces.map((p) => p.name),
    datasets: [
      {
        label: "Approved Places",
        data: approvedPlaces.map(() => 1), // count each place as 1
        backgroundColor: "rgba(255, 215, 0, 0.7)",
      },
    ],
  };

  const chartOptions = {
    responsive: true,
    plugins: {
      legend: {
        display: false,
      },
      title: {
        display: true,
        text: "Approved Places Analysis",
        color: "#ffd700",
        font: {
          size: 22,
        },
      },
    },
    scales: {
      y: {
        beginAtZero: true,
        ticks: {
          stepSize: 1,
          color: "white",
        },
        grid: {
          color: "rgba(255,255,255,0.2)",
        },
      },
      x: {
        ticks: {
          color: "white",
        },
        grid: {
          color: "rgba(255,255,255,0.2)",
        },
      },
    },
  };

  return (
    <div className="admin-container">
      <h1 className="page-title">Admin Dashboard</h1>

      {/* DASHBOARD STATS */}
      <section className="dashboard-stats">
        <h2>Dashboard Stats</h2>
        <div className="stats">
          <div className="stat-card">Approved Places: {stats.places}</div>
          <div className="stat-card">Pending Places: {stats.pending}</div>
        </div>
      </section>

      {/* PLACES MANAGEMENT */}
      <section className="places-management">
        <h2>Places Management</h2>
        <div className="admin-actions">
          <button onClick={() => navigate("/admin/pending-places")}>
            View Pending Places
          </button>
          <button onClick={() => navigate("/explore")}>
            View Approved Places
          </button>
        </div>
      </section>

      {/* ANALYSIS */}
      <section className="analysis-section">
        <h2>Analysis</h2>
        {approvedPlaces.length === 0 ? (
          <p>No approved places yet.</p>
        ) : (
          <div className="analysis-cards">
            <Bar data={chartData} options={chartOptions} />
          </div>
        )}
      </section>
    </div>
  );
}
