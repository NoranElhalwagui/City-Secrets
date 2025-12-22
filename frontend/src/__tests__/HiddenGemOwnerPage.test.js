import { render, screen } from "@testing-library/react";
import HiddenGemOwnerPage from "../pages/HiddenGemOwnerPage";
import axios from "axios";

jest.mock("axios");

beforeEach(() => {
  localStorage.setItem("accessToken", "fake-token");

  axios.get.mockResolvedValueOnce({
    data: { id: 1, username: "Owner", email: "owner@test.com" },
  });

  axios.get.mockResolvedValueOnce({ data: [] });
});

test("renders Hidden Gem submission page", async () => {
  render(<HiddenGemOwnerPage setPage={jest.fn()} />);
  expect(
    await screen.findByText(/Share Your Hidden Gem/i)
  ).toBeInTheDocument();
});
