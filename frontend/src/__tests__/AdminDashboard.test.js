import { render, screen } from "@testing-library/react";
import AdminDashboard from "../pages/AdminDashboard";
import axios from "axios";

jest.mock("axios");

beforeEach(() => {
  localStorage.setItem(
    "user",
    JSON.stringify({ role: "Admin", username: "AdminUser" })
  );
  localStorage.setItem("accessToken", "fake-token");

  axios.get.mockResolvedValueOnce({
    data: {
      totalUsers: 5,
      totalPlaces: 10,
      pendingApprovals: 2,
      flaggedContent: 1,
    },
  });

  axios.get.mockResolvedValueOnce({ data: [] });
  axios.get.mockResolvedValueOnce({ data: [] });
});

test("renders Admin Dashboard heading", async () => {
  render(<AdminDashboard setPage={jest.fn()} />);
  expect(await screen.findByText(/Admin Dashboard/i)).toBeInTheDocument();
});
