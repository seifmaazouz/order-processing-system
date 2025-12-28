import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faShoppingCart, faEdit, faTrash, faCheck } from '@fortawesome/free-solid-svg-icons';

export default function BookCard({ book, status, onSelect, onAddToCart, onEdit, onRemove, isLoading = false, isAdminMode = false, isInCart = false }) {
  const isOutOfStock = status.label === 'Out of Stock';
  // Handle both PascalCase and camelCase
  const authorsArray = book.Authors || book.authors || [];
  const authors = Array.isArray(authorsArray) ? authorsArray.join(', ') : authorsArray;

  const handleAddClick = async (e) => {
    e.stopPropagation();
    await onAddToCart();
  };

  const handleEditClick = (e) => {
    e.stopPropagation();
    onEdit && onEdit(book);
  };

  const handleRemoveClick = (e) => {
    e.stopPropagation();
    onRemove && onRemove(book);
  };

  return (
    <div
      className="relative group rounded-lg bg-white border border-gray-200 overflow-visible hover:shadow-lg transition-all duration-300 hover:border-primary cursor-pointer h-full flex flex-col"
      onClick={onSelect}
    >
      <div className="pointer-events-auto flex flex-col h-full">
        <div className="flex justify-start p-4 pb-3">
          <span className={`inline-flex text-xs font-semibold px-3 py-1 rounded-full ${status.color}`}>
            • {status.label}
          </span>
        </div>

        <div className="px-4 pb-4 flex flex-col items-start flex-1">
          <h4 className="font-bold text-base mb-3 text-gray-900 line-clamp-2 text-left">
            {book.Title || book.title}
          </h4>

          {authors && (
            <p className="text-xs text-gray-500 mb-3 text-left">
              by {authors}
            </p>
          )}

          <div className="flex items-center justify-between mt-auto w-full">
            {(book.Price || book.price) && (
              <p className="text-lg font-bold text-gray-900">
                ${parseFloat(book.Price || book.price).toFixed(2)}
              </p>
            )}

            {isAdminMode ? (
              <div className="flex items-center gap-2">
                <button
                  onClick={handleEditClick}
                  className="flex items-center gap-1 px-3 py-2 rounded-full bg-blue-500 hover:bg-blue-600 text-white text-sm font-semibold transition-colors shadow-md"
                >
                  <FontAwesomeIcon icon={faEdit} className="text-sm" />
                  Edit
                </button>
                <button
                  onClick={handleRemoveClick}
                  className="flex items-center gap-1 px-3 py-2 rounded-full bg-red-500 hover:bg-red-600 text-white text-sm font-semibold transition-colors shadow-md"
                >
                  <FontAwesomeIcon icon={faTrash} className="text-sm" />
                  Remove
                </button>
              </div>
            ) : (
              <>
                {isInCart ? (
                  <button
                    disabled
                    className="flex items-center gap-1 px-3 py-2 rounded-full bg-green-500 text-white text-sm font-semibold cursor-not-allowed"
                  >
                    <FontAwesomeIcon icon={faCheck} className="text-sm" />
                    In Cart
                  </button>
                ) : isOutOfStock ? (
                  <button
                    disabled
                    className="flex items-center gap-1 px-3 py-2 rounded-full bg-gray-300 text-gray-600 text-sm font-semibold cursor-not-allowed opacity-60"
                  >
                    Sold Out
                  </button>
                ) : (
                  <button
                    onClick={handleAddClick}
                    disabled={isLoading}
                    className={`flex items-center gap-1 px-3 py-2 rounded-full text-white text-sm font-semibold transition-colors shadow-md ${
                      isLoading
                        ? 'bg-gray-400 cursor-not-allowed'
                        : 'bg-orange-500 hover:bg-orange-600 hover:shadow-lg'
                    }`}
                  >
                    <FontAwesomeIcon icon={faShoppingCart} className="text-sm" />
                    {isLoading ? 'Adding...' : 'Add'}
                  </button>
                )}
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
