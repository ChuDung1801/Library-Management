# Test Case — Sprint 2: Epic 02 Book Management

## TC-01 Thêm sách thành công (Positive)
- Given: Admin/Employee đã đăng nhập, mã sách chưa tồn tại, dữ liệu hợp lệ
- When: POST /api/books
- Then: 201 Created, trả về sách vừa tạo, status = Available (nếu totalCopies > 0)

## TC-02 Thêm sách trùng mã (Negative)
- Given: bookCode đã tồn tại
- When: POST /api/books
- Then: 409 Conflict

## TC-03 Thêm sách sai định dạng mã (Negative)
- Given: bookCode = "abc123" (không đúng PREFIX-0000)
- When: POST /api/books
- Then: 400 Bad Request

## TC-04 Thêm sách với categoryId không tồn tại (Negative)
- Given: categoryId trỏ tới thể loại không có trong hệ thống
- When: POST /api/books
- Then: 400 Bad Request ("Thể loại đã chọn không tồn tại.")

## TC-05 Số sách hiện có = 0 (Boundary)
- Given: totalCopies = 0
- When: POST /api/books
- Then: 201 Created, status = Unavailable

## TC-06 Số sách hiện có âm (Boundary/Negative)
- Given: totalCopies = -1
- When: POST /api/books
- Then: 400 Bad Request

## TC-07 Cập nhật sách thành công (Positive)
- Given: sách tồn tại, dữ liệu cập nhật hợp lệ
- When: PUT /api/books/{id}
- Then: 200 OK, trạng thái tự tính lại theo totalCopies mới

## TC-08 Cập nhật sách không tồn tại (Negative)
- Given: id không tồn tại
- When: PUT /api/books/{id}
- Then: 404 Not Found

## TC-09 Xóa sách thành công (Positive)
- Given: sách tồn tại
- When: DELETE /api/books/{id}
- Then: 204 No Content, sách không còn trong danh sách

## TC-10 Xóa sách không tồn tại (Negative)
- Given: id không tồn tại hoặc đã xóa trước đó
- When: DELETE /api/books/{id}
- Then: 404 Not Found

## TC-11 Member/Guest gọi API sách (Permission)
- Given: token của tài khoản role Member, hoặc không có token
- When: GET/POST/PUT/DELETE /api/books
- Then: 403 Forbidden (có token role Member) hoặc 401 Unauthorized (không có token)

## TC-12 Thêm thể loại thành công (Positive)
- Given: tên thể loại chưa tồn tại
- When: POST /api/categories
- Then: 200 OK, trả về thể loại vừa tạo

## TC-13 Thêm thể loại trùng tên (Negative)
- Given: tên thể loại đã tồn tại (không phân biệt hoa/thường)
- When: POST /api/categories
- Then: 409 Conflict

## TC-14 Xem danh sách sách (Positive)
- Given: Admin/Employee đã đăng nhập
- When: GET /api/books
- Then: 200 OK, trả về toàn bộ sách sắp xếp theo ngày tạo mới nhất
