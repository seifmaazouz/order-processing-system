import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBox } from '@fortawesome/free-solid-svg-icons';
import { useNavigate } from 'react-router-dom';

export default function OrdersEmpty() {
  const navigate = useNavigate();

  return (
    <div className="rounded-xl border border-dashed border-gray-300 dark:border-gray-700 bg-white dark:bg-surface-dark p-8 text-center">
      <FontAwesomeIcon icon={faBox} className="text-4xl text-gray-300 dark:text-gray-600 mb-4" />
      <p className="text-lg font-semibold mb-2">No orders yet</p>
      <p className="text-gray-600 dark:text-gray-400 mb-4">You haven't placed any orders.</p>
      <button
        onClick={() => navigate('/dashboard')}
        className="px-5 py-2 rounded-full bg-primary text-white font-semibold hover:bg-primary/90"
      >
        Start Shopping
      </button>
    </div>
  );
}
