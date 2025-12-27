import axios from "axios";
import API_BASE_URL from '../config/api.config.js';

// Get shopping cart items
export async function getCartItems() {
  console.log('Fetching cart items from backend');
  
  try {
    const token = localStorage.getItem('access');
    const response = await axios.get(
      `${API_BASE_URL}/shoppingcart/items`,
      {
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
      }
    );
    console.log('Fetch cart items success:', response.data);
    return response.data;
  } catch (error) {
    console.error('Fetch cart items error:', error);
    console.error('Error response:', error.response?.data);
    console.error('Error status:', error.response?.status);
    throw error;
  }
}

// Add item to cart (legacy function kept for compatibility)
export async function addCart(data) {
  const { isbn } = data;
  console.log('Adding to cart - ISBN:', isbn);
  
  try {
    const token = localStorage.getItem('access');
    const response = await axios.post(
      `${API_BASE_URL}/shoppingcart/add-item/${isbn}`,
      {},
      {
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
      }
    );
    console.log('Add to cart success:', response.data);
    return response;
  } catch (error) {
    console.error('Add to cart error:', error);
    console.error('Error response:', error.response?.data);
    console.error('Error status:', error.response?.status);
    console.error('Error message:', error.message);
    throw error;
  }
}

// Update cart item quantity
export async function updateCartQuantity(isbn, quantity) {
  console.log('Updating cart quantity - ISBN:', isbn, 'Quantity:', quantity);
  
  try {
    const token = localStorage.getItem('access');
    const response = await axios.put(
      `${API_BASE_URL}/shoppingcart/update-item/${isbn}`,
      {
        isbn: isbn,
        quantity: quantity
      },
      {
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
      }
    );
    console.log('Update quantity success:', response.data);
    return response;
  } catch (error) {
    console.error('Update quantity error:', error);
    console.error('Error response:', error.response?.data);
    console.error('Error status:', error.response?.status);
    console.error('Error message:', error.message);
    throw error;
  }
}