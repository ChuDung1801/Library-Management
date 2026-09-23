# Test Case — Sprint 5: History Tracking & Book Borrowing Requests (SCRUM-46..50)

## SCRUM-46 Admin xem lịch sử mượn sách

### TC-01 Xem toàn bộ lịch sử (Positive)
- Given: Admin đã đăng nhập
- When: GET /api/borrows
- Then: 200 OK, trả về mọi phiếu mượn (đã trả + đang mượn), mới nhất trước

### TC-02 Lọc lịch sử theo thành viên (Positive)
- Given: memberId hợp lệ, thành viên có ít nhất 1 phiếu mượn
- When: GET /api/borrows?memberId={id}
- Then: 200 OK, chỉ trả về phiếu mượn của thành viên đó

## SCRUM-47 Thành viên xem lịch sử mượn sách của mình

### TC-03 Member xem lịch sử của chính mình (Positive)
- Given: Member đã đăng nhập
- When: GET /api/members/me/borrows
- Then: 200 OK, chỉ trả về phiếu mượn của chính user gọi API (không lộ của người khác)

### TC-04 Member chưa từng mượn sách (Boundary)
- Given: Member mới đăng ký, chưa có phiếu mượn nào
- When: GET /api/members/me/borrows
- Then: 200 OK, danh sách rỗng

## SCRUM-48 Thành viên xem sách đang mượn

### TC-05 Xem sách đang mượn (Positive)
- Given: Member có 1 phiếu mượn Status=Borrowed và 1 phiếu đã Returned
- When: GET /api/members/me/borrows/active
- Then: 200 OK, chỉ trả về phiếu đang Borrowed, không có phiếu đã trả

## SCRUM-49 Thành viên gửi yêu cầu mượn sách

### TC-06 Gửi yêu cầu thành công (Positive)
- Given: Member đang active, sách còn totalCopies > 0
- When: POST /api/borrow-requests { bookId }
- Then: 200 OK, status = "Pending"

### TC-07 Gửi yêu cầu cho sách đã hết (Negative)
- Given: book.totalCopies = 0
- When: POST /api/borrow-requests
- Then: 400 Bad Request

### TC-08 Gửi trùng yêu cầu đang chờ duyệt (Negative/Boundary)
- Given: Member đã có 1 request Pending cho đúng cuốn sách này
- When: POST /api/borrow-requests { bookId cũ }
- Then: 409 Conflict ("Bạn đã gửi yêu cầu mượn sách này và đang chờ duyệt.")

### TC-09 Admin/Guest gửi yêu cầu mượn sách (Permission)
- Given: token role Admin/Employee, hoặc không có token
- When: POST /api/borrow-requests
- Then: 403 Forbidden / 401 Unauthorized (route yêu cầu Roles = "Member")

### TC-10 Admin duyệt yêu cầu (Positive)
- Given: request đang Pending, sách vẫn còn totalCopies > 0 tại thời điểm duyệt
- When: PUT /api/borrow-requests/{id}/approve
- Then: 200 OK, status = "Approved", resultingBorrowId có giá trị; một Borrow mới được tạo,
  book.totalCopies giảm 1 (tái sử dụng đúng logic BorrowService.CreateAsync)

### TC-11 Admin từ chối yêu cầu (Positive)
- Given: request đang Pending
- When: PUT /api/borrow-requests/{id}/reject { "note": "Sách đang được sửa chữa" }
- Then: 200 OK, status = "Rejected", adminNote được lưu, không tạo Borrow, không trừ kho

### TC-12 Duyệt/từ chối yêu cầu đã xử lý (Negative/Boundary)
- Given: request.status đã là Approved hoặc Rejected
- When: PUT /api/borrow-requests/{id}/approve hoặc /reject
- Then: 400 Bad Request ("Yêu cầu này đã được xử lý trước đó.")

### TC-13 Member xem yêu cầu của người khác (Permission)
- Given: Member A gọi API, cố xem yêu cầu của Member B
- When: GET /api/borrow-requests/mine (chỉ trả theo token của người gọi, không nhận memberId từ query)
- Then: 200 OK, chỉ thấy yêu cầu của chính mình - không có cách nào truyền id người khác

## SCRUM-50 Khách xem chi tiết sách

### TC-14 Đã hoàn thành từ Sprint 3 (Regression check)
- Given: không có Authorization header
- When: GET /api/catalog/books/{id}
- Then: 200 OK - xác nhận lại tính năng vẫn hoạt động đúng sau các thay đổi Sprint 4/5
  (Book.TotalCopies bị Borrow/Return chỉnh sửa nhưng không phá vỡ endpoint công khai này)
