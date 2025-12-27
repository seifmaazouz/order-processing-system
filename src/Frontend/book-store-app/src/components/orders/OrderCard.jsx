import React from 'react';

export default function OrderCard({ order, getStatusBadge }) {
  const statusBadge = getStatusBadge(order.status);
  const orderDate = new Date(order.date || order.createdAt).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  });

  return (
    <div className="rounded-xl border border-gray-200 dark:border-gray-700 bg-white dark:bg-surface-dark p-6 shadow-sm hover:shadow-md transition-shadow">
      <div className="flex items-start justify-between mb-4">
        <div>
          <h3 className="text-lg font-semibold mb-1">Order #{order.id}</h3>
          <p className="text-sm text-gray-600 dark:text-gray-400">{orderDate}</p>
        </div>
        <span className={`inline-flex px-3 py-1 rounded-full text-xs font-semibold ${statusBadge.color}`}>
          {statusBadge.label}
        </span>
      </div>

      {/* Items List */}
      {order.items && order.items.length > 0 && (
        <div className="mb-4">
          <h4 className="text-sm font-semibold mb-2">Items:</h4>
          <ul className="space-y-1 text-sm text-gray-700 dark:text-gray-300">
            {order.items.map((item, idx) => (
              <li key={idx} className="flex justify-between">
                <span>
                  {item.title} <span className="text-gray-500">x{item.quantity}</span>
                </span>
                <span>${(Number(item.price) * item.quantity).toFixed(2)}</span>
              </li>
            ))}
          </ul>
        </div>
      )}

      {/* Total Price */}
      <div className="border-t border-gray-200 dark:border-gray-700 pt-4 flex justify-between items-center">
        <span className="font-semibold">Total:</span>
        <span className="text-lg font-bold text-primary">
          ${Number(order.totalPrice || order.total || 0).toFixed(2)}
        </span>
      </div>
    </div>
  );
}
