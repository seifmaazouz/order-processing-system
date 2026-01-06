import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBookOpen, faShoppingBag, faChartBar, faArrowRightFromBracket, faCheckCircle, faFilter } from '@fortawesome/free-solid-svg-icons';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { getAllOrders, confirmOrder } from '../../api/adminOrders.api.js';
import LogoutConfirmation from '../../components/shared/LogoutConfirmation.jsx';
import { logout } from '../../api/accountDetails.api.js';

export default function AdminOrders() {
	const navigate = useNavigate();
	const [orders, setOrders] = useState([]);
	const [filteredOrders, setFilteredOrders] = useState([]);
	const [isLoading, setIsLoading] = useState(false);
	const [statusFilter, setStatusFilter] = useState('All');
	const [showConfirmModal, setShowConfirmModal] = useState(false);
	const [showLogoutConfirm, setShowLogoutConfirm] = useState(false);
	const [selectedOrder, setSelectedOrder] = useState(null);


	useEffect(() => {
		fetchOrders();
	}, []);

	useEffect(() => {
		filterOrders();
	}, [statusFilter, orders]);

	const fetchOrders = async () => {
		setIsLoading(true);
		try {
			const data = await getAllOrders();
			setOrders(Array.isArray(data) ? data : []);
		} catch (error) {
			const errorMsg = error.response?.data?.error || error.response?.data?.message || 'Failed to fetch orders';
			toast.error(errorMsg);
			setOrders([]);
		} finally {
			setIsLoading(false);
		}
	};

	const filterOrders = () => {
		if (statusFilter === 'All') {
			setFilteredOrders(orders);
		} else {
			setFilteredOrders(orders.filter(order => {
				const orderStatus = order.status?.toString() || '';
				return orderStatus.toLowerCase() === statusFilter.toLowerCase();
			}));
		}
	};

	const handleConfirmClick = (order) => {
		setSelectedOrder(order);
		setShowConfirmModal(true);
	};

	const handleConfirmOrder = async () => {
		if (!selectedOrder) return;

		try {
			await confirmOrder(selectedOrder.orderId);
			toast.success('Order confirmed successfully! Stock updated.');
			setShowConfirmModal(false);
			setSelectedOrder(null);
			fetchOrders(); // Refresh orders list
		} catch (error) {
			const errorMsg = error.response?.data?.error || error.response?.data?.message || 'Failed to confirm order';
			toast.error(errorMsg);
			console.error('Confirm order error:', error.response?.data);
		}
	};


	const handleLogout = async () => {
		setShowLogoutConfirm(false);

		try {
			const token = localStorage.getItem('access');
			if (token) {
				// Call backend logout to invalidate session
				await logout(token);
			}
		} catch (error) {
			console.warn('Backend logout failed:', error);
			// Continue with local cleanup even if backend call fails
		}

		// Clear local authentication data
		localStorage.removeItem('access');
		localStorage.removeItem('role');
		localStorage.removeItem('userId');
		localStorage.removeItem('authToken');

		navigate('/login', { replace: true });
	};

	const getStatusColor = (status) => {
		const statusStr = status?.toString().toLowerCase() || '';
		switch (statusStr) {
			case 'pending':
				return 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400';
			case 'confirmed':
				return 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400';
			case 'canceled':
			case 'cancelled':
				return 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400';
			default:
				return 'bg-gray-100 text-gray-700 dark:bg-gray-900/30 dark:text-gray-400';
		}
	};

	return (
		<div className="light bg-background-light font-display text-text-main antialiased overflow-hidden">
			<div className="flex h-screen w-full">
				<ToastContainer position="top-right" autoClose={3000} hideProgressBar theme="colored" />
				
				{/* Side Navigation */}
				<aside className="hidden md:flex w-72 flex-col justify-between border-r border-[#e6e0db] bg-background-light p-6 transition-all">
					<div className="flex flex-col gap-8">
						<div className="flex flex-col gap-1 px-2">
							<h1 className="text-2xl font-bold text-text-main dark:text-white">Admin Dashboard</h1>
						</div>
						<nav className="flex flex-col gap-2">
							<button onClick={() => navigate('/admin')} className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main hover:bg-[#efe9e3] transition-colors w-full text-left">
								<FontAwesomeIcon icon={faBookOpen} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Inventory</p>
							</button>
							<button onClick={() => navigate('/admin/orders')} className="flex items-center gap-3 px-4 py-3 rounded-full bg-[#f4ede7] shadow-sm w-full text-left">
								<FontAwesomeIcon icon={faShoppingBag} className="text-text-main" />
								<p className="text-sm font-bold text-text-main">Orders</p>
							</button>
							<button onClick={() => navigate('/admin/analytics')} className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main hover:bg-[#efe9e3] transition-colors w-full text-left">
								<FontAwesomeIcon icon={faChartBar} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Analytics</p>
							</button>
						</nav>
					</div>
					<button
						onClick={() => setShowLogoutConfirm(true)}
						className="flex w-full cursor-pointer items-center justify-center gap-2 overflow-hidden rounded-full h-12 px-6 bg-red-50 hover:bg-red-100 dark:bg-red-900/20 dark:hover:bg-red-900/30 text-red-600 dark:text-red-400 text-sm font-bold transition-colors"
					>
						<FontAwesomeIcon icon={faArrowRightFromBracket} />
						<span>Logout</span>
					</button>
				</aside>

				{/* Main Content */}
				<main className="flex flex-1 flex-col overflow-hidden">
					{/* Header */}
					<header className="border-b border-[#e6e0db] bg-background-light">
						<div className="px-6 py-6">
							<div className="flex items-center justify-between mb-4">
								<div>
									<h2 className="text-2xl font-bold text-text-main">Order Management</h2>
										<p className="text-sm text-text-secondary mt-1">Confirm automatic replenishment orders from publishers</p>
								</div>
								<div className="flex items-center gap-3">
									<FontAwesomeIcon icon={faFilter} className="text-text-secondary" />
									<select
										value={statusFilter}
										onChange={(e) => setStatusFilter(e.target.value)}
										className="h-10 px-4 rounded-full bg-surface-light border border-gray-200 focus:ring-2 focus:ring-primary outline-none text-sm font-medium"
									>
										<option value="All">All Orders</option>
										<option value="Pending">Pending</option>
										<option value="Confirmed">Confirmed</option>
										<option value="Cancelled">Cancelled</option>
									</select>
								</div>
							</div>
						</div>
					</header>

					{/* Scrollable Content */}
					<div className="flex-1 overflow-y-auto px-6 py-6 scroll-smooth">
						<div className="max-w-[1400px] mx-auto">
							{isLoading ? (
								<div className="text-center py-12 text-gray-500">Loading orders...</div>
							) : filteredOrders.length > 0 ? (
								<div className="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
									<div className="overflow-x-auto">
										<table className="w-full">
											<thead className="bg-surface-light">
												<tr>
													<th className="text-left py-4 px-6 text-sm font-bold">Order ID</th>
													<th className="text-left py-4 px-6 text-sm font-bold">Order Date</th>
													<th className="text-left py-4 px-6 text-sm font-bold">Publisher ID</th>
													<th className="text-right py-4 px-6 text-sm font-bold">Total Price</th>
													<th className="text-left py-4 px-6 text-sm font-bold">Ordered by</th>
													<th className="text-left py-4 px-6 text-sm font-bold">Confirmed by</th>
													<th className="text-center py-4 px-6 text-sm font-bold">Status</th>
													<th className="text-center py-4 px-6 text-sm font-bold">Action</th>
												</tr>
											</thead>
											<tbody>
												{filteredOrders.map((order) => (
													<tr key={order.orderId} className="border-t border-gray-200 hover:bg-surface-light transition-colors">
														<td className="py-4 px-6 font-medium">#{order.orderId}</td>
														<td className="py-4 px-6 text-sm">{order.orderDate ? new Date(order.orderDate).toLocaleDateString() : 'N/A'}</td>
														<td className="py-4 px-6 text-sm text-text-secondary">#{order.publisherId}</td>
														<td className="py-4 px-6 text-right font-bold text-primary">${order.totalPrice?.toFixed(2) || '0.00'}</td>
														<td className="py-4 px-6 text-sm">
															<span className="text-blue-600 font-medium">System / Auto Trigger</span>
														</td>
														<td className="py-4 px-6 text-sm">
															{order.confirmedBy ? (
																<span className="text-green-600 font-medium">{order.confirmedBy}</span>
															) : (
																<span className="text-gray-400">—</span>
															)}
														</td>
														<td className="py-4 px-6 text-center">
															<span className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${getStatusColor(order.status)}`}>
																{order.status}
															</span>
														</td>
														<td className="py-4 px-6 text-center">
															{order.status?.toString().toLowerCase() === 'pending' ? (
																<button
																	onClick={() => handleConfirmClick(order)}
																	className="px-4 py-2 rounded-full bg-primary text-white text-sm font-semibold hover:bg-primary/90 transition-colors"
																>
																	<FontAwesomeIcon icon={faCheckCircle} className="mr-2" />
																	Confirm
																</button>
															) : (
																<span className="text-sm text-text-secondary">—</span>
															)}
														</td>
													</tr>
												))}
											</tbody>
										</table>
									</div>
								</div>
							) : (
								<div className="bg-white rounded-2xl p-12 shadow-sm border border-gray-200 text-center">
									<FontAwesomeIcon icon={faShoppingBag} className="text-6xl text-gray-300 mb-4" />
									<p className="text-lg font-semibold text-text-main mb-2">No orders found</p>
									<p className="text-sm text-text-secondary">
										{statusFilter !== 'All' ? `No ${statusFilter.toLowerCase()} orders available` : 'No orders available'}
									</p>
								</div>
							)}
						</div>
					</div>
				</main>

				{/* Confirm Order Modal */}
				{showConfirmModal && selectedOrder && (
					<div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 backdrop-blur-sm px-4">
						<div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-2xl border border-gray-200">
							<h3 className="text-xl font-bold mb-4 text-text-main">Confirm Order</h3>
							<div className="space-y-4 mb-6">
								<p className="text-sm text-text-secondary">
									You are about to confirm order <span className="font-bold text-text-main">#{selectedOrder.orderId}</span> from publisher #{selectedOrder.publisherId}.
								</p>

								{/* Order Items */}
								<div className="bg-surface-light p-4 rounded-lg">
									<h4 className="text-sm font-semibold text-text-main mb-3">Books to be added to inventory:</h4>
									<div className="space-y-2">
										{selectedOrder.items && selectedOrder.items.length > 0 ? (
											selectedOrder.items.map((item, index) => (
												<div key={index} className="flex justify-between items-center py-2 border-b border-gray-200 last:border-b-0">
													<div>
														<span className="text-sm font-medium">ISBN: {item.isbn}</span>
													</div>
													<div className="text-right">
														<span className="text-sm font-bold text-primary">+{item.quantity} units</span>
														<span className="text-xs text-text-secondary ml-2">@ ${item.unitPrice?.toFixed(2)}</span>
													</div>
												</div>
											))
										) : (
											<p className="text-sm text-text-secondary">No items found in this order.</p>
										)}
									</div>
									<div className="flex justify-between items-center pt-3 mt-3 border-t border-gray-200">
										<span className="text-sm font-semibold">Total Value:</span>
										<span className="text-sm font-bold text-primary">${selectedOrder.totalPrice?.toFixed(2) || '0.00'}</span>
									</div>
								</div>

								<div className="bg-green-50 p-3 rounded-lg">
									<p className="text-sm text-green-700 flex items-center">
										<FontAwesomeIcon icon={faCheckCircle} className="mr-2" />
										Confirming this order will add the listed quantities to book inventory
									</p>
								</div>
							</div>
							<div className="flex justify-end gap-3">
								<button
									onClick={() => {
										setShowConfirmModal(false);
										setSelectedOrder(null);
									}}
									className="px-4 py-2 rounded-full border border-gray-300 bg-surface-light text-sm font-semibold hover:border-primary hover:text-primary transition-colors"
								>
									Cancel
								</button>
								<button
									onClick={handleConfirmOrder}
									className="px-6 py-2 rounded-full bg-primary text-white text-sm font-semibold hover:bg-primary/90 transition-colors"
								>
									<FontAwesomeIcon icon={faCheckCircle} className="mr-2" />
									Confirm Order
								</button>
							</div>
						</div>
					</div>
				)}


				<LogoutConfirmation
					isOpen={showLogoutConfirm}
					onConfirm={handleLogout}
					onCancel={() => setShowLogoutConfirm(false)}
				/>
			</div>
		</div>
	);
}
