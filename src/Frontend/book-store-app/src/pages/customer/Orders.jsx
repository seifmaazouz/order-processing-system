import React, { useEffect, useState, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faHome } from '@fortawesome/free-solid-svg-icons';
import DashboardHeader from '../../components/dashboard/DashboardHeader.jsx';
import OrdersList from '../../components/orders/OrdersList.jsx';
import OrdersEmpty from '../../components/orders/OrdersEmpty.jsx';
import OrdersLoading from '../../components/orders/OrdersLoading.jsx';
import OrdersError from '../../components/orders/OrdersError.jsx';
import { getOrders } from '../../api/orders.api.js';
import { useCart } from '../../context/CartContext.jsx';

export default function Orders() {
  const navigate = useNavigate();
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showSettings, setShowSettings] = useState(false);
  const settingsRef = useRef(null);
  const { summary } = useCart();

  useEffect(() => {
    function handleClick(e) {
      if (settingsRef.current && !settingsRef.current.contains(e.target)) {
        setShowSettings(false);
      }
    }
    document.addEventListener('click', handleClick);
    return () => document.removeEventListener('click', handleClick);
  }, []);

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
      <DashboardHeader
        showSettings={showSettings}
        onToggleSettings={setShowSettings}
        settingsRef={settingsRef}
        cartTotal={summary?.totalItems ?? 0}
      />

      <main className="max-w-5xl mx-auto px-4 sm:px-6 lg:px-8 py-10">
        <div className="flex items-center justify-between mb-6">
          <button
            onClick={() => navigate('/dashboard')}
            className="flex items-center gap-2 px-4 py-2 text-sm font-semibold bg-primary text-white rounded-full hover:bg-primary/90 transition-colors"
          >
            <FontAwesomeIcon icon={faHome} /> Back to Home
          </button>
          <h1 className="text-3xl font-bold">Orders</h1>
        </div>

        {loading && <OrdersLoading />}
        {error && <OrdersError error={error} />}
        {!loading && !error && orders.length === 0 && <OrdersEmpty />}
        {!loading && !error && orders.length > 0 && <OrdersList orders={orders} getStatusBadge={getStatusBadge} />}
      </main>
    </div>
  );
}
