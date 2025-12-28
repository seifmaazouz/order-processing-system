import React, { useState, useRef, useEffect } from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowLeft, faTrash, faHome } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../../context/CartContext.jsx';
import DashboardHeader from '../../components/dashboard/DashboardHeader.jsx';
import { checkoutCart } from '../../api/addCart.js';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

export default function Cart() {
  const navigate = useNavigate();
  const { items, summary, removeFromCart, updateQuantity, clearCart, loadCart } = useCart();
  const [showSettings, setShowSettings] = useState(false);
  const [showCheckoutModal, setShowCheckoutModal] = useState(false);
  const [isCheckingOut, setIsCheckingOut] = useState(false);
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
    loadCart();
  }, [loadCart]);

  const handleQtyChange = (id, delta) => {
    const item = items.find((i) => i.id === id);
    if (!item) return;
    updateQuantity(id, Math.max(1, item.quantity + delta));
  };

  const handleCheckout = async () => {
    if (!cardNumber || !expiryDate) {
      toast.error('Please enter credit card information');
      return;
    }

    setIsCheckingOut(true);
    try {
      // Convert expiry date from MM/YY to Date string (last day of the month) in YYYY-MM-DD format
      const [month, year] = expiryDate.split('/');
      const expiryYear = 2000 + parseInt(year);
      const expiryMonth = parseInt(month);
      // Get last day of the month
      const lastDay = new Date(expiryYear, expiryMonth, 0).getDate();
      // Format as YYYY-MM-DD for better compatibility
      const expiryDateStr = `${expiryYear}-${String(expiryMonth).padStart(2, '0')}-${String(lastDay).padStart(2, '0')}`;
      
      const cardNum = parseInt(cardNumber.replace(/\s/g, ''));
      if (isNaN(cardNum)) {
        toast.error('Invalid card number');
        setIsCheckingOut(false);
        return;
      }
      
      await checkoutCart(cardNum, expiryDateStr);
      toast.success('Order placed successfully!');
      clearCart();
      setShowCheckoutModal(false);
      setCardNumber('');
      setExpiryDate('');
      await loadCart(); // Refresh cart
      navigate('/orders');
    } catch (error) {
      const errorMsg = error.response?.data?.message || error.response?.data?.error || error.message || 'Checkout failed';
      toast.error(errorMsg);
    } finally {
      setIsCheckingOut(false);
    }
  };

  return (
    <div className="min-h-screen bg-background-light text-text-main">
      <DashboardHeader
        showSettings={showSettings}
        onToggleSettings={setShowSettings}
        settingsRef={settingsRef}
        cartTotal={summary?.totalItems ?? 0}
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
              className="text-sm font-semibold text-red-600 hover:text-red-500"
            >
              Clear cart
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
                      <h3 className="font-semibold text-lg">{item.title}</h3>
                      {authors && (
                        <p className="text-sm text-gray-600 mb-1">{authors}</p>
                      )}
                      <p className="text-sm text-gray-500">Stock: {item.stock ?? 'N/A'}</p>
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

            <div className="rounded-xl border border-gray-200 bg-white p-6 shadow-sm flex flex-col gap-4">
              <h2 className="text-xl font-bold">Order Summary</h2>
              <div className="flex items-center justify-between text-sm">
                <span>Items</span>
                <span>{summary.totalItems}</span>
              </div>
              <div className="flex items-center justify-between text-sm">
                <span>Subtotal</span>
                <span>${summary.totalPrice.toFixed(2)}</span>
              </div>
              <button
                onClick={() => setShowCheckoutModal(true)}
                className="mt-4 w-full rounded-full bg-primary text-white font-semibold py-3 hover:bg-primary/90"
              >
                Checkout
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
                      const formatted = value.length > 2 ? `${value.slice(0, 2)}/${value.slice(2)}` : value;
                      setExpiryDate(formatted);
                    }
                  }}
                  placeholder="MM/YY"
                  className="w-full h-10 px-3 rounded-md bg-white border border-gray-200 focus:ring-2 focus:ring-primary outline-none"
                />
              </div>
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
                  setCardNumber('');
                  setExpiryDate('');
                }}
                className="px-4 py-2 rounded-full border border-gray-300 bg-white text-sm font-semibold hover:border-primary hover:text-primary transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleCheckout}
                disabled={isCheckingOut || !cardNumber || !expiryDate}
                className="px-6 py-2 rounded-full bg-primary text-white text-sm font-semibold hover:bg-primary/90 disabled:opacity-50 transition-colors"
              >
                {isCheckingOut ? 'Processing...' : 'Complete Order'}
              </button>
            </div>
          </div>
        </div>
      )}

      <ToastContainer position="top-right" autoClose={3000} hideProgressBar />
    </div>
  );
}
