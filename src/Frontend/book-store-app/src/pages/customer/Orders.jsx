import React, { useEffect, useState } from 'react';
import Header from '../../components/shared/Header.jsx';
import OrdersList from '../../components/orders/OrdersList.jsx';
import OrdersEmpty from '../../components/orders/OrdersEmpty.jsx';
import OrdersLoading from '../../components/orders/OrdersLoading.jsx';
import OrdersError from '../../components/orders/OrdersError.jsx';
import { getOrders } from '../../api/orders.api.js';

export default function Orders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);
        const data = await getOrders();
        setOrders(data || []);
      } catch (err) {
        setError(err?.message || 'Failed to load orders');
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []);

  const getStatusBadge = (status) => {
    const statusMap = {
      pending: { label: 'Pending', color: 'bg-yellow-100 text-yellow-600' },
      processing: { label: 'Processing', color: 'bg-blue-100 text-blue-600' },
      shipped: { label: 'Shipped', color: 'bg-purple-100 text-purple-600' },
      delivered: { label: 'Delivered', color: 'bg-green-100 text-green-600' },
      cancelled: { label: 'Cancelled', color: 'bg-red-100 text-red-600' },
    };
    return statusMap[status?.toLowerCase()] || { label: status, color: 'bg-gray-100 text-gray-600' };
  };

  return (
    <div className="min-h-screen bg-background-light dark:bg-background-dark text-text-main-light dark:text-text-main-dark">
      {/* Shared Header */}
      <Header />

      <main className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
        <h1 className="text-3xl font-bold mb-6">Orders</h1>

        {loading && <OrdersLoading />}
        {error && <OrdersError error={error} />}
        {!loading && !error && orders.length === 0 && <OrdersEmpty />}
        {!loading && !error && orders.length > 0 && <OrdersList orders={orders} getStatusBadge={getStatusBadge} />}
      </main>
    </div>
  );
}
