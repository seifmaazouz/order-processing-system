import React from 'react';
import OrderCard from './OrderCard.jsx';

export default function OrdersList({ orders, getStatusBadge }) {
  return (
    <div className="space-y-4">
      {orders.map((order) => (
        <OrderCard
          key={order.id}
          order={order}
          getStatusBadge={getStatusBadge}
        />
      ))}
    </div>
  );
}
