/**
 * books.js - Nghiệp vụ quản lý sách (Epic 02: SCRUM-19, 33, 34, 36).
 * Yêu cầu api.js + auth.js được load trước file này.
 */
const BooksApi = {
  getAll() {
    return apiRequest('/books', { method: 'GET', auth: true });
  },
  getById(id) {
    return apiRequest(`/books/${id}`, { method: 'GET', auth: true });
  },
  create(payload) {
    return apiRequest('/books', { method: 'POST', auth: true, body: payload });
  },
  update(id, payload) {
    return apiRequest(`/books/${id}`, { method: 'PUT', auth: true, body: payload });
  },
  remove(id) {
    return apiRequest(`/books/${id}`, { method: 'DELETE', auth: true });
  }
};
