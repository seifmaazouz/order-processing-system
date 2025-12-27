import axios from 'axios';
import API_BASE_URL from '../config/api.config.js';

const ORDERS_URL = `${API_BASE_URL}/orders`;

// Get all orders for admin
export const getAllOrders = async () => {
	try {
		const response = await axios.get(ORDERS_URL);
		return response.data;
	} catch (error) {
		console.error('Error fetching orders:', error.response?.data);
		throw error;
	}
};

// Confirm an order (add quantity to stock)
export const confirmOrder = async (orderId) => {
	try {
		const response = await axios.put(`${ORDERS_URL}/${orderId}/confirm`);
		return response.data;
	} catch (error) {
		console.error('Error confirming order:', error.response?.data);
		throw error;
	}
};

// Get pending orders only
export const getPendingOrders = async () => {
	try {
		const response = await axios.get(ORDERS_URL, {
			params: { status: 'Pending' }
		});
		return response.data;
	} catch (error) {
		console.error('Error fetching pending orders:', error.response?.data);
		throw error;
	}
};
