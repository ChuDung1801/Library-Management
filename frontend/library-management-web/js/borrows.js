/**
 * borrows.js - Nghiệp vụ mượn/trả sách (Epic 04: SCRUM-43, 44, 45).
 * Yêu cầu api.js + auth.js được load trước file này.
 */
const BorrowsApi = {
  getAll() {
    return apiRequest('/borrows', { method: 'GET', auth: true });
  },
  getOverdue() {
    return apiRequest('/borrows/overdue', { method: 'GET', auth: true });
  },
  create(bookId, memberId) {
    return apiRequest('/borrows', { method: 'POST', auth: true, body: { bookId, memberId } });
  },
  returnBook(borrowId) {
    return apiRequest(`/borrows/${borrowId}/return`, { method: 'PUT', auth: true });
  },
  // --- Member tự xem lịch sử / sách đang mượn của mình (SCRUM-47, 48) ---
  getMine() {
    return apiRequest('/members/me/borrows', { method: 'GET', auth: true });
  },
  getMineActive() {
    return apiRequest('/members/me/borrows/active', { method: 'GET', auth: true });
  }
};
