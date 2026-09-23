using LibraryManagement.Application.Interfaces;
using LibraryManagement.Domain.Exceptions;

namespace LibraryManagement.Application.Validators;

public static class CategoryValidator
{
    public static void ValidateCreate(CreateCategoryModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name) || model.Name.Length > 100)
            throw new BusinessRuleException("Tên thể loại phải có độ dài từ 1 đến 100 ký tự.");
    }
}
