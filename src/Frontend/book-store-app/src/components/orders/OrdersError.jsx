import React from 'react';

export default function OrdersError({ error }) {
  return (
    <div className="rounded-xl border border-red-200 dark:border-red-800 bg-red-50 dark:bg-red-900/20 p-4 text-red-600 dark:text-red-300">
      Error: {error}
    </div>
  );
}
