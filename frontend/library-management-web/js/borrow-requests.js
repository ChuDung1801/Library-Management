/**
 * borrow-requests.js - Yêu cầu mượn sách (SCRUM-49).
 * Yêu cầu api.js + auth.js được load trước file này.
 */
const BorrowRequestsApi = {
  create(bookId) {
    return apiRequest('/borrow-requests', { method: 'POST', auth: true, body: { bookId } });
  },
  getMine() {
    return apiRequest('/borrow-requests/mine', { method: 'GET', auth: true });
  },
  getAll() {
    return apiRequest('/borrow-requests', { method: 'GET', auth: true });
  },
  getPending() {
    return apiRequest('/borrow-requests/pending', { method: 'GET', auth: true });
  },
  approve(id) {
    return apiRequest(`/borrow-requests/${id}/approve`, { method: 'PUT', auth: true });
  },
  reject(id, note) {
    return apiRequest(`/borrow-requests/${id}/reject`, { method: 'PUT', auth: true, body: { note: note || null } });
  }
};
