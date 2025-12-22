import { render, screen } from "@testing-library/react";
import HomePage from "../pages/HomePage";

jest.mock("axios");

test("renders Home Page without crashing", () => {
  render(<HomePage />);

  const heading = screen.getByRole("heading", {
    level: 1,
    name: /city secrets/i,
  });

  expect(heading).toBeInTheDocument();
});
