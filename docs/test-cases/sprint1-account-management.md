# Test Case — Sprint 1: Epic 01 Account Management

## TC-01 Đăng ký thành công (Positive)
- Given: dữ liệu hợp lệ (fullName, email chưa tồn tại, password >= 6 ký tự, phone hợp lệ)
- When: POST /api/auth/register
- Then: 200 OK, trả về token + user role = Member

## TC-02 Đăng ký trùng email (Negative)
- Given: email đã tồn tại trong hệ thống
- When: POST /api/auth/register
- Then: 409 Conflict, message "Email này đã được sử dụng để đăng ký."

## TC-03 Đăng ký thiếu trường bắt buộc (Negative)
- Given: bỏ trống phoneNumber
- When: POST /api/auth/register
- Then: 400 Bad Request

## TC-04 Đăng ký tên 51 ký tự (Boundary)
- Given: fullName dài 51 ký tự
- When: POST /api/auth/register
- Then: 400 Bad Request ("Tên phải có độ dài từ 1 đến 50 ký tự.")

## TC-05 Đăng ký tên đúng 50 ký tự (Boundary)
- Given: fullName dài đúng 50 ký tự
- When: POST /api/auth/register
- Then: 200 OK

## TC-06 Đăng nhập đúng thông tin (Positive)
- Given: username/email + password khớp tài khoản đã seed (vd admin/admin123)
- When: POST /api/auth/login
- Then: 200 OK, trả token JWT

## TC-07 Đăng nhập sai mật khẩu (Negative)
- Given: username đúng, password sai
- When: POST /api/auth/login
- Then: 401 Unauthorized, message chung "Tên đăng nhập/email hoặc mật khẩu không đúng."
  (không tiết lộ username tồn tại hay không)

## TC-08 Cập nhật hồ sơ thành công (Positive)
- Given: đã đăng nhập, token hợp lệ
- When: PUT /api/members/me với fullName/email/phoneNumber mới hợp lệ
- Then: 200 OK, trả về thông tin đã cập nhật

## TC-09 Cập nhật hồ sơ không có token (Permission)
- Given: không gửi Authorization header
- When: PUT /api/members/me
- Then: 401 Unauthorized

## TC-10 Cập nhật email trùng người khác (Negative)
- Given: email mới trùng với một user khác đang tồn tại
- When: PUT /api/members/me
- Then: 409 Conflict
