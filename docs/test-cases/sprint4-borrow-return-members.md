# Test Case — Sprint 4: Member Management (Admin) + Borrow & Return

## Member Management (SCRUM-24, 42)

### TC-01 Admin xem danh sách thành viên (Positive)
- Given: Admin đã đăng nhập
- When: GET /api/members
- Then: 200 OK, chỉ trả về user có role = Member (không lẫn Admin/Employee)

### TC-02 Admin cập nhật thông tin thành viên (Positive)
- Given: memberId tồn tại, dữ liệu hợp lệ
- When: PUT /api/members/{id}
- Then: 200 OK, thông tin được cập nhật

### TC-03 Admin cập nhật thành viên không tồn tại (Negative)
- Given: id không tồn tại hoặc không phải role Member (vd: id của Admin khác)
- When: PUT /api/members/{id}
- Then: 404 Not Found

### TC-04 Vô hiệu hóa tài khoản thành viên (Positive)
- Given: memberId tồn tại, đang active
- When: PUT /api/members/{id}/status { "isActive": false }
- Then: 200 OK; thành viên đó đăng nhập lại → 401 (tái sử dụng logic Sprint 1: !user.IsActive)

### TC-05 Member/Guest gọi endpoint quản trị thành viên (Permission)
- Given: token role Member, hoặc không có token
- When: GET /api/members, PUT /api/members/{id}, PUT /api/members/{id}/status
- Then: 403 Forbidden / 401 Unauthorized

## Borrow & Return (SCRUM-43, 44, 45)

### TC-06 Ghi nhận mượn sách thành công (Positive)
- Given: sách còn totalCopies > 0, member tồn tại + đang active
- When: POST /api/borrows { bookId, memberId }
- Then: 201 Created, dueDate = borrowDate + 30 ngày; book.totalCopies giảm 1

### TC-07 Mượn sách đã hết bản (Negative)
- Given: book.totalCopies = 0
- When: POST /api/borrows
- Then: 400 Bad Request

### TC-08 Mượn sách cho tài khoản không phải Member (Negative)
- Given: memberId trỏ tới tài khoản role Admin/Employee
- When: POST /api/borrows
- Then: 400 Bad Request ("Chỉ có thể ghi nhận mượn sách cho tài khoản Thành viên.")

### TC-09 Mượn sách cho tài khoản đã bị vô hiệu hóa (Negative)
- Given: member.isActive = false
- When: POST /api/borrows
- Then: 400 Bad Request

### TC-10 Ghi nhận trả sách thành công (Positive)
- Given: borrowId tồn tại, status = Borrowed
- When: PUT /api/borrows/{id}/return
- Then: 200 OK, status = Returned, returnDate được set; book.totalCopies tăng 1

### TC-11 Trả sách đã trả trước đó (Negative/Boundary)
- Given: borrow.status đã = Returned
- When: PUT /api/borrows/{id}/return
- Then: 400 Bad Request ("Phiếu mượn này đã được ghi nhận trả trước đó.")

### TC-12 Xem danh sách sách quá hạn (Positive)
- Given: có borrow với status=Borrowed và dueDate < hiện tại
- When: GET /api/borrows/overdue
- Then: 200 OK, chỉ trả về các phiếu quá hạn, isOverdue = true

### TC-13 Không có sách quá hạn (Boundary)
- Given: tất cả borrow đang active đều còn hạn hoặc đã trả hết
- When: GET /api/borrows/overdue
- Then: 200 OK, danh sách rỗng

### TC-14 Member/Guest gọi endpoint mượn/trả (Permission)
- Given: token role Member, hoặc không có token
- When: POST/GET/PUT bất kỳ route /api/borrows/*
- Then: 403 Forbidden / 401 Unauthorized
