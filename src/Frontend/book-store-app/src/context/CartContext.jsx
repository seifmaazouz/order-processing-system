import React, { createContext, useContext, useEffect, useMemo, useState, useCallback } from 'react';
import { addCart, getCartItems, updateCartQuantity } from '../api/addCart.js';


const CartContext = createContext(null);

export function CartProvider({ children }) {
  const [items, setItems] = useState([]);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

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
      let itemsArray = [];
      if (Array.isArray(cartData)) {
        itemsArray = cartData;
      } else if (cartData?.items && Array.isArray(cartData.items)) {
        itemsArray = cartData.items;
      } else if (cartData?.cartItems && Array.isArray(cartData.cartItems)) {
        itemsArray = cartData.cartItems;
      }

      // Transform backend data to frontend format
      const formattedItems = itemsArray.map(item => ({
        id: item.isbn || item.id || item.bookIsbn,
        title: item.title || item.bookTitle || item.name,
        authors: item.authors || item.author || [],
        price: parseFloat(item.price || item.unitPrice || item.bookPrice || 0),
        stock: item.stock || item.stockLevel || item.availableStock || 0,
        quantity: parseInt(item.quantity || 1, 10),
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

  const addToCart = async (book) => {
    if (!book) return;
    setError(null);
    setIsLoading(true);

    try {
      // Call backend API to add to cart
      const isbn = book.id ?? book.isbn;
      console.log('CartContext: Adding book to cart with ISBN:', isbn);
      await addCart({ isbn });
      
      // Update local state after successful API call
      setItems((prev) => {
        const existing = prev.find((item) => item.id === isbn);
        if (existing) {
          return prev.map((item) =>
            item.id === isbn
              ? { ...item, quantity: item.quantity + 1 }
              : item
          );
        }
        return [
          ...prev,
          {
            id: isbn,
            title: book.title,
            authors: book.authors,
            price: book.price || 0,
            stock: book.stock ?? book.stockLevel ?? 0,
            quantity: 1,
          },
        ];
      });

      setIsLoading(false);
      return true;
    } catch (err) {
      console.error('CartContext: Error adding to cart:', err);
      const errorMsg = err?.response?.data?.message || err?.message || 'Failed to add to cart';
      setError(errorMsg);
      setIsLoading(false);
      return false;
    }
  };

  const removeFromCart = (id) => {
    setItems((prev) => prev.filter((item) => item.id !== id));
  };

  const updateQuantity = async (id, quantity) => {
    try {
      // Call API to update quantity on backend
      await updateCartQuantity(id, quantity);
      
      // Update local state
      setItems((prev) =>
        prev.map((item) =>
          item.id === id ? { ...item, quantity: Math.max(1, quantity) } : item
        )
      );
    } catch (error) {
      console.error('Failed to update quantity:', error);
      setError(error?.message || 'Failed to update quantity');
    }
  };

  const clearCart = () => setItems([]);

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
    [items, summary, error, isLoading]
  );

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
