/**
 * catalog.js - Tìm kiếm & xem sách công khai (Member + Guest).
 * Gọi /api/catalog/* - không cần token (auth: false).
 * Yêu cầu api.js được load trước file này.
 */
const CatalogApi = {
  getBooks(keyword, categoryId) {
    const params = new URLSearchParams();
    if (keyword) params.set('keyword', keyword);
    if (categoryId) params.set('categoryId', categoryId);
    const qs = params.toString();
    return apiRequest(`/catalog/books${qs ? '?' + qs : ''}`, { method: 'GET', auth: false });
  },
  getBookById(id) {
    return apiRequest(`/catalog/books/${id}`, { method: 'GET', auth: false });
  },
  getCategories() {
    return apiRequest('/catalog/categories', { method: 'GET', auth: false });
  }
};
