import axios from "axios";
export async function searchBooks(query) {
  try {
    // Make GET request with query params
    const response = await axios.get(`http://localhost:8080/api/books/search`, {
      params: query, // Axios automatically converts this object into URL query parameters
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