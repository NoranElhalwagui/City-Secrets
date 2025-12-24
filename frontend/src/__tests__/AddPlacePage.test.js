import AddPlacePage from "../pages/AddPlacePage";
import { renderWithRouter, screen, fireEvent } from "../test-utils";

test("renders add place form", () => {
  renderWithRouter(<AddPlacePage />);

  expect(
    screen.getByText(/add your place/i)
  ).toBeInTheDocument();

  expect(
    screen.getByPlaceholderText(/place name/i)
  ).toBeInTheDocument();
});

test("shows success message after submit", () => {
  renderWithRouter(<AddPlacePage />);

  fireEvent.change(screen.getByPlaceholderText(/place name/i), {
    target: { value: "Test Place" },
  });

  fireEvent.change(screen.getByRole("combobox"), {
    target: { value: "Cafe" },
  });

  fireEvent.change(
    screen.getByPlaceholderText(/why should people visit/i),
    { target: { value: "Nice place" } }
  );

  fireEvent.click(
    screen.getByRole("button", { name: /send to admin/i })
  );

  expect(
    screen.getByText(/thank you for submitting your place/i)
  ).toBeInTheDocument();
});
