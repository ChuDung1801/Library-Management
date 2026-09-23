/**
 * categories.js - Nghiệp vụ quản lý thể loại (Epic 02: SCRUM-35).
 * Yêu cầu api.js + auth.js được load trước file này.
 */
const CategoriesApi = {
  getAll() {
    return apiRequest('/categories', { method: 'GET', auth: true });
  },
  create(payload) {
    return apiRequest('/categories', { method: 'POST', auth: true, body: payload });
  }
};
