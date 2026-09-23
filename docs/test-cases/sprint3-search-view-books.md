# Test Case — Sprint 3: Search & View Books (SCRUM-37..41)

## TC-01 Admin tìm kiếm sách theo tên (Positive)
- Given: Admin đã đăng nhập, có sách "The Elegant Universe" trong hệ thống
- When: GET /api/books?keyword=elegant
- Then: 200 OK, trả về sách khớp (không phân biệt hoa/thường)

## TC-02 Admin tìm kiếm không có kết quả (Negative)
- Given: từ khóa không khớp sách nào
- When: GET /api/books?keyword=xyzxyz123
- Then: 200 OK, danh sách rỗng ([])

## TC-03 Admin xem danh sách không truyền keyword (Positive/Boundary)
- Given: không truyền query keyword
- When: GET /api/books
- Then: 200 OK, trả về toàn bộ sách (tương đương SCRUM-36)

## TC-04 Guest xem danh sách sách không cần đăng nhập (Positive/Permission)
- Given: không có Authorization header
- When: GET /api/catalog/books
- Then: 200 OK (không phải 401) — đúng nghiệp vụ Guest được xem tự do

## TC-05 Member tìm kiếm sách qua catalog công khai (Positive)
- Given: có/không có token đều được
- When: GET /api/catalog/books?keyword=sapiens
- Then: 200 OK, trả về sách khớp kèm categoryName đã resolve

## TC-06 Xem chi tiết sách hợp lệ (Positive)
- Given: id sách tồn tại
- When: GET /api/catalog/books/{id}
- Then: 200 OK, trả về đủ field bao gồm status (SCRUM-40)

## TC-07 Xem chi tiết sách không tồn tại (Negative)
- Given: id không tồn tại
- When: GET /api/catalog/books/{id}
- Then: 404 Not Found

## TC-08 Lọc theo thể loại (Positive)
- Given: categoryId hợp lệ, có sách thuộc thể loại đó
- When: GET /api/catalog/books?categoryId={id}
- Then: 200 OK, chỉ trả về sách thuộc thể loại đã chọn

## TC-09 Guest/Member không thể sửa/xóa qua route catalog (Permission)
- Given: CatalogController chỉ có GET, không có POST/PUT/DELETE
- When: thử POST/PUT/DELETE tới /api/catalog/books
- Then: 404/405 — xác nhận route không tồn tại, catalog chỉ đọc

## TC-10 Trạng thái "Không khả dụng" hiển thị đúng (Boundary)
- Given: sách có totalCopies = 0 (Status = Unavailable, xem Sprint 2 TC-05)
- When: GET /api/catalog/books/{id}
- Then: status = "Unavailable", frontend hiển thị chấm đỏ "Không khả dụng"
