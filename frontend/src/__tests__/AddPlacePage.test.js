import AddPlacePage from "../pages/AddPlacePage";
import { renderWithRouter, screen } from "../test-utils";

test("renders add place form fields", () => {
  renderWithRouter(<AddPlacePage />);

  // Place name input
  expect(
    screen.getByPlaceholderText("Place Name")
  ).toBeInTheDocument();

  // Category dropdown
  expect(
    screen.getByDisplayValue("Select Category")
  ).toBeInTheDocument();

  // Description textarea
  expect(
    screen.getByPlaceholderText(
      "Why should people visit this place?"
    )
  ).toBeInTheDocument();

  // Submit button
  expect(
    screen.getByText("Send to Admin")
  ).toBeInTheDocument();
});
