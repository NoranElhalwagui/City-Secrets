import ExplorePage from "../pages/ExplorePage";
import { renderWithRouter, screen } from "../test-utils";

const mockPosts = [];

test("renders Explore page and main action buttons", () => {
  renderWithRouter(
    <ExplorePage posts={mockPosts} setPosts={() => {}} currentUser="User1" />
  );

  expect(
    screen.getByText(/explore places/i)
  ).toBeInTheDocument();

  expect(
    screen.getByRole("button", { name: /make your own post/i })
  ).toBeInTheDocument();

  expect(
    screen.getByRole("button", { name: /add your own place/i })
  ).toBeInTheDocument();
});
