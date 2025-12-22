import { render, screen } from "@testing-library/react";
import FeedPage from "../pages/FeedPage";
import axios from "axios";

jest.mock("axios");

beforeEach(() => {
  axios.get.mockResolvedValueOnce({
    data: [],
  });
});

test("renders Feed page header", async () => {
  render(<FeedPage setPage={jest.fn()} />);
  expect(
    await screen.findByText(/Discover & Share/i)
  ).toBeInTheDocument();
});
