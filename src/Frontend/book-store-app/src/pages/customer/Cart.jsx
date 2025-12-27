import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowLeft, faTrash } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';
import { useCart } from '../../context/CartContext.jsx';
import Header from '../../components/shared/Header.jsx';

export default function Cart() {
  const navigate = useNavigate();
  const { items, summary, removeFromCart, updateQuantity, clearCart } = useCart();

  const handleQtyChange = (id, delta) => {
    const item = items.find((i) => i.id === id);
    if (!item) return;
    updateQuantity(id, Math.max(1, item.quantity + delta));
  };

  return (
    <div className="min-h-screen bg-background-light dark:bg-background-dark 
    text-text-main-light dark:text-text-main-dark px-4 sm:px-8 py-8">
        <Header />
      <div className="max-w-5xl mx-auto">
        <div className="flex items-center justify-between mb-6">
          <button
            onClick={() => navigate('/dashboard')}
            className="flex items-center gap-2 text-sm font-semibold text-primary hover:text-primary/80"
          >
            <FontAwesomeIcon icon={faArrowLeft} /> Back to shop
          </button>
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
        <p className="text-gray-600 dark:text-gray-400 mb-8">Review items before checkout.</p>

        {items.length === 0 ? (
          <div className="rounded-xl border border-dashed border-gray-300 dark:border-gray-700 bg-white dark:bg-surface-dark p-8 text-center">
            <p className="text-lg font-semibold mb-2">Your cart is empty</p>
            <p className="text-gray-600 dark:text-gray-400 mb-4">Add books from the dashboard to see them here.</p>
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
                    className="flex items-start gap-4 rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-surface-dark p-4 shadow-sm"
                  >
                    <div className="flex-1">
                      <h3 className="font-semibold text-lg">{item.title}</h3>
                      {authors && (
                        <p className="text-sm text-gray-600 dark:text-gray-400 mb-1">{authors}</p>
                      )}
                      <p className="text-sm text-gray-500 dark:text-gray-500">Stock: {item.stock ?? 'N/A'}</p>
                      <div className="mt-3 flex items-center gap-3">
                        <div className="flex items-center gap-2 bg-gray-100 dark:bg-gray-800 rounded-full px-3 py-1">
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
                        <p className="text-sm font-semibold text-gray-800 dark:text-gray-200">
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

            <div className="rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-surface-dark p-6 shadow-sm flex flex-col gap-4">
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
                className="mt-4 w-full rounded-full bg-primary text-white font-semibold py-3 hover:bg-primary/90"
              >
                Checkout
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
