import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend,
} from "chart.js";
import { Bar } from "react-chartjs-2";
import "./AdminDashboard.css";

ChartJS.register(
  CategoryScale,
  LinearScale,
  BarElement,
  Title,
  Tooltip,
  Legend
);

export default function AdminDashboard() {
  const navigate = useNavigate();

  const [stats, setStats] = useState({
    places: 0,
    pending: 0,
  });

  useEffect(() => {
    const pendingPlaces =
      JSON.parse(localStorage.getItem("pendingPlaces")) || [];
    const approvedPlaces =
      JSON.parse(localStorage.getItem("approvedPlaces")) || [];

    setStats({
      places: approvedPlaces.length,
      pending: pendingPlaces.length,
    });
  }, []);

  /* ================= ANALYSIS DATA ================= */
  const chartData = {
    labels: ["Approved Places", "Pending Places"],
    datasets: [
      {
        label: "Places Overview",
        data: [stats.places, stats.pending],
        backgroundColor: [
          "rgba(255, 215, 0, 0.85)", // gold
          "rgba(255, 255, 255, 0.5)", // light
        ],
        borderRadius: 10,
      },
    ],
  };

  const chartOptions = {
    responsive: true,
    plugins: {
      legend: {
        labels: {
          color: "white",
          font: { size: 14 },
        },
      },
      title: {
        display: true,
        text: "Approved vs Pending Places",
        color: "#ffd700",
        font: {
          size: 22,
          weight: "bold",
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
          display: false,
        },
      },
    },
  };

  return (
    <div className="admin-container">
      {/* TITLE */}
      <h1 className="page-title">Admin Dashboard</h1>

      {/* STATS */}
      <section>
        <div className="stats">
          <div className="stat-card">Approved Places: {stats.places}</div>
          <div className="stat-card">Pending Places: {stats.pending}</div>
        </div>
      </section>

      {/* ANALYSIS */}
      <section className="analysis-section">
        <h2>Analysis</h2>
        <div className="analysis-box">
          <Bar data={chartData} options={chartOptions} />
        </div>
      </section>

      {/* PLACE MANAGEMENT */}
      <section>
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
    </div>
  );
}
