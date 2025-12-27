import axios from 'axios';

const BASE_URL = 'http://localhost:8080/api/orders';

export async function getOrders() {
  const res = await axios.get(`${BASE_URL}`, {
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true,
  });
  return res.data; // [ { id, date, status, items: [], totalPrice, ... }, ... ]
}

export async function getOrderDetails(orderId) {
  const res = await axios.get(`${BASE_URL}/${orderId}`, {
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true,
  });
  return res.data; // { id, date, status, items, totalPrice, ... }
}
