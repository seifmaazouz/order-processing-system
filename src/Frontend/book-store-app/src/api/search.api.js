import axios from "axios";
import API_BASE_URL from '../config/api.config.js';

export async function searchBooks(query) {
  try {
    // Filter out empty/falsy values to avoid sending empty params
    const cleanParams = Object.fromEntries(
      Object.entries(query).filter(([_, value]) => value && value !== '')
    );

    console.log('Sending search request with params:', cleanParams);

    // Make GET request with query params
    const response = await axios.get(`${API_BASE_URL}/books/search`, {
      params: cleanParams, // Axios automatically converts this object into URL query parameters
      headers: {
        "Content-Type": "application/json", // optional for GET, but fine to include
      },
    });

    // Return the data from the response
    return response.data;
  } catch (error) {
    console.error("Error searching books:", error);
    throw error; // re-throw so the caller can handle it
  }
}