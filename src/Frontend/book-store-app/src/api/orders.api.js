import axios from 'axios';
import API_BASE_URL from '../config/api.config.js';

const CUSTOMER_ORDERS_URL = `${API_BASE_URL}/orders`;

export async function getOrders() {
  const res = await axios.get(`${CUSTOMER_ORDERS_URL}`, {
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true,
  });
  return res.data; // [ { id, date, status, items: [], totalPrice, ... }, ... ]
}

export async function getOrderDetails(orderId) {
  const res = await axios.get(`${CUSTOMER_ORDERS_URL}/${orderId}`, {
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true,
  });
  return res.data; // { id, date, status, items, totalPrice, ... }
}
