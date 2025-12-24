import HomePage from "../pages/HomePage";
import { renderWithRouter, screen } from "../test-utils";

test("renders HomePage title and login button", () => {
  renderWithRouter(<HomePage />);

  expect(
    screen.getByText(/discover hidden gems/i)
  ).toBeInTheDocument();

  expect(
    screen.getByRole("button", { name: /login/i })
  ).toBeInTheDocument();
});
