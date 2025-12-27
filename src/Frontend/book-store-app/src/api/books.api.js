import axios from "axios";
import API_BASE_URL from '../config/api.config.js';

const BOOKS_URL = `${API_BASE_URL}/books`;

export async function addBook(payload) {
  try {
    const token = localStorage.getItem('access');
    const response = await axios.post(BOOKS_URL, payload, {
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
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
    const token = localStorage.getItem('access');
    const response = await axios.put(`${BOOKS_URL}/${isbn}`, updates, {
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
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
    const token = localStorage.getItem('access');
    const response = await axios.delete(`${BOOKS_URL}/${isbn}`, {
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token}`
      },
    });
    return response.data;
  } catch (error) {
    console.error("Error removing book:", error.response?.data || error.message);
    throw error;
  }
}
