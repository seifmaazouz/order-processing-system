import axios from "axios";

// Check if the requested quantity is available
export async function checkCartAvailability(bookId, quantity) {
  try {
    const response = await axios.post(
      `http://localhost:8080/api/cart/check-availability`,
      {
        bookId,
        quantity,
      },
      {
        headers: {
          "Content-Type": "application/json",
        },
      }
    );
    return response.data; // Expected: { ok: true/false, message?: string }
  } catch (error) {
    throw error.response?.data || { ok: false, message: "Failed to check availability" };
  }
}

// Add item to cart (legacy function kept for compatibility)
export async function addCart(data) {
  const token = await axios.post(`http://localhost:8080/api/user/login`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });
  return token;
}