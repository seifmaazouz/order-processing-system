import React, { useState } from 'react';
import { AnimatePresence, motion } from 'framer-motion';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faArrowAltCircleLeft, faArrowAltCircleRight, faClose, faDiagramNext, faShoppingCart } from '@fortawesome/free-solid-svg-icons';
import BookCard from './BookCard.jsx';
import { useCart } from '../../context/CartContext.jsx';

export default function ResultsGrid({ results, lastQueryLabel, onStockUpdate }) {
  const [selectedIndex, setSelectedIndex] = useState(null);
  const [toast, setToast] = useState(null);
  const [loadingBooks, setLoadingBooks] = useState(new Set()); // Track which books are being added
  const selectedBook = selectedIndex !== null && results ? results[selectedIndex] : null;
  const { addToCart, error, items } = useCart();

  if (!results || results.length === 0) return null;

  const getStockStatus = (book) => {
    // Prefer `displayStock` computed by Dashboard; fall back to Book.Stock / book.stock etc.
    const stock = (book.displayStock ?? book.Stock ?? book.stock ?? book.Quantity ?? book.quantity ?? book.stockLevel) ?? 0;
    if (stock === 0) return { label: 'Out of Stock', color: 'bg-gray-100 text-gray-600', textColor: 'text-gray-500' };
    if (stock <= 2) return { label: `Only ${stock} left`, color: 'bg-red-200 text-red-600', textColor: 'text-red-500' };
    if (stock <= 5) return { label: `Only ${stock} left`, color: 'bg-orange-100 text-orange-600', textColor: 'text-orange-500' };

    return { label: 'In Stock', color: 'bg-green-100 text-green-600', textColor: 'text-green-500' };
  };

  const handleNext = () => {
    if (!results || results.length === 0) return;
    setSelectedIndex((prev) => {
      if (prev === null) return 0;
      return prev + 1;
    });
  };

  const handlePrev = () => {
    if (!results || results.length === 0) return;
    setSelectedIndex((prev) => {
      if (prev === null) return 0;
      return prev-1 ;
    });
  };

  const isInCart = (book) => {
    const bookIsbn = book.ISBN || book.isbn || book.id;
    return items.some(item => item.id === bookIsbn);
  };

  const isBookLoading = (book) => {
    const bookId = book.ISBN || book.isbn || book.id;
    return loadingBooks.has(bookId);
  };

  const handleAddToCart = async (book) => {
    const bookId = book.ISBN || book.isbn || book.id;
    setLoadingBooks(prev => new Set(prev).add(bookId));

    try {
      const success = await addToCart(book);
      if (success) {
        const bookTitle = book.Title || book.title || 'Book';
        setToast({ type: 'success', message: `"${bookTitle}" added to cart!` });
        setTimeout(() => setToast(null), 3000);

        // Dispatch a global cart change immediately so UI updates (dashboard/listings)
        try {
          window.dispatchEvent(new CustomEvent('cart:quantityChanged', { detail: { id: bookId, delta: 1 } }));
        } catch (e) {}

        // Fallback: update via callback prop if provided
        const currentStock = book.Quantity || book.quantity || book.Stock || book.stock || book.stockLevel || 10;
        if (onStockUpdate && currentStock > 0) {
          onStockUpdate(bookId, currentStock - 1);
        }
      } else {
        setToast({ type: 'error', message: error || 'Failed to add item' });
        setTimeout(() => setToast(null), 4000);
      }
    } catch (err) {
      console.error('Error adding to cart:', err);
      setToast({ type: 'error', message: 'Failed to add item to cart' });
      setTimeout(() => setToast(null), 4000);
    } finally {
      setLoadingBooks(prev => {
        const newSet = new Set(prev);
        newSet.delete(bookId);
        return newSet;
      });
    }
  };

  return (
    <div className="mt-8">
      <h3 className="text-2xl font-bold mb-6">
        Search for {lastQueryLabel || 'your query'} ({results.length} results)
      </h3>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        {results.map((book, idx) => {
          // Prefer displayStock if available (Dashboard subtracts cart quantities)
          const stock = (book.displayStock ?? book.Stock ?? book.stock ?? book.Quantity ?? book.quantity ?? book.stockLevel) ?? 0;
          const available = book.isAvailable !== undefined ? !!book.isAvailable : true;
          const status = !available || stock === 0
            ? { label: 'Out of Stock', color: 'bg-gray-100 text-gray-600', textColor: 'text-gray-500', unavailable: true }
            : stock <= 2
            ? { label: `Only ${stock} left`, color: 'bg-red-200 text-red-600', textColor: 'text-red-500', unavailable: false }
            : stock <= 5
            ? { label: `Only ${stock} left`, color: 'bg-orange-100 text-orange-600', textColor: 'text-orange-500', unavailable: false }
            : { label: 'In Stock', color: 'bg-green-100 text-green-600', textColor: 'text-green-500', unavailable: false };

          return (
            <BookCard
              key={book.id || book.isbn || book.ISBN}
              book={book}
              status={status}
              onSelect={() => setSelectedIndex(idx)}
              onAddToCart={() => handleAddToCart(book)}
              isLoading={isBookLoading(book)}
              isInCart={isInCart(book)}
            />
          );
        })}
      </div>

      {/* Centered Modal for selected book with framer-motion */}
      <AnimatePresence>
        {selectedBook && (
          <div className="fixed inset-0 z-50 flex items-center justify-center px-4">
            {/* Backdrop */}
            <motion.div
              className="absolute inset-0 bg-black/50 backdrop-blur-sm"
              onClick={() => setSelectedIndex(null)}
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
            />

            {/* Modal Card */}
            <motion.div
              key={selectedIndex}
              className="relative z-10 w-full max-w-3xl bg-white dark:bg-surface-dark border-2 border-primary rounded-2xl shadow-2xl p-8 pointer-events-auto"
              initial={{ opacity: 0, scale: 0.96, y: 16 }}
              animate={{ opacity: 1, scale: 1, y: 0 }}
              exit={{ opacity: 0, scale: 0.96, y: 16 }}
              transition={{ type: 'spring', stiffness: 220, damping: 24 }}
            >
              {/* Left hover area for Previous button */}
              <div className="absolute left-0 top-0 bottom-0 w-24 group/left">
                <button
                  onClick={handlePrev}
                  className="absolute left-2 top-1/2 -translate-y-1/2 h-10 w-10 rounded-full bg-gray-100 text-gray-700 border border-gray-300 hover:bg-gray-200 shadow-lg opacity-0 group-hover/left:opacity-100 transition-opacity duration-200 flex items-center justify-center"
                  aria-label="Previous"
                >
                  <FontAwesomeIcon icon={faArrowAltCircleLeft} />
                </button>
              </div>

              {/* Right hover area for Next button */}
              <div className="absolute right-0 top-0 bottom-0 w-24 group/right">
                <button
                  onClick={handleNext}
                  className="absolute right-2 top-1/2 -translate-y-1/2 h-10 w-10 rounded-full bg-gray-100 text-gray-700 border border-gray-300 hover:bg-gray-200 shadow-lg opacity-0 group-hover/right:opacity-100 transition-opacity duration-200 flex items-center justify-center"
                  aria-label="Next"
                >
                  <FontAwesomeIcon icon={faArrowAltCircleRight} />
                </button>
              </div>

            {/* Close button (top-right) */}
            <button
              onClick={() => setSelectedIndex(null)}
              className="absolute top-3 right-3 h-10 w-10 rounded-full bg-gray-100 text-gray-600 hover:bg-gray-200 font-bold"
              aria-label="Close"
            >
              <FontAwesomeIcon icon={faClose}/>
            </button>

            {/* Animated content wrapper */}
            <AnimatePresence mode="wait">
              <motion.div
                key={`book-${selectedBook.isbn || selectedBook.id || selectedIndex}`}
                initial={{ opacity: 0, x: 30 }}
                animate={{ opacity: 1, x: 0 }}
                exit={{ opacity: 0, x: -30 }}
                transition={{ duration: 0.2, ease: 'easeInOut' }}
                className="pointer-events-auto"
              >
                {/* Status Badge */}
                <div className="flex justify-start mb-4">
                  {(() => {
                    let stock = selectedBook.Stock ?? selectedBook.stock ?? selectedBook.Quantity ?? selectedBook.quantity ?? selectedBook.stockLevel ?? 10;
                    let available = selectedBook.isAvailable !== undefined ? !!selectedBook.isAvailable : true;
                    let status;
                    if (!available || stock === 0) status = { label: 'Out of Stock', color: 'bg-gray-100 text-gray-600', textColor: 'text-gray-500', unavailable: true };
                    else if (stock <= 2) status = { label: `Only ${stock} left`, color: 'bg-red-200 text-red-600', textColor: 'text-red-500', unavailable: false };
                    else if (stock <= 5) status = { label: `Only ${stock} left`, color: 'bg-orange-100 text-orange-600', textColor: 'text-orange-500', unavailable: false };
                    else status = { label: 'In Stock', color: 'bg-green-100 text-green-600', textColor: 'text-green-500', unavailable: false };
                    return (
                      <span className={`inline-flex text-sm font-semibold px-4 py-1.5 rounded-full ${status.color}`}>
                        • {status.label}
                      </span>
                    );
                  })()}
                </div>

                {/* Header */}
                <div className="flex flex-col gap-2 mb-6">
                  <h3 className="text-2xl font-bold text-gray-900">{selectedBook.Title || selectedBook.title}</h3>
                  {(selectedBook.Authors || selectedBook.authors) && (selectedBook.Authors || selectedBook.authors).length > 0 && (
                    <p className="text-sm text-gray-600">
                      <span className="font-semibold">Authors:</span> {Array.isArray(selectedBook.Authors || selectedBook.authors) ? (selectedBook.Authors || selectedBook.authors).join(', ') : (selectedBook.Authors || selectedBook.authors)}
                    </p>
                  )}
                </div>

            {/* Details Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-6 text-sm">
              {(selectedBook.ISBN || selectedBook.isbn) && (
                <div className="flex justify-between border border-gray-100 rounded-md px-3 py-2">
                  <span className="text-gray-600">ISBN</span>
                  <span className="font-semibold text-gray-900">{selectedBook.ISBN || selectedBook.isbn}</span>
                </div>
              )}
              {(selectedBook.Year || selectedBook.year) && (
                <div className="flex justify-between border border-gray-100 rounded-md px-3 py-2">
                  <span className="text-gray-600">Year</span>
                  <span className="font-semibold text-gray-900">{selectedBook.Year || selectedBook.year}</span>
                </div>
              )}
              {(selectedBook.Category || selectedBook.category) && (
                <div className="flex justify-between border border-gray-100 rounded-md px-3 py-2">
                  <span className="text-gray-600">Category</span>
                  <span className="font-semibold text-gray-900">{selectedBook.Category || selectedBook.category}</span>
                </div>
              )}
              {(selectedBook.Publisher || selectedBook.publisher) && (
                <div className="flex justify-between border border-gray-100 rounded-md px-3 py-2">
                  <span className="text-gray-600">Publisher</span>
                  <span className="font-semibold text-gray-900">{selectedBook.Publisher || selectedBook.publisher}</span>
                </div>
              )}
              {((selectedBook.displayStock !== undefined) || (selectedBook.Stock !== undefined) || (selectedBook.stock !== undefined)) && (
                <div className="flex justify-between border border-gray-100 rounded-md px-3 py-2">
                  <span className="text-gray-600">Stock</span>
                  <span className="font-semibold text-gray-900">{(selectedBook.displayStock ?? selectedBook.Stock ?? selectedBook.stock)} units</span>
                </div>
              )}
              {selectedBook.isAvailable !== undefined && (
                <div className="flex justify-between border border-gray-100 rounded-md px-3 py-2">
                  <span className="text-gray-600">Available</span>
                  <span className={`font-semibold ${selectedBook.isAvailable ? 'text-green-600' : 'text-red-600'}`}>
                    {selectedBook.isAvailable ? 'Yes' : 'No'}
                  </span>
                </div>
              )}
            </div>

            {/* Price + Action */}
            <div className="flex items-center justify-between border-t border-gray-200 pt-4 mt-2">
              {(selectedBook.Price || selectedBook.price) && (
                <p className="text-2xl font-bold text-gray-900">
                  ${parseFloat(selectedBook.Price || selectedBook.price).toFixed(2)}
                </p>
              )}

              <div className="flex items-center gap-3 ml-auto">
                <button
                  onClick={() => handleAddToCart(selectedBook)}
                  disabled={(selectedBook.displayStock ?? selectedBook.Stock ?? selectedBook.stock ?? 0) === 0 || isBookLoading(selectedBook) || isInCart(selectedBook)}
                  className={`flex items-center gap-2 px-5 py-2 rounded-full text-white font-semibold shadow-md ${
                    isInCart(selectedBook)
                      ? 'bg-green-500 cursor-not-allowed'
                      : (selectedBook.Stock || selectedBook.stock || 0) === 0 || isBookLoading(selectedBook)
                      ? 'bg-gray-400 cursor-not-allowed'
                      : 'bg-orange-500 hover:bg-orange-600'
                  } pointer-events-auto`}
                >
                  <FontAwesomeIcon icon={faShoppingCart} className="text-sm" />
                  {isInCart(selectedBook)
                    ? 'In Cart'
                    : isBookLoading(selectedBook)
                    ? 'Adding...'
                    : (selectedBook.displayStock ?? selectedBook.Stock ?? selectedBook.stock ?? 0) === 0
                    ? 'Unavailable'
                    : 'Add to Cart'}
                </button>
              </div>
            </div>
          </motion.div>
        </AnimatePresence>
          </motion.div>
        </div>
      )}
    </AnimatePresence>

    {/* Toast Notification (top-center) */}
    <AnimatePresence>
      {toast && (
        <motion.div
          initial={{ opacity: 0, y: -20 }}
          animate={{ opacity: 1, y: 0 }}
          exit={{ opacity: 0, y: -20 }}
          className={`fixed top-4 left-1/2 transform -translate-x-1/2 px-5 py-3 rounded-lg font-semibold text-white shadow-lg z-50 ${
            toast.type === 'success' ? 'bg-green-500' : 'bg-red-500'
          }`}
        >
          {toast.message}
        </motion.div>
      )}
    </AnimatePresence>
    </div>
  );
}
