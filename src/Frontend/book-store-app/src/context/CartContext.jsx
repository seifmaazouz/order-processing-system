import React, { createContext, useContext, useMemo, useState } from 'react';
import { updateCartQuantity } from '../api/addCart.js';


const CartContext = createContext(null);

export function CartProvider({ children }) {
  const [items, setItems] = useState([]);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);

  const addToCart = async (book) => {
    if (!book) return;
    setError(null);
    setIsLoading(true);

    try {
      // Add to cart directly
      setItems((prev) => {
        const existing = prev.find((item) => item.id === (book.id ?? book.isbn));
        if (existing) {
          return prev.map((item) =>
            item.id === (book.id ?? book.isbn)
              ? { ...item, quantity: item.quantity + 1 }
              : item
          );
        }
        return [
          ...prev,
          {
            id: book.id ?? book.isbn ?? crypto.randomUUID?.() ?? Date.now(),
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
      const errorMsg = err?.message || 'Failed to add to cart';
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
    () => ({ items, addToCart, removeFromCart, updateQuantity, clearCart, summary, error, isLoading }),
    [items, summary, error, isLoading]
  );

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
