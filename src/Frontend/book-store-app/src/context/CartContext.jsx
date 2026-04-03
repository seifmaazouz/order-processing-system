import React, { createContext, useContext, useEffect, useMemo, useState, useCallback, useRef } from 'react';
import { useLocation } from 'react-router-dom';
import { addCart, getCartItems, updateCartQuantity, getCartItemCount } from '../api/addCart.js';
import { toast } from 'react-toastify';
import axios from 'axios';
import API_BASE_URL from '../config/api.config.js';


const CartContext = createContext(null);

export function CartProvider({ children }) {
  const [items, setItems] = useState([]);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [cartCount, setCartCount] = useState(0);
  const location = useLocation();
  const prevPathnameRef = useRef();

  // Load cart data (items and count) for navigation badge and persistence
  const loadCartData = useCallback(async () => {
    const token = localStorage.getItem('access');
    if (!token) {
      console.log('CartContext: No auth token available, clearing cart data');
      setCartCount(0);
      setItems([]); // Clear items when logged out
      return;
    }

    // Always load cart items for better UX and to derive count
    console.log('CartContext: Loading cart data for better UX and badge...');
    await loadCart();
  }, [location.pathname]);

  // Reusable function to load cart items from backend
  const loadCart = useCallback(async () => {
    const token = localStorage.getItem('access');
    if (!token) {
      console.log('CartContext: No auth token available, skipping cart items load');
      setItems([]);
      return;
    }

    // Prevent multiple simultaneous calls
    if (isLoading) {
      console.log('CartContext: Already loading cart items, skipping duplicate call');
      return;
    }

    try {
      console.log('CartContext: Loading cart items from backend...');
      setIsLoading(true);
      const cartData = await getCartItems();
      console.log('CartContext: Raw cart data received from backend:', cartData);
      
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
      
      // If no items found, ensure we have an empty array and zero count
      if (!itemsArray || itemsArray.length === 0) {
        console.log('CartContext: Cart is empty or no items found in response');
        setItems([]);
        setCartCount(0);
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
      // Set cartCount to total quantity, not just unique items
      setCartCount(formattedItems.reduce((sum, item) => sum + (item.quantity || 0), 0));
      console.log(`CartContext: Cart items loaded and formatted (${formattedItems.length} items):`, formattedItems);
    } catch (err) {
      console.error('CartContext: Failed to load cart items:', err);
      console.error('CartContext: Error details:', err.response?.data);
      setItems([]); // Clear items on error
      setCartCount(0); // Reset count on error
      // Don't set error state here to avoid showing error on initial load
    } finally {
      setIsLoading(false);
    }
  }, []);

  // Load cart data on mount and route changes (for navigation badge and persistence)
  useEffect(() => {
    // Only load cart data if pathname actually changed (prevents duplicate calls)
    if (prevPathnameRef.current !== location.pathname) {
      console.log(`CartContext: Route changed to ${location.pathname}, refreshing cart data for navigation badge`);
      loadCartData();
      prevPathnameRef.current = location.pathname;
    }
  }, [location.pathname]); // loadCartData includes location.pathname in deps

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
      

      console.log(`CartContext: Adding book to cart (ISBN: ${isbnStr})`);
      await addCart({ isbn: isbnStr });

      // Optimistic UI: update local items and cartCount immediately
      setItems(prev => {
        const found = prev.find(i => i.id === isbnStr);
        if (found) {
          return prev.map(i => i.id === isbnStr ? { ...i, quantity: (i.quantity || 0) + 1, stock: Math.max(0, (i.stock || 0) - 1) } : i);
        }
        // If item not present, we cannot construct full item details here; reload authoritative cart
        return prev;
      });
      setCartCount(c => c + 1);

      // Reload authoritative cart data to reconcile state
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
      console.log(`CartContext: Removing item ${id} from cart`);
      // determine previous quantity so we can adjust stock
      const prevItem = items.find(i => i.id === id);
      const prevQty = prevItem?.quantity || 0;

      const { removeCartItem } = await import('../api/addCart.js');
      await removeCartItem(id);

      // Reload cart data to reflect the removal
      console.log('CartContext: Reloading cart data after item removal');
      await loadCart();
      // Optimistic UI: adjust local items and cartCount (will reconcile after loadCart)
      setItems(prev => prev.filter(i => i.id !== id));
      setCartCount(c => Math.max(0, c - prevQty));
    } catch (error) {
      console.error('CartContext: Failed to remove item:', error);
      setError(error?.message || 'Failed to remove item');
    }
  }, [loadCart, items]);

  const updateQuantity = useCallback(async (id, quantity) => {
    try {
      console.log(`CartContext: Updating item ${id} quantity to ${quantity}`);
      // find previous quantity for delta
      const prevItem = items.find(i => i.id === id);
      const prevQty = prevItem?.quantity || 0;

      // Call API to update quantity on backend
      await updateCartQuantity(id, quantity);

      // Reload cart data to reflect the quantity change
      console.log('CartContext: Reloading cart data after quantity update');
      await loadCart();
      // Optimistic UI: adjust local items and cartCount
      const delta = quantity - prevQty;
      if (delta !== 0) {
        setItems(prev => prev.map(i => i.id === id ? { ...i, quantity, stock: Math.max(0, (i.stock || 0) - delta) } : i));
        setCartCount(c => Math.max(0, c + delta));
      }
    } catch (error) {
      console.error('CartContext: Failed to update quantity:', error);
      const data = error?.response?.data ?? {};
      const message = data?.message || data?.error || error?.message || 'Failed to update quantity';

      // If backend returned structured insufficient-stock info, show an error toast and reload authoritative cart
      const isInsufficient = data?.error === 'Insufficient stock' || (data?.isbn && typeof data?.available !== 'undefined');
      if (isInsufficient) {
        const isbn = data.isbn ?? id;
        const available = typeof data.available !== 'undefined' ? Number(data.available) : null;
        const title = data.title ?? null;
        const displayName = title ?? isbn;
        if (available === 0) {
          toast.error(`${displayName} is out of stock and was removed from your cart.`);
        } else if (available !== null) {
          toast.error(`Insufficient stock for ${displayName}. Quantity adjusted to ${available}.`);
        } else {
          toast.error(message);
        }
        try {
          await loadCart();
        } catch (reloadErr) {
          console.error('CartContext: Failed to reload cart after insufficient-stock error:', reloadErr);
        }
      } else {
        // Generic failure
        toast.error(message);
      }

      setError(message);
    }
  }, [loadCart, items]);

  // Batch adjust multiple cart items (used when backend reports insufficient stock for multiple items)
  const adjustCartItems = useCallback(async (insufficientItems = []) => {
    if (!Array.isArray(insufficientItems) || insufficientItems.length === 0) return;
    const messages = [];

    for (const it of insufficientItems) {
      const isbn = it.isbn ?? it.id;
      const available = typeof it.available !== 'undefined' ? Number(it.available) : null;
      const title = it.title ?? null;
      const displayName = title ?? isbn;

      try {
        if (available === null) {
          // No actionable info, skip
          messages.push(`Could not determine stock for ${displayName}. Please review your cart.`);
          continue;
        }

        if (available <= 0) {
          const { removeCartItem } = await import('../api/addCart.js');
          await removeCartItem(isbn);
          // Optimistic: remove from local items and update count
          const prev = items.find(i => i.id === isbn);
          const prevQty = prev?.quantity || 0;
          setItems(prevItems => prevItems.filter(i => i.id !== isbn));
          setCartCount(c => Math.max(0, c - prevQty));
          messages.push(`${displayName} was removed — out of stock.`);
        } else {
          const { updateCartQuantity } = await import('../api/addCart.js');
          await updateCartQuantity(isbn, available);
          const prev = items.find(i => i.id === isbn);
          const prevQty = prev?.quantity || 0;
          const delta = available - prevQty;
          if (delta !== 0) {
            setItems(prevItems => prevItems.map(i => i.id === isbn ? { ...i, quantity: available, stock: Math.max(0, (i.stock || 0) - delta) } : i));
            setCartCount(c => Math.max(0, c + delta));
          }
          messages.push(`Insufficient stock for ${displayName}. Quantity adjusted to ${available}.`);
        }
      } catch (err) {
        console.error('CartContext: Failed to adjust item', isbn, err);
        messages.push(`Failed to adjust ${displayName}. Please review your cart.`);
      }
    }

    try {
      await loadCart();
    } catch (err) {
      console.error('CartContext: Failed to reload cart after adjustments', err);
    }

    // Show notifications for each message
    for (const m of messages) toast.error(m);
  }, [items, loadCart]);

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
    setCartCount(0);
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
    () => ({ items, addToCart, removeFromCart, updateQuantity, clearCart, loadCart, adjustCartItems, summary, error, isLoading, cartCount, loadCartData }),
    [items, addToCart, removeFromCart, updateQuantity, clearCart, loadCart, adjustCartItems, summary, error, isLoading, cartCount, loadCartData]
  );

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const ctx = useContext(CartContext);
  if (!ctx) throw new Error('useCart must be used within CartProvider');
  return ctx;
}
