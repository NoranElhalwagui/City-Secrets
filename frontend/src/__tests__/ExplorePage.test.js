import { render, screen } from "@testing-library/react";
import ExplorePage from "../pages/ExplorePage";
import axios from "axios";

jest.mock("axios");

test("renders Explore page title", async () => {
  axios.get.mockResolvedValueOnce({ data: [] });

  render(<ExplorePage setPage={jest.fn()} />);
  expect(await screen.findByText(/Explore Hidden Gems/i)).toBeInTheDocument();
});
