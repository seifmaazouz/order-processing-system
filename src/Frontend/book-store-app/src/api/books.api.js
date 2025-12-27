import axios from "axios";

const BASE_URL = "http://localhost:8080/api/books";

export async function addBook(payload) {
  try {
    const response = await axios.post(BASE_URL, payload, {
      headers: {
        "Content-Type": "application/json",
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error adding book:", error);
    throw error;
  }
}

export async function editBook(isbn, updates) {
  try {
    const response = await axios.put(`${BASE_URL}/${isbn}`, updates, {
      headers: {
        "Content-Type": "application/json",
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error updating book:", error.response?.data || error.message);
    throw error;
  }
}

export async function removeBook(isbn) {
  try {
    const response = await axios.delete(`${BASE_URL}/${isbn}`);
    return response.data;
  } catch (error) {
    console.error("Error removing book:", error.response?.data || error.message);
    throw error;
  }
}
