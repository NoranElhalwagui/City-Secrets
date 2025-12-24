import HomePage from "../pages/HomePage";
import { renderWithRouter, screen } from "../test-utils";

test("renders homepage headline and login button", () => {
  renderWithRouter(<HomePage />);

  // headline
  expect(
    screen.getByRole("heading", { name: /discover hidden gems/i })
  ).toBeInTheDocument();

  // login button
  expect(
    screen.getByRole("button", { name: /login/i })
  ).toBeInTheDocument();
});
