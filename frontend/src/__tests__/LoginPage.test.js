import LoginPage from "../pages/LoginPage";
import { renderWithRouter, screen, fireEvent } from "../test-utils";

test("shows login form by default", () => {
  renderWithRouter(<LoginPage />);

  expect(screen.getByPlaceholderText(/email/i)).toBeInTheDocument();
  expect(screen.getByPlaceholderText(/password/i)).toBeInTheDocument();
});

test("switches to register mode", () => {
  renderWithRouter(<LoginPage />);

  fireEvent.click(screen.getByText(/sign up/i));

  expect(screen.getByPlaceholderText(/name/i)).toBeInTheDocument();
});

test("switches to forgot password mode", () => {
  renderWithRouter(<LoginPage />);

  fireEvent.click(screen.getByText(/forgot password/i));

  expect(
    screen.getByPlaceholderText(/enter your email/i)
  ).toBeInTheDocument();
});
