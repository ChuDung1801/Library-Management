/**
 * api.js - Lớp giao tiếp HTTP dùng chung cho toàn bộ frontend.
 * Sprint 1: chỉ cần các endpoint Account Management (/api/auth, /api/members).
 */
const API_BASE_URL = 'http://localhost:5000/api';

/**
 * Gửi request tới backend, tự đính kèm Bearer token nếu auth=true.
 * Ném Error(message) khi backend trả lỗi (đã được ExceptionMiddleware chuẩn hóa JSON).
 */
async function apiRequest(path, { method = 'GET', body, auth = false } = {}) {
  const headers = { 'Content-Type': 'application/json' };

  if (auth) {
    const token = AuthStorage.getToken();
    if (!token) {
      throw new Error('Bạn cần đăng nhập để thực hiện thao tác này.');
    }
    headers['Authorization'] = `Bearer ${token}`;
  }

  let response;
  try {
    response = await fetch(`${API_BASE_URL}${path}`, {
      method,
      headers,
      body: body !== undefined ? JSON.stringify(body) : undefined
    });
  } catch (networkErr) {
    throw new Error('Không thể kết nối tới máy chủ. Vui lòng kiểm tra API đã chạy tại ' + API_BASE_URL);
  }

  let data = null;
  try {
    data = await response.json();
  } catch (_) {
    // response rỗng (ví dụ 204) - bỏ qua
  }

  if (!response.ok) {
    const message = (data && data.message) || `Lỗi ${response.status}`;
    throw new Error(message);
  }

  return data;
}
