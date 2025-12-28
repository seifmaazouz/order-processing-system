import axios from "axios";
import API_BASE_URL from '../config/api.config.js';

// Get shopping cart items
export async function getCartItems() {
  console.log('Fetching cart items from backend');
  
  try {
    const token = localStorage.getItem('access');
    if (!token) {
      console.warn('No access token found, returning empty cart');
      return { Items: [], TotalPrice: 0, CartId: 0, Username: '' };
    }
    
    const response = await axios.get(
      `${API_BASE_URL}/shoppingcart`,
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
    // If 401/403, user is not authenticated - return empty cart
    if (error.response?.status === 401 || error.response?.status === 403) {
      console.warn('Unauthorized access to cart, returning empty cart');
      return { Items: [], TotalPrice: 0, CartId: 0, Username: '' };
    }
    console.error('Fetch cart items error:', error);
    console.error('Error response:', error.response?.data);
    console.error('Error status:', error.response?.status);
    // Return empty cart on error instead of throwing to prevent UI crashes
    return { Items: [], TotalPrice: 0, CartId: 0, Username: '' };
  }
}

// Add item to cart (legacy function kept for compatibility)
export async function addCart(data) {
  const { isbn } = data;
  console.log('Adding to cart - ISBN:', isbn);
  
  try {
    const token = localStorage.getItem('access');
    if (!token) {
      throw new Error('No access token found. Please login first.');
    }
    
    // URL encode ISBN to handle special characters like hyphens
    const encodedIsbn = encodeURIComponent(isbn);
    const response = await axios.post(
      `${API_BASE_URL}/shoppingcart/add-item/${encodedIsbn}`,
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
    if (!token) {
      throw new Error('No access token found. Please login first.');
    }
    
    // URL encode ISBN to handle special characters like hyphens
    const encodedIsbn = encodeURIComponent(isbn);
    const response = await axios.put(
      `${API_BASE_URL}/shoppingcart/update-item/${encodedIsbn}`,
      quantity,
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

// Remove item from cart
export async function removeCartItem(isbn) {
  try {
    const token = localStorage.getItem('access');
    if (!token) {
      throw new Error('No access token found. Please login first.');
    }
    
    // URL encode ISBN to handle special characters like hyphens
    const encodedIsbn = encodeURIComponent(isbn);
    const response = await axios.delete(
      `${API_BASE_URL}/shoppingcart/remove-item/${encodedIsbn}`,
      {
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
      }
    );
    return response;
  } catch (error) {
    console.error('Remove cart item error:', error);
    console.error('Error response:', error.response?.data);
    throw error;
  }
}

// Checkout cart
export async function checkoutCart(cardNumber, expiryDate) {
  try {
    const token = localStorage.getItem('access');
    // Backend expects: { CardNumber: long, ExpiryDate: DateTime }
    // expiryDate can be YYYY-MM-DD string or ISO string - ASP.NET Core will parse it
    const response = await axios.post(
      `${API_BASE_URL}/shoppingcart/checkout`,
      {
        CardNumber: cardNumber,
        ExpiryDate: expiryDate // Send as string (YYYY-MM-DD or ISO), backend will parse to DateTime
      },
      {
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
      }
    );
    return response.data;
  } catch (error) {
    console.error('Checkout error:', error);
    console.error('Checkout error response:', error.response?.data);
    throw error;
  }
}