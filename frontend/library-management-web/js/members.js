/**
 * members.js - Quản trị viên quản lý tài khoản thành viên (SCRUM-24, 42).
 * Yêu cầu api.js + auth.js được load trước file này.
 */
const MembersAdminApi = {
  getAll() {
    return apiRequest('/members', { method: 'GET', auth: true });
  },
  update(id, payload) {
    return apiRequest(`/members/${id}`, { method: 'PUT', auth: true, body: payload });
  },
  setActive(id, isActive) {
    return apiRequest(`/members/${id}/status`, { method: 'PUT', auth: true, body: { isActive } });
  }
};
