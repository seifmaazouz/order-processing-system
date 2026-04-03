import React, { useState, useRef, useEffect } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowLeft, faTrash, faHome } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../../context/CartContext.jsx';
import DashboardHeader from '../../components/dashboard/DashboardHeader.jsx';
import { checkoutCart } from '../../api/addCart.js';
import { getAccountDetails } from '../../api/accountDetails.api.js';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export default function Cart() {
  const navigate = useNavigate();
  const { items, summary, cartCount, removeFromCart, updateQuantity, clearCart, loadCart } = useCart();
  const SHIPPING_PERCENT = 0.05; // 5% shipping fee
  const shippingFee = summary.totalPrice * SHIPPING_PERCENT;
  const totalWithShipping = summary.totalPrice + shippingFee;
  const [showSettings, setShowSettings] = useState(false);
  const [showCheckoutModal, setShowCheckoutModal] = useState(false);
  const [isCheckingOut, setIsCheckingOut] = useState(false);
  const [cardholderName, setCardholderName] = useState('');
  const [shippingAddress, setShippingAddress] = useState('');
  const [userAddress, setUserAddress] = useState('');
  const [paymentMethod, setPaymentMethod] = useState('saved'); // 'saved' or 'new'
  const [selectedSavedCard, setSelectedSavedCard] = useState('');
  const [savedCards, setSavedCards] = useState([]);
  const [cardNumber, setCardNumber] = useState('');
  const [expiryDate, setExpiryDate] = useState('');
  const settingsRef = useRef(null);

  useEffect(() => {
    function handleClick(e) {
      if (settingsRef.current && !settingsRef.current.contains(e.target)) {
        setShowSettings(false);
      }
    }
    document.addEventListener('click', handleClick);
    return () => document.removeEventListener('click', handleClick);
  }, []);

  // Reload cart when component mounts
  useEffect(() => {
    console.log('Cart component: Loading cart items on mount');
    loadCart();
  }, []); // Only load once on mount

  const [stockOverrides, setStockOverrides] = useState({});

  // Listen for cart quantity changes to update displayed stock immediately
  useEffect(() => {
    function handleCartQuantityChange(e) {
      const detail = e?.detail || {};
      const { id, delta } = detail;
      if (!id || typeof delta !== 'number') return;

      // Only update overrides for items currently in the cart
      setStockOverrides(prev => {
        const current = prev[id] ?? (items.find(i => i.id === id)?.stock ?? 0);
        const updated = Math.max(0, current - delta);
        return { ...prev, [id]: updated };
      });
    }

    window.addEventListener('cart:quantityChanged', handleCartQuantityChange);
    return () => window.removeEventListener('cart:quantityChanged', handleCartQuantityChange);
  }, [items]);

  // Reset overrides when items list reloads to keep authoritative data
  useEffect(() => {
    // Recompute overrides based on authoritative `items` (stock - quantity)
    const map = {};
    (items || []).forEach(i => {
      const base = Number(i.stock ?? 0);
      const qty = Number(i.quantity ?? 0);
      map[i.id] = Math.max(0, base - qty);
    });
    setStockOverrides(map);
  }, [items]);

  // Fetch saved cards and user address when checkout modal opens
  useEffect(() => {
    if (showCheckoutModal) {
      fetchSavedCards();
    }
  }, [showCheckoutModal]);

  // Fetch user address on mount
  useEffect(() => {
    const fetchUserAddress = async () => {
      try {
        const token = localStorage.getItem('access');
        const userDetails = await getAccountDetails(token);
        setUserAddress(userDetails.address || '');
      } catch (error) {
        setUserAddress('');
      }
    };
    fetchUserAddress();
  }, []);

  // When savedCards are loaded and payment method is 'saved', select the first card by default
  // No longer auto-select the first saved card when payment method is 'saved'

  const fetchSavedCards = async () => {
    try {
      const token = localStorage.getItem('access');
      const userDetails = await getAccountDetails(token);
      setSavedCards(userDetails.creditCards || []);
    } catch (error) {
      console.error('Failed to fetch saved cards:', error);
      setSavedCards([]);
    }
  };

  const handleQtyChange = (id, delta) => {
    const item = items.find((i) => i.id === id);
    if (!item) return;
    updateQuantity(id, Math.max(1, item.quantity + delta));
  };

  const handleCheckout = async () => {
    // Validate cardholder name (only required for new cards)
    if (paymentMethod === 'new' && !cardholderName.trim()) {
      toast.error('Please enter cardholder name');
      return;
    }

    // Validate payment method
    if (paymentMethod === 'saved') {
      if (!selectedSavedCard) {
        toast.error('Please select a saved card');
        return;
      }
    } else if (paymentMethod === 'new') {
      if (!cardNumber || !expiryDate) {
        toast.error('Please enter credit card information');
        return;
      }
    } else {
      toast.error('Please select a payment method');
      return;
    }

    setIsCheckingOut(true);
    try {
      let savedCardNumber = null;
      let newCardNumber = null;
      let newCardExpiry = null;

      if (paymentMethod === 'saved') {
        savedCardNumber = parseInt(selectedSavedCard);
        if (isNaN(savedCardNumber)) {
          toast.error('Invalid saved card selection');
          setIsCheckingOut(false);
          return;
        }
      } else {
        // Convert expiry date from MM/YY to Date string (last day of the month) in YYYY-MM-DD format
        const [month, year] = expiryDate.split('/');
        const expiryYear = 2000 + parseInt(year);
        const expiryMonth = parseInt(month);
        // Get last day of the month
        const lastDay = new Date(expiryYear, expiryMonth, 0).getDate();
        // Format as YYYY-MM-DD for better compatibility
        newCardExpiry = `${expiryYear}-${String(expiryMonth).padStart(2, '0')}-${String(lastDay).padStart(2, '0')}`;

        newCardNumber = parseInt(cardNumber.replace(/\s/g, ''));
        if (isNaN(newCardNumber)) {
          toast.error('Invalid card number');
          setIsCheckingOut(false);
          return;
        }
      }

      const cardholderNameToSend = paymentMethod === 'new' ? cardholderName.trim() : null;
      await checkoutCart(cardholderNameToSend, null, savedCardNumber, newCardNumber, newCardExpiry);
      toast.success('Order placed successfully!');
      clearCart();
      setShowCheckoutModal(false);
      resetCheckoutForm();
      await loadCart(); // Refresh cart
      navigate('/orders');
    } catch (error) {
      // Robust handling for insufficient-stock errors.
      // Attempt to parse structured response from backend, but fall back to message matching.
      const data = error?.response?.data ?? {};
      const message = data?.message || data?.error || error?.message || '';

      const isInsufficient = (data?.error === 'Insufficient stock')
        || (typeof message === 'string' && message.toLowerCase().includes('insufficient'))
        || (data?.isbn && typeof data?.available !== 'undefined');

      console.warn('Checkout failed:', { isInsufficient, message, data });

      if (isInsufficient) {
        // Ensure user returns to cart view immediately
        try {
          setShowCheckoutModal(false);
        } catch (e) {
          console.warn('Failed to close checkout modal:', e);
        }

        // If backend returned structured insufficient-stock error, auto-adjust cart to the provided available quantity
        if (data?.error === 'Insufficient stock' && data?.isbn !== undefined && typeof data?.available !== 'undefined') {
          const isbn = data.isbn;
          const available = Number(data.available);
          const title = data.title ?? null;
          const displayName = title ?? isbn;
          try {
            if (available <= 0) {
              await removeFromCart(isbn);
              toast.error(`${displayName} is out of stock and was removed from your cart.`);
            } else {
              await updateQuantity(isbn, available);
              toast.error(`Insufficient stock for ${displayName}. Quantity adjusted to ${available}.`);
            }
          } catch (adjErr) {
            console.error('Failed to auto-adjust cart after checkout failure:', adjErr);
            toast.error('Checkout failed and auto-adjust failed. Please review your cart.');
          }
        } else {
          // Non-structured insufficient message: notify user and reload authoritative cart state
          toast.error(message || 'Insufficient stock — your cart was updated.');
        }

        // Refresh cart to reflect adjustments or authoritative state regardless of auto-adjust outcome
        try {
          await loadCart();
        } catch (reloadErr) {
          console.error('Failed to reload cart after checkout failure:', reloadErr);
        }
      } else {
        const errorMsg = message || 'Checkout failed';
        toast.error(errorMsg);
      }
    } finally {
      setIsCheckingOut(false);
    }
  };

  const resetCheckoutForm = () => {
    setCardholderName('');
    setPaymentMethod('new');
    setSelectedSavedCard('');
    setCardNumber('');
    setExpiryDate('');
  };

  return (
    <div className="min-h-screen bg-background-light text-text-main">
      <DashboardHeader
        showSettings={showSettings}
        onToggleSettings={setShowSettings}
        settingsRef={settingsRef}
        cartTotal={cartCount}
      />
      <div className="px-4 sm:px-8 py-8">
      <div className="max-w-5xl mx-auto">
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <button
              onClick={() => navigate('/dashboard')}
              className="flex items-center gap-2 px-4 py-2 text-sm font-semibold bg-primary text-white rounded-full hover:bg-primary/90 transition-colors"
            >
              <FontAwesomeIcon icon={faHome} /> Back to Home
            </button>
            <button
              onClick={() => navigate('/dashboard')}
              className="flex items-center gap-2 text-sm font-semibold text-primary hover:text-primary/80"
            >
              <FontAwesomeIcon icon={faArrowLeft} /> Back to shop
            </button>
          </div>
          {items.length > 0 && (
            <button
              onClick={clearCart}
              className="text-base font-bold text-white bg-red-600 hover:bg-red-700 rounded-full px-6 py-2 shadow transition-colors duration-150"
              style={{ minWidth: '120px' }}
            >
              Clear Cart
            </button>
          )}
        </div>

        <h1 className="text-3xl font-bold mb-4">Your Cart</h1>
            <p className="text-gray-600 mb-8">Review items before checkout.</p>

        {items.length === 0 ? (
          <div className="rounded-xl border border-dashed border-gray-300 bg-white p-8 text-center">
            <p className="text-lg font-semibold mb-2">Your cart is empty</p>
            <p className="text-gray-600 mb-4">Add books from the dashboard to see them here.</p>
            <button
              onClick={() => navigate('/dashboard')}
              className="px-5 py-2 rounded-full bg-primary text-white font-semibold hover:bg-primary/90"
            >
              Continue shopping
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <div className="lg:col-span-2 space-y-4">
              {items.map((item) => {
                const authors = Array.isArray(item.authors) ? item.authors.join(', ') : item.authors;
                return (
                  <div
                    key={item.id}
                    className="flex items-start gap-4 rounded-xl border border-gray-200 bg-white p-4 shadow-sm"
                  >
                    <div className="flex-1">
                      <div className="flex items-center gap-2 mb-1">
                        <h3 className="font-semibold text-lg m-0 p-0">{item.title}</h3>
                        {item.id && (
                          <span className="text-xs text-gray-400">ISBN: {item.id}</span>
                        )}
                      </div>
                      {authors && (
                        <p className="text-sm text-gray-600 mb-1">{authors}</p>
                      )}
                      <p className="text-sm text-gray-500">Stock: {stockOverrides[item.id] ?? item.stock ?? 'N/A'}</p>
                      <div className="mt-3 flex items-center gap-3">
                        <div className="flex items-center gap-2 bg-gray-100 rounded-full px-3 py-1">
                          <button
                            onClick={() => handleQtyChange(item.id, -1)}
                            className="text-lg font-bold px-2"
                            disabled={item.quantity <= 1}
                          >
                            -
                          </button>
                          <span className="text-sm font-semibold">{item.quantity}</span>
                          <button
                            onClick={() => handleQtyChange(item.id, 1)}
                            className="text-lg font-bold px-2"
                          >
                            +
                          </button>
                        </div>
                        <p className="text-sm font-semibold text-gray-800">
                          ${(Number(item.price) * item.quantity).toFixed(2)}
                        </p>
                      </div>
                    </div>
                    <button
                      onClick={() => removeFromCart(item.id)}
                      className="text-red-600 hover:text-red-500"
                      aria-label="Remove"
                    >
                      <FontAwesomeIcon icon={faTrash} />
                    </button>
                  </div>
                );
              })}
            </div>

            <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm flex flex-col gap-4 min-h-[240px] max-h-[320px] justify-between sticky top-40 w-full z-20">
              <h2 className="text-xl font-bold mb-2">Order Summary</h2>
              <div className="flex flex-col gap-3 text-sm mb-4">
                <div className="flex items-center justify-between">
                  <span>Items in Cart:</span>
                  <span className="font-semibold">{summary.totalItems}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span>Unique Products:</span>
                  <span className="font-semibold">{items.length}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span>Subtotal:</span>
                  <span className="font-semibold">${summary.totalPrice.toFixed(2)}</span>
                </div>
                <div className="flex items-center justify-between">
                  <span>Shipping Fee (5%):</span>
                  <span className="font-semibold">${shippingFee.toFixed(2)}</span>
                </div>
                <div className="flex items-center justify-between border-t pt-2">
                  <span>Total:</span>
                  <span className="font-bold text-lg">${totalWithShipping.toFixed(2)}</span>
                </div>
                <div className="flex items-center justify-between text-xs text-gray-500 mt-2">
                  <span>Shipping Address:</span>
                  <span className="truncate max-w-[60%] text-right" title={userAddress || 'No shipping address set'}>
                    {userAddress || 'No shipping address set'}
                  </span>
                </div>
              </div>
              <button
                onClick={() => setShowCheckoutModal(true)}
                className="mt-6 w-full rounded-full bg-primary text-white font-semibold py-3 hover:bg-primary/90 text-lg shadow-md"
              >
                Proceed to Checkout
              </button>
            </div>
          </div>
        )}
      </div>
      </div>

      {/* Checkout Modal */}
      {showCheckoutModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm px-4">
          <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-2xl border border-gray-200">
            <h3 className="text-xl font-bold mb-4 text-text-main">Checkout</h3>
            <div className="space-y-4 mb-6">
              {/* Cardholder Name (only for new cards) */}
              {paymentMethod === 'new' && (
                <div>
                  <label className="block text-sm font-semibold mb-2">Cardholder Name</label>
                  <input
                    type="text"
                    value={cardholderName}
                    onChange={(e) => setCardholderName(e.target.value)}
                    placeholder="John Doe"
                    className="w-full h-10 px-3 rounded-md bg-white border border-gray-200 focus:ring-2 focus:ring-primary outline-none"
                  />
                </div>
              )}

              {/* Payment Method Selection */}
              <div>
                <label className="block text-sm font-semibold mb-2">Payment Method</label>
                <div className="space-y-2">
                  <label className="flex items-center">
                    <input
                      type="radio"
                      name="paymentMethod"
                      value="saved"
                      checked={paymentMethod === 'saved'}
                      onChange={(e) => setPaymentMethod(e.target.value)}
                      className="mr-2"
                    />
                    <span className="text-sm">Use saved card</span>
                  </label>
                  <label className="flex items-center">
                    <input
                      type="radio"
                      name="paymentMethod"
                      value="new"
                      checked={paymentMethod === 'new'}
                      onChange={(e) => setPaymentMethod(e.target.value)}
                      className="mr-2"
                    />
                    <span className="text-sm">Enter new card</span>
                  </label>
                </div>
              </div>

              {/* Saved Cards Selection */}
              {paymentMethod === 'saved' && (
                <div>
                  <label className="block text-sm font-semibold mb-2">Select Saved Card</label>
                  <select
                    value={selectedSavedCard}
                    onChange={(e) => setSelectedSavedCard(e.target.value)}
                    className="w-full h-10 px-3 rounded-md bg-white border border-gray-200 focus:ring-2 focus:ring-primary outline-none"
                  >
                    <option value="">Choose a card...</option>
                    {savedCards.map((card, index) => (
                      <option key={index} value={card.cardNumber}>
                        **** **** **** {card.cardNumber.toString().slice(-4)} (Expires {card.expiryMonth}/{card.expiryYear})
                      </option>
                    ))}
                  </select>
                  {savedCards.length === 0 && (
                    <p className="text-sm text-gray-500 mt-1">No saved cards found. Add a card in your account settings.</p>
                  )}
                </div>
              )}

              {/* New Card Details */}
              {paymentMethod === 'new' && (
                <>
                  <div>
                    <label className="block text-sm font-semibold mb-2">Card Number</label>
                    <input
                      type="text"
                      value={cardNumber}
                      onChange={(e) => {
                        const value = e.target.value.replace(/\D/g, '');
                        const formatted = value.match(/.{1,4}/g)?.join(' ') || value;
                        setCardNumber(formatted.slice(0, 19));
                      }}
                      placeholder="1234 5678 9012 3456"
                      className="w-full h-10 px-3 rounded-md bg-white border border-gray-200 focus:ring-2 focus:ring-primary outline-none"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold mb-2">Expiry Date (MM/YY)</label>
                    <input
                      type="text"
                      value={expiryDate}
                      onChange={(e) => {
                        const value = e.target.value.replace(/\D/g, '');
                        if (value.length <= 4) {
                          const formatted = value.length >= 2 ? `${value.slice(0, 2)}${value.slice(2)}` : value;
                          // Add slash automatically when 2+ digits
                          if (formatted.length > 2) {
                            const month = formatted.slice(0, 2);
                            const year = formatted.slice(2);
                            setExpiryDate(`${month}/${year}`);
                          } else {
                            setExpiryDate(formatted);
                          }
                        }
                      }}
                      onBlur={() => {
                        // Validate on blur
                        if (expiryDate && !/^(0[1-9]|1[0-2])\/\d{2}$/.test(expiryDate)) {
                          // Could add error state here
                        } else if (expiryDate) {
                          const [month, year] = expiryDate.split('/');
                          const expiryMonth = parseInt(month);
                          const expiryYear = 2000 + parseInt(year);
                          const now = new Date();
                          const expiryDateObj = new Date(expiryYear, expiryMonth - 1);
                          if (expiryDateObj < now) {
                            // Could add expired error here
                          }
                        }
                      }}
                      placeholder="MM/YY"
                      maxLength="5"
                      className="w-full h-10 px-3 rounded-md bg-white border border-gray-200 focus:ring-2 focus:ring-primary outline-none"
                    />
                  </div>
                </>
              )}

              {/* Order Summary */}
              <div className="bg-gray-50 p-4 rounded-lg">
                <div className="flex justify-between text-sm mb-2">
                  <span>Total:</span>
                  <span className="font-bold text-primary">${summary.totalPrice.toFixed(2)}</span>
                </div>
              </div>
            </div>
            <div className="flex justify-end gap-3">
              <button
                onClick={() => {
                  setShowCheckoutModal(false);
                  resetCheckoutForm();
                }}
                className="px-4 py-2 rounded-full border border-gray-300 bg-white text-sm font-semibold hover:border-primary hover:text-primary transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleCheckout}
                disabled={isCheckingOut ||
                  (paymentMethod === 'new' && (!cardholderName.trim() || !cardNumber || !expiryDate)) ||
                  (paymentMethod === 'saved' && !selectedSavedCard)}
                className="px-6 py-2 rounded-full bg-primary text-white text-sm font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors"
              >
                {isCheckingOut ? 'Processing...' : 'Complete Order'}
              </button>
            </div>
          </div>
        </div>
      )}

      
    </div>
  );
}
