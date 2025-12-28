import React, { createContext, useContext, useEffect, useMemo, useState, useCallback } from 'react';
import { useLocation } from 'react-router-dom';
import { addCart, getCartItems, updateCartQuantity } from '../api/addCart.js';
import axios from 'axios';
import API_BASE_URL from '../config/api.config.js';


const CartContext = createContext(null);

export function CartProvider({ children }) {
  const [items, setItems] = useState([]);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const location = useLocation();

  // Reusable function to load cart items from backend
  const loadCart = useCallback(async () => {
    const token = localStorage.getItem('access');
    if (!token) {
      console.log('No auth token, skipping cart load');
      return;
    }

    try {
      console.log('Loading cart items from backend...');
      const cartData = await getCartItems();
      console.log('Raw cart data from backend:', cartData);
      
      // Handle different response formats
      // Backend returns: { CartId, Username, Items: [{ ISBN, Title, Authors, Quantity, UnitPrice, TotalPrice }], TotalPrice }
      let itemsArray = [];
      if (Array.isArray(cartData)) {
        itemsArray = cartData;
      } else if (cartData?.Items && Array.isArray(cartData.Items)) {
        itemsArray = cartData.Items;
      } else if (cartData?.items && Array.isArray(cartData.items)) {
        itemsArray = cartData.items;
      } else if (cartData?.cartItems && Array.isArray(cartData.cartItems)) {
        itemsArray = cartData.cartItems;
      }
      
      // If no items found, ensure we have an empty array
      if (!itemsArray || itemsArray.length === 0) {
        console.log('Cart is empty or no items found');
        setItems([]);
        return;
      }

      // Transform backend data to frontend format
      // Backend returns: { CartId, Username, Items: [{ ISBN, Title, Authors, Quantity, UnitPrice, TotalPrice }], TotalPrice }
      const formattedItems = itemsArray.map(item => ({
        id: item.ISBN || item.isbn || item.id || item.bookIsbn,
        title: item.Title || item.title || item.bookTitle || item.name || '',
        authors: Array.isArray(item.Authors) ? item.Authors : 
                 (item.Authors ? [item.Authors] : 
                 (Array.isArray(item.authors) ? item.authors : 
                 (item.authors ? [item.authors] : []))),
        price: parseFloat(item.UnitPrice || item.unitPrice || item.price || item.bookPrice || 0),
        stock: item.Stock || item.stock || item.stockLevel || item.availableStock || 0,
        quantity: parseInt(item.Quantity || item.quantity || 1, 10),
      }));
      
      setItems(formattedItems);
      console.log('Cart items loaded and formatted:', formattedItems);
    } catch (err) {
      console.error('Failed to load cart items:', err);
      console.error('Error details:', err.response?.data);
      // Don't set error state here to avoid showing error on initial load
    }
  }, []);

  // Load cart items on mount
  useEffect(() => {
    console.log('CartProvider mounted, loading cart...');
    loadCart();
  }, [loadCart]);

  // Reload cart when navigating to a new route (ensures cart is fresh)
  useEffect(() => {
    const token = localStorage.getItem('access');
    if (token) {
      console.log('Route changed, reloading cart...', location.pathname);
      loadCart();
    }
  }, [location.pathname, loadCart]);

  const addToCart = useCallback(async (book) => {
    if (!book) {
      console.error('CartContext: No book provided');
      setError('No book provided');
      return false;
    }
    
    setError(null);
    setIsLoading(true);

    try {
      // Call backend API to add to cart
      // Backend returns books with ISBN (PascalCase) or isbn (camelCase)
      // Also check normalized properties from Dashboard
      const isbn = book.ISBN || book.isbn || book.id || (book && typeof book === 'string' ? book : null);
      
      if (!isbn) {
        console.error('CartContext: Book missing ISBN:', book);
        console.error('CartContext: Book keys:', Object.keys(book || {}));
        setError('Book missing ISBN');
        setIsLoading(false);
        return false;
      }
      
      // Ensure ISBN is a string
      const isbnStr = String(isbn).trim();
      if (!isbnStr) {
        console.error('CartContext: Invalid ISBN:', isbn);
        setError('Invalid ISBN');
        setIsLoading(false);
        return false;
      }
      
      console.log('CartContext: Adding book to cart with ISBN:', isbnStr);
      await addCart({ isbn: isbnStr });
      
      // Reload cart from backend to ensure sync
      await loadCart();

      setIsLoading(false);
      return true;
    } catch (err) {
      console.error('CartContext: Error adding to cart:', err);
      console.error('CartContext: Error response:', err?.response?.data);
      console.error('CartContext: Error status:', err?.response?.status);
      const errorMsg = err?.response?.data?.message || err?.response?.data?.error || err?.message || 'Failed to add to cart';
      setError(errorMsg);
      setIsLoading(false);
      return false;
    }
  }, [loadCart]);

  const removeFromCart = useCallback(async (id) => {
    try {
      const { removeCartItem } = await import('../api/addCart.js');
      await removeCartItem(id);
      // Reload cart from backend to ensure sync
      await loadCart();
    } catch (error) {
      console.error('Failed to remove item:', error);
      setError(error?.message || 'Failed to remove item');
    }
  }, [loadCart]);

  const updateQuantity = useCallback(async (id, quantity) => {
    try {
      // Call API to update quantity on backend
      await updateCartQuantity(id, quantity);
      
      // Reload cart from backend to ensure sync
      await loadCart();
    } catch (error) {
      console.error('Failed to update quantity:', error);
      setError(error?.message || 'Failed to update quantity');
    }
  }, [loadCart]);

  const clearCart = useCallback(async () => {
    const token = localStorage.getItem('access');
    if (token) {
      try {
        // Clear cart on backend
        await axios.delete(`${API_BASE_URL}/shoppingcart`, {
          headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
          },
        });
      } catch (error) {
        console.error('Failed to clear cart on backend:', error);
      }
    }
    setItems([]);
  }, []);

  const summary = useMemo(() => {
    const totalItems = items.reduce((sum, item) => sum + item.quantity, 0);
    const totalPrice = items.reduce(
      (sum, item) => sum + Number(item.price || 0) * item.quantity,
      0
    );
    return { totalItems, totalPrice };
  }, [items]);

  const value = useMemo(
    () => ({ items, addToCart, removeFromCart, updateQuantity, clearCart, loadCart, summary, error, isLoading }),
    [items, addToCart, removeFromCart, updateQuantity, clearCart, loadCart, summary, error, isLoading]
  );

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
