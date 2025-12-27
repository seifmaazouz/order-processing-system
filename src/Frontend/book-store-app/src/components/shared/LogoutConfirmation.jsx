import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faExclamationTriangle } from '@fortawesome/free-solid-svg-icons';

export default function LogoutConfirmation({ isOpen, onConfirm, onCancel }) {
  if (!isOpen) return null;

  return (
    <>
      {/* Opaque Background */}
      <div
        className="fixed inset-0 bg-black/50 z-40"
        onClick={onCancel}
      />

      {/* Modal */}
      <div className="fixed inset-0 flex items-center justify-center z-50 px-4">
        <div className="bg-white dark:bg-surface-dark rounded-lg shadow-2xl max-w-sm w-full border border-gray-200 dark:border-gray-700">
          {/* Header */}
          <div className="flex items-center justify-center pt-6 pb-4">
            <div className="w-12 h-12 rounded-full bg-red-100 dark:bg-red-900/30 flex items-center justify-center">
              <FontAwesomeIcon icon={faExclamationTriangle} className="text-red-600 dark:text-red-400 text-lg" />
            </div>
          </div>

          {/* Content */}
          <div className="px-6 py-4 text-center">
            <h2 className="text-xl font-bold text-text-main-light dark:text-text-main-dark mb-2">
              Confirm Logout
            </h2>
            <p className="text-gray-600 dark:text-gray-400 text-sm font-medium">
              Are you sure you want to log out? You'll need to sign in again to access your account.
            </p>
          </div>

          {/* Footer */}
          <div className="flex gap-3 px-6 py-4 border-t border-gray-200 dark:border-gray-700">
            <button
              onClick={onCancel}
              className="flex-1 px-4 py-2 rounded-lg bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 hover:border-gray-300 dark:hover:border-gray-600 text-text-main-light dark:text-text-main-dark font-semibold transition-colors"
            >
              No, Stay
            </button>
            <button
              onClick={onConfirm}
              className="flex-1 px-4 py-2 rounded-lg bg-red-600 hover:bg-red-700 text-white font-semibold transition-colors"
            >
              Yes, Logout
            </button>
          </div>
        </div>
      </div>
    </>
  );
}
