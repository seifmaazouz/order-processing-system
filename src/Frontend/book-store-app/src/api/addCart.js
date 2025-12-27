import axios from "axios";
import API_BASE_URL from '../config/api.config.js';

// Check if the requested quantity is available
export async function checkCartAvailability(bookId, quantity) {
  try {
    const response = await axios.post(
      `${API_BASE_URL}/cart/check-availability`,
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
  const token = await axios.post(`${API_BASE_URL}/user/login`, data, {
    headers: {
      "Content-Type": "application/json",
    },
  });
  return token;
}