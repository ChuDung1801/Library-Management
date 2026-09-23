/**
 * auth.js - Quản lý phiên đăng nhập (sessionStorage) và các lệnh gọi API
 * cho Epic Account Management (SCRUM-16, 29, 30, 31).
 * Yêu cầu api.js được load trước file này.
 */
const SESSION_KEY = 'scholaris_session';

const AuthStorage = {
  save(session) {
    sessionStorage.setItem(SESSION_KEY, JSON.stringify(session));
  },
  get() {
    const raw = sessionStorage.getItem(SESSION_KEY);
    return raw ? JSON.parse(raw) : null;
  },
  getToken() {
    const session = AuthStorage.get();
    return session ? session.token : null;
  },
  getUser() {
    const session = AuthStorage.get();
    return session ? session.user : null;
  },
  clear() {
    sessionStorage.removeItem(SESSION_KEY);
  }
};

const AuthApi = {
  /**
   * Đăng ký tài khoản Member. Dùng chung cho "Thành viên tạo tài khoản" (SCRUM-16)
   * và "Khách tạo tài khoản" (SCRUM-31).
   */
  async register({ fullName, username, email, password, phoneNumber }) {
    const data = await apiRequest('/auth/register', {
      method: 'POST',
      body: { fullName, username: username || null, email, password, phoneNumber }
    });
    AuthStorage.save({ token: data.token, expiresAt: data.expiresAt, user: data.user });
    return data;
  },

  /** Đăng nhập bằng username hoặc email (SCRUM-29). */
  async login(usernameOrEmail, password) {
    const data = await apiRequest('/auth/login', {
      method: 'POST',
      body: { usernameOrEmail, password }
    });
    AuthStorage.save({ token: data.token, expiresAt: data.expiresAt, user: data.user });
    return data;
  },

  /** Lấy thông tin cá nhân mới nhất từ server. */
  async getMe() {
    return apiRequest('/members/me', { method: 'GET', auth: true });
  },

  /** Cập nhật thông tin cá nhân (SCRUM-30). */
  async updateProfile({ fullName, phoneNumber, email }) {
    const updated = await apiRequest('/members/me', {
      method: 'PUT',
      auth: true,
      body: { fullName, phoneNumber, email }
    });
    const session = AuthStorage.get();
    if (session) {
      session.user = updated;
      AuthStorage.save(session);
    }
    return updated;
  },

  logout() {
    AuthStorage.clear();
  }
};

/**
 * Chặn truy cập trang nếu chưa đăng nhập (dùng ở đầu các trang cần bảo vệ).
 * allowedRoles: mảng role được phép, ví dụ ['Admin','Employee']. Bỏ trống = mọi role đã đăng nhập.
 */
function requireAuth(allowedRoles) {
  const user = AuthStorage.getUser();
  if (!user) {
    window.location.href = 'login.html';
    return null;
  }
  if (allowedRoles && allowedRoles.length && !allowedRoles.includes(user.role)) {
    window.location.href = 'catalog.html';
    return null;
  }
  return user;
}
