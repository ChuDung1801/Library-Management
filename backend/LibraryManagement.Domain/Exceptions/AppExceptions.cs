namespace LibraryManagement.Domain.Exceptions;

/// <summary>Lỗi nghiệp vụ chung - map sang HTTP 400.</summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException(string message) : base(message) { }
}

/// <summary>Không tìm thấy tài nguyên - map sang HTTP 404.</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

/// <summary>Sai thông tin đăng nhập - map sang HTTP 401.</summary>
public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException(string message = "Tên đăng nhập/email hoặc mật khẩu không đúng.")
        : base(message) { }
}

/// <summary>Dữ liệu đã tồn tại (trùng email/username) - map sang HTTP 409.</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>Không đủ quyền truy cập - map sang HTTP 403.</summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message = "Bạn không có quyền thực hiện thao tác này.")
        : base(message) { }
}
