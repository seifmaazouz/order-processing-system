import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faBookOpen, faShoppingBag, faChartBar, faArrowRightFromBracket, faCalendar, faSearch } from '@fortawesome/free-solid-svg-icons';
import { useForm } from 'react-hook-form';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import {
	getTotalSalesPreviousMonth,
	getTotalSalesByDate,
	getTop5Customers,
	getTop10SellingBooks,
	getBookReplenishmentCount
} from '../../api/reports.api.js';

export default function Analytics() {
	const navigate = useNavigate();
	const [previousMonthSales, setPreviousMonthSales] = useState(null);
	const [salesByDate, setSalesByDate] = useState(null);
	const [topCustomers, setTopCustomers] = useState([]);
	const [topBooks, setTopBooks] = useState([]);
	const [replenishmentData, setReplenishmentData] = useState(null);
	const [isLoading, setIsLoading] = useState({
		previousMonth: false,
		byDate: false,
		customers: false,
		books: false,
		replenishment: false
	});

	const { register: registerDate, handleSubmit: handleSubmitDate } = useForm({
		defaultValues: { date: '' }
	});

	const { register: registerIsbn, handleSubmit: handleSubmitIsbn } = useForm({
		defaultValues: { isbn: '' }
	});

	// Fetch previous month sales on mount
	useEffect(() => {
		fetchPreviousMonthSales();
		fetchTop5Customers();
		fetchTop10SellingBooks();
	}, []);

	const fetchPreviousMonthSales = async () => {
		setIsLoading(prev => ({ ...prev, previousMonth: true }));
		try {
			const data = await getTotalSalesPreviousMonth();
			setPreviousMonthSales(data);
		} catch (error) {
			const errorMsg = error.response?.data?.error || error.response?.data?.message || 'Failed to fetch previous month sales';
			toast.error(errorMsg);
		} finally {
			setIsLoading(prev => ({ ...prev, previousMonth: false }));
		}
	};

	const fetchSalesByDate = async (formData) => {
		if (!formData.date) {
			toast.error('Please select a date');
			return;
		}
		setIsLoading(prev => ({ ...prev, byDate: true }));
		try {
			const data = await getTotalSalesByDate(formData.date);
			setSalesByDate(data);
			toast.success('Sales data retrieved successfully');
		} catch (error) {
			const errorMsg = error.response?.data?.error || error.response?.data?.message || 'Failed to fetch sales by date';
			toast.error(errorMsg);
		} finally {
			setIsLoading(prev => ({ ...prev, byDate: false }));
		}
	};

	const fetchTop5Customers = async () => {
		setIsLoading(prev => ({ ...prev, customers: true }));
		try {
			const data = await getTop5Customers();
			setTopCustomers(Array.isArray(data) ? data : []);
		} catch (error) {
			const errorMsg = error.response?.data?.error || error.response?.data?.message || 'Failed to fetch top customers';
			toast.error(errorMsg);
			setTopCustomers([]);
		} finally {
			setIsLoading(prev => ({ ...prev, customers: false }));
		}
	};

	const fetchTop10SellingBooks = async () => {
		setIsLoading(prev => ({ ...prev, books: true }));
		try {
			const data = await getTop10SellingBooks();
			setTopBooks(Array.isArray(data) ? data : []);
		} catch (error) {
			const errorMsg = error.response?.data?.error || error.response?.data?.message || 'Failed to fetch top selling books';
			toast.error(errorMsg);
			setTopBooks([]);
		} finally {
			setIsLoading(prev => ({ ...prev, books: false }));
		}
	};

	const fetchBookReplenishment = async (formData) => {
		if (!formData.isbn) {
			toast.error('Please enter an ISBN');
			return;
		}
		setIsLoading(prev => ({ ...prev, replenishment: true }));
		try {
			const data = await getBookReplenishmentCount(formData.isbn);
			setReplenishmentData(data);
			toast.success('Replenishment data retrieved successfully');
		} catch (error) {
			const errorMsg = error.response?.data?.error || error.response?.data?.message || 'Failed to fetch replenishment data';
			toast.error(errorMsg);
			setReplenishmentData(null);
		} finally {
			setIsLoading(prev => ({ ...prev, replenishment: false }));
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
							<button onClick={() => navigate('/admin/orders')} className="group flex items-center gap-3 px-4 py-3 rounded-full text-text-main dark:text-gray-200 hover:bg-[#efe9e3] dark:hover:bg-[#3a2d20] transition-colors w-full text-left">
								<FontAwesomeIcon icon={faShoppingBag} className="group-hover:scale-110 transition-transform" />
								<p className="text-sm font-medium">Orders</p>
							</button>
							<button onClick={() => navigate('/admin/analytics')} className="flex items-center gap-3 px-4 py-3 rounded-full bg-[#f4ede7] dark:bg-[#3a2d20] shadow-sm w-full text-left">
								<FontAwesomeIcon icon={faChartBar} className="text-text-main dark:text-primary" />
								<p className="text-sm font-bold text-text-main dark:text-white">Analytics</p>
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
									<h2 className="text-2xl font-bold text-text-main dark:text-white">Analytics & Reports</h2>
									<p className="text-sm text-text-secondary dark:text-gray-400 mt-1">System-wide sales and performance metrics</p>
								</div>
							</div>
						</div>
					</header>

					{/* Scrollable Content */}
					<div className="flex-1 overflow-y-auto px-6 py-6 scroll-smooth">
						<div className="max-w-[1400px] mx-auto space-y-6">
							
							{/* Previous Month Sales */}
							<div className="bg-white dark:bg-surface-dark rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700">
								<h3 className="text-lg font-bold mb-4 flex items-center gap-2">
									<FontAwesomeIcon icon={faChartBar} className="text-primary" />
									Total Sales - Previous Month
								</h3>
								{isLoading.previousMonth ? (
									<div className="text-center py-4 text-gray-500">Loading...</div>
								) : previousMonthSales ? (
									<div className="grid grid-cols-1 md:grid-cols-3 gap-4">
										<div className="bg-surface-light dark:bg-background-dark p-4 rounded-lg">
											<p className="text-sm text-text-secondary dark:text-gray-400">Total Sales</p>
											<p className="text-2xl font-bold text-primary">${previousMonthSales.totalSales?.toFixed(2) || '0.00'}</p>
										</div>
										<div className="bg-surface-light dark:bg-background-dark p-4 rounded-lg">
											<p className="text-sm text-text-secondary dark:text-gray-400">Books Sold</p>
											<p className="text-2xl font-bold text-text-main dark:text-white">{previousMonthSales.totalBooksSold || 0}</p>
										</div>
										<div className="bg-surface-light dark:bg-background-dark p-4 rounded-lg">
											<p className="text-sm text-text-secondary dark:text-gray-400">Orders</p>
											<p className="text-2xl font-bold text-text-main dark:text-white">{previousMonthSales.totalOrders || 0}</p>
										</div>
									</div>
								) : (
									<div className="text-center py-4 text-gray-500">No data available</div>
								)}
							</div>

							{/* Sales by Specific Date */}
							<div className="bg-white dark:bg-surface-dark rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700">
								<h3 className="text-lg font-bold mb-4 flex items-center gap-2">
									<FontAwesomeIcon icon={faCalendar} className="text-primary" />
									Total Sales by Specific Date
								</h3>
								<form onSubmit={handleSubmitDate(fetchSalesByDate)} className="space-y-4">
									<div className="flex gap-3">
										<input
											type="date"
											{...registerDate('date', { required: true })}
											className="flex-1 h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										<button
											type="submit"
											disabled={isLoading.byDate}
											className="px-6 py-2 rounded-full bg-primary text-white font-semibold hover:bg-primary/90 disabled:opacity-50"
										>
											{isLoading.byDate ? 'Loading...' : 'Search'}
										</button>
									</div>
								</form>
								{salesByDate && (
									<div className="grid grid-cols-1 md:grid-cols-3 gap-4 mt-4">
										<div className="bg-surface-light dark:bg-background-dark p-4 rounded-lg">
											<p className="text-sm text-text-secondary dark:text-gray-400">Total Sales</p>
											<p className="text-2xl font-bold text-primary">${salesByDate.totalSales?.toFixed(2) || '0.00'}</p>
										</div>
										<div className="bg-surface-light dark:bg-background-dark p-4 rounded-lg">
											<p className="text-sm text-text-secondary dark:text-gray-400">Books Sold</p>
											<p className="text-2xl font-bold text-text-main dark:text-white">{salesByDate.totalBooksSold || 0}</p>
										</div>
										<div className="bg-surface-light dark:bg-background-dark p-4 rounded-lg">
											<p className="text-sm text-text-secondary dark:text-gray-400">Orders</p>
											<p className="text-2xl font-bold text-text-main dark:text-white">{salesByDate.totalOrders || 0}</p>
										</div>
									</div>
								)}
							</div>

							{/* Top 5 Customers */}
							<div className="bg-white dark:bg-surface-dark rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700">
								<h3 className="text-lg font-bold mb-4 flex items-center gap-2">
									<FontAwesomeIcon icon={faChartBar} className="text-primary" />
									Top 5 Customers (Last 3 Months)
								</h3>
								{isLoading.customers ? (
									<div className="text-center py-4 text-gray-500">Loading...</div>
								) : topCustomers.length > 0 ? (
									<div className="overflow-x-auto">
										<table className="w-full">
											<thead>
												<tr className="border-b border-gray-200 dark:border-gray-700">
													<th className="text-left py-3 px-4 text-sm font-semibold">Rank</th>
													<th className="text-left py-3 px-4 text-sm font-semibold">Customer Name</th>
													<th className="text-left py-3 px-4 text-sm font-semibold">Email</th>
													<th className="text-right py-3 px-4 text-sm font-semibold">Total Spent</th>
													<th className="text-right py-3 px-4 text-sm font-semibold">Orders</th>
												</tr>
											</thead>
											<tbody>
												{topCustomers.map((customer, index) => (
													<tr key={customer.userId} className="border-b border-gray-200 dark:border-gray-700 hover:bg-surface-light dark:hover:bg-background-dark">
														<td className="py-3 px-4">
															<span className="inline-flex items-center justify-center w-8 h-8 rounded-full bg-primary text-white font-bold text-sm">
																{index + 1}
															</span>
														</td>
														<td className="py-3 px-4 font-medium">{customer.customerName}</td>
														<td className="py-3 px-4 text-sm text-text-secondary">{customer.email}</td>
														<td className="py-3 px-4 text-right font-bold text-primary">${customer.totalSpent?.toFixed(2) || '0.00'}</td>
														<td className="py-3 px-4 text-right">{customer.orderCount || 0}</td>
													</tr>
												))}
											</tbody>
										</table>
									</div>
								) : (
									<div className="text-center py-4 text-gray-500">No customer data available</div>
								)}
							</div>

							{/* Top 10 Selling Books */}
							<div className="bg-white dark:bg-surface-dark rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700">
								<h3 className="text-lg font-bold mb-4 flex items-center gap-2">
									<FontAwesomeIcon icon={faBookOpen} className="text-primary" />
									Top 10 Selling Books (Last 3 Months)
								</h3>
								{isLoading.books ? (
									<div className="text-center py-4 text-gray-500">Loading...</div>
								) : topBooks.length > 0 ? (
									<div className="overflow-x-auto">
										<table className="w-full">
											<thead>
												<tr className="border-b border-gray-200 dark:border-gray-700">
													<th className="text-left py-3 px-4 text-sm font-semibold">Rank</th>
													<th className="text-left py-3 px-4 text-sm font-semibold">ISBN</th>
													<th className="text-left py-3 px-4 text-sm font-semibold">Title</th>
													<th className="text-right py-3 px-4 text-sm font-semibold">Copies Sold</th>
													<th className="text-right py-3 px-4 text-sm font-semibold">Revenue</th>
												</tr>
											</thead>
											<tbody>
												{topBooks.map((book, index) => (
													<tr key={book.isbn} className="border-b border-gray-200 dark:border-gray-700 hover:bg-surface-light dark:hover:bg-background-dark">
														<td className="py-3 px-4">
															<span className="inline-flex items-center justify-center w-8 h-8 rounded-full bg-primary text-white font-bold text-sm">
																{index + 1}
															</span>
														</td>
														<td className="py-3 px-4 text-sm text-text-secondary">{book.isbn}</td>
														<td className="py-3 px-4 font-medium">{book.title}</td>
														<td className="py-3 px-4 text-right font-bold">{book.totalCopiesSold || 0}</td>
														<td className="py-3 px-4 text-right text-primary font-bold">${book.totalRevenue?.toFixed(2) || '0.00'}</td>
													</tr>
												))}
											</tbody>
										</table>
									</div>
								) : (
									<div className="text-center py-4 text-gray-500">No book data available</div>
								)}
							</div>

							{/* Book Replenishment Count */}
							<div className="bg-white dark:bg-surface-dark rounded-2xl p-6 shadow-sm border border-gray-200 dark:border-gray-700">
								<h3 className="text-lg font-bold mb-4 flex items-center gap-2">
									<FontAwesomeIcon icon={faSearch} className="text-primary" />
									Book Replenishment History
								</h3>
								<form onSubmit={handleSubmitIsbn(fetchBookReplenishment)} className="space-y-4">
									<div className="flex gap-3">
										<input
											type="text"
											{...registerIsbn('isbn', { required: true })}
											placeholder="Enter ISBN"
											className="flex-1 h-10 px-3 rounded-md bg-surface-light dark:bg-surface-dark border border-gray-200 dark:border-gray-700 focus:ring-2 focus:ring-primary outline-none"
										/>
										<button
											type="submit"
											disabled={isLoading.replenishment}
											className="px-6 py-2 rounded-full bg-primary text-white font-semibold hover:bg-primary/90 disabled:opacity-50"
										>
											{isLoading.replenishment ? 'Loading...' : 'Search'}
										</button>
									</div>
								</form>
								{replenishmentData && (
									<div className="mt-4 bg-surface-light dark:bg-background-dark p-6 rounded-lg">
										<div className="grid grid-cols-1 md:grid-cols-2 gap-4">
											<div>
												<p className="text-sm text-text-secondary dark:text-gray-400">Book Title</p>
												<p className="text-lg font-bold text-text-main dark:text-white">{replenishmentData.bookTitle}</p>
											</div>
											<div>
												<p className="text-sm text-text-secondary dark:text-gray-400">ISBN</p>
												<p className="text-lg font-bold text-text-main dark:text-white">{replenishmentData.isbn}</p>
											</div>
											<div>
												<p className="text-sm text-text-secondary dark:text-gray-400">Times Ordered</p>
												<p className="text-3xl font-bold text-primary">{replenishmentData.timesOrdered || 0}</p>
											</div>
											<div>
												<p className="text-sm text-text-secondary dark:text-gray-400">Total Quantity</p>
												<p className="text-3xl font-bold text-text-main dark:text-white">{replenishmentData.totalQuantityOrdered || 0}</p>
											</div>
										</div>
									</div>
								)}
							</div>

						</div>
					</div>
				</main>
			</div>
		</div>
	);
}
