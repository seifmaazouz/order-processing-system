import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBookOpen, faShoppingBag, faChartBar, faArrowRightFromBracket, faCheckCircle, faFilter } from '@fortawesome/free-solid-svg-icons';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { getAllOrders, confirmOrder } from '../../api/adminOrders.api.js';

export default function AdminOrders() {
	const navigate = useNavigate();
	const [orders, setOrders] = useState([]);
	const [filteredOrders, setFilteredOrders] = useState([]);
	const [isLoading, setIsLoading] = useState(false);
	const [statusFilter, setStatusFilter] = useState('All');
	const [showConfirmModal, setShowConfirmModal] = useState(false);
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
			setFilteredOrders(orders.filter(order => order.status === statusFilter));
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

	const getStatusColor = (status) => {
		switch (status) {
			case 'Pending':
				return 'bg-yellow-100 text-yellow-700 dark:bg-yellow-900/30 dark:text-yellow-400';
			case 'Confirmed':
				return 'bg-green-100 text-green-700 dark:bg-green-900/30 dark:text-green-400';
			case 'Cancelled':
				return 'bg-red-100 text-red-700 dark:bg-red-900/30 dark:text-red-400';
			default:
				return 'bg-gray-100 text-gray-700 dark:bg-gray-900/30 dark:text-gray-400';
		}
	};

	return (
		<div className="light bg-background-light dark:bg-background-dark font-display text-text-main dark:text-gray-100 antialiased overflow-hidden">
			<div className="flex h-screen w-full">
				<ToastContainer position="top-right" autoClose={3000} hideProgressBar theme="colored" />
				
				{/* Side Navigation */}
				<aside className="hidden md:flex w-72 flex-col justify-between border-r border-[#e6e0db] dark:border-[#443628] bg-background-light dark:bg-background-dark p-6 transition-all">
					<div className="flex flex-col gap-8">
						<div className="flex flex-col gap-1 px-2">
							<h1 className="text-2xl font-black tracking-tighter text-text-main dark:text-white">Chapter One</h1>
							<p className="text-text-secondary text-sm font-medium">Admin Dashboard</p>
						</div>
						<nav className="flex flex-col gap-2">
							<button onClick={() => navigate('/admin')} className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main dark:text-gray-200 hover:bg-[#efe9e3] dark:hover:bg-[#3a2d20] transition-colors w-full text-left">
								<FontAwesomeIcon icon={faBookOpen} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Inventory</p>
							</button>
							<button onClick={() => navigate('/admin/orders')} className="flex items-center gap-3 px-4 py-3 rounded-full bg-[#f4ede7] dark:bg-[#3a2d20] shadow-sm w-full text-left">
								<FontAwesomeIcon icon={faShoppingBag} className="text-text-main dark:text-primary" />
								<p className="text-sm font-bold text-text-main dark:text-white">Orders</p>
							</button>
							<button onClick={() => navigate('/admin/analytics')} className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main dark:text-gray-200 hover:bg-[#efe9e3] dark:hover:bg-[#3a2d20] transition-colors w-full text-left">
								<FontAwesomeIcon icon={faChartBar} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Analytics</p>
							</button>
						</nav>
					</div>
					<button className="flex w-full cursor-pointer items-center justify-center gap-2 overflow-hidden rounded-full h-12 px-6 bg-primary/10 hover:bg-primary/20 dark:bg-primary/20 dark:hover:bg-primary/30 text-text-main dark:text-primary text-sm font-bold transition-colors">
						<FontAwesomeIcon icon={faArrowRightFromBracket} />
						<span>Logout</span>
					</button>
				</aside>

				{/* Main Content */}
				<main className="flex flex-1 flex-col overflow-hidden">
					{/* Header */}
					<header className="border-b border-[#e6e0db] dark:border-[#443628] bg-background-light dark:bg-background-dark">
						<div className="px-6 py-6">
							<div className="flex items-center justify-between mb-4">
								<div>
									<h2 className="text-2xl font-bold text-text-main dark:text-white">Order Management</h2>
									<p className="text-sm text-text-secondary dark:text-gray-400 mt-1">Confirm replenishment orders from publishers</p>
								</div>
								<div className="flex items-center gap-3">
									<FontAwesomeIcon icon={faFilter} className="text-text-secondary" />
									<select
										value={statusFilter}
										onChange={(e) => setStatusFilter(e.target.value)}
										className="h-10 px-4 rounded-full bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none text-sm font-medium"
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
								<div className="bg-white dark:bg-surface-dark rounded-2xl shadow-sm border border-gray-200 dark:border-gray-700 overflow-hidden">
									<div className="overflow-x-auto">
										<table className="w-full">
											<thead className="bg-surface-light dark:bg-background-dark">
												<tr>
													<th className="text-left py-4 px-6 text-sm font-bold">Order ID</th>
													<th className="text-left py-4 px-6 text-sm font-bold">ISBN</th>
													<th className="text-left py-4 px-6 text-sm font-bold">Book Title</th>
													<th className="text-left py-4 px-6 text-sm font-bold">Publisher</th>
													<th className="text-right py-4 px-6 text-sm font-bold">Quantity</th>
													<th className="text-left py-4 px-6 text-sm font-bold">Order Date</th>
													<th className="text-center py-4 px-6 text-sm font-bold">Status</th>
													<th className="text-center py-4 px-6 text-sm font-bold">Action</th>
												</tr>
											</thead>
											<tbody>
												{filteredOrders.map((order) => (
													<tr key={order.orderId} className="border-t border-gray-200 dark:border-gray-700 hover:bg-surface-light dark:hover:bg-background-dark transition-colors">
														<td className="py-4 px-6 font-medium">#{order.orderId}</td>
														<td className="py-4 px-6 text-sm text-text-secondary">{order.isbn}</td>
														<td className="py-4 px-6 font-medium">{order.bookTitle || 'N/A'}</td>
														<td className="py-4 px-6 text-sm">{order.publisherName || 'N/A'}</td>
														<td className="py-4 px-6 text-right font-bold text-primary">{order.quantity}</td>
														<td className="py-4 px-6 text-sm">{order.orderDate ? new Date(order.orderDate).toLocaleDateString() : 'N/A'}</td>
														<td className="py-4 px-6 text-center">
															<span className={`inline-block px-3 py-1 rounded-full text-xs font-semibold ${getStatusColor(order.status)}`}>
																{order.status}
															</span>
														</td>
														<td className="py-4 px-6 text-center">
															{order.status === 'Pending' ? (
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
								<div className="bg-white dark:bg-surface-dark rounded-2xl p-12 shadow-sm border border-gray-200 dark:border-gray-700 text-center">
									<FontAwesomeIcon icon={faShoppingBag} className="text-6xl text-gray-300 dark:text-gray-600 mb-4" />
									<p className="text-lg font-semibold text-text-main dark:text-white mb-2">No orders found</p>
									<p className="text-sm text-text-secondary dark:text-gray-400">
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
						<div className="w-full max-w-md rounded-2xl bg-white dark:bg-surface-dark p-6 shadow-2xl border border-gray-200 dark:border-gray-700">
							<h3 className="text-xl font-bold mb-4 text-text-main dark:text-white">Confirm Order</h3>
							<div className="space-y-3 mb-6">
								<p className="text-sm text-text-secondary dark:text-gray-400">
									You are about to confirm order <span className="font-bold text-text-main dark:text-white">#{selectedOrder.orderId}</span>
								</p>
								<div className="bg-surface-light dark:bg-background-dark p-4 rounded-lg space-y-2">
									<div className="flex justify-between">
										<span className="text-sm text-text-secondary">Book:</span>
										<span className="text-sm font-semibold">{selectedOrder.bookTitle}</span>
									</div>
									<div className="flex justify-between">
										<span className="text-sm text-text-secondary">ISBN:</span>
										<span className="text-sm font-semibold">{selectedOrder.isbn}</span>
									</div>
									<div className="flex justify-between">
										<span className="text-sm text-text-secondary">Quantity:</span>
										<span className="text-sm font-bold text-primary">{selectedOrder.quantity} units</span>
									</div>
								</div>
								<p className="text-sm text-green-600 dark:text-green-400 bg-green-50 dark:bg-green-900/20 p-3 rounded-lg">
									✓ This quantity will be added to the book's stock
								</p>
							</div>
							<div className="flex justify-end gap-3">
								<button
									onClick={() => {
										setShowConfirmModal(false);
										setSelectedOrder(null);
									}}
									className="px-4 py-2 rounded-full border border-gray-300 dark:border-gray-600 bg-surface-light dark:bg-surface-dark text-sm font-semibold hover:border-primary hover:text-primary transition-colors"
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
			</div>
		</div>
	);
}
