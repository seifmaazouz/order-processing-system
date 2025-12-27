import axios from 'axios';

const BASE_URL = 'http://localhost:8080/api/account';

export async function getAccountDetails() {
  const res = await axios.get(`${BASE_URL}/details`, {
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true,
  });
  return res.data; // { id, name, email, phone, address, ... }
}

export async function updateAccountDetails(payload) {
  const res = await axios.put(`${BASE_URL}/details`, payload, {
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true,
  });
  return res.data; // { ok: true, message }
}

export async function changePassword(payload) {
  const res = await axios.post(`${BASE_URL}/change-password`, payload, {
    headers: { 'Content-Type': 'application/json' },
    withCredentials: true,
  });
  return res.data; // { ok: true, message }
}
