# FHIR-AI Backend Documentation Index

Đây là index tổng thể cho tất cả tài liệu của dự án FHIR-AI Backend.

## 📁 Cấu trúc thư mục

```
docs/
├── README.md                    # Tổng quan dự án
├── INDEX.md                     # File này - Index tổng thể
├── api/                         # Tài liệu API
│   ├── README.md               # Hướng dẫn API
│   ├── specifications/         # Đặc tả kỹ thuật
│   │   └── FHIR_ENDPOINTS_IMPLEMENTATION.md
│   ├── guides/                 # Hướng dẫn sử dụng
│   │   ├── AUTHENTICATION_GUIDE.md
│   │   └── FHIR_IMPORT_GUIDE.md
│   └── reports/                # Báo cáo API
│       └── ENDPOINT_VERIFICATION_REPORT.md
├── architecture/               # Tài liệu kiến trúc
│   ├── CLEAN_ARCHITECTURE_REFERENCE.md
│   ├── CQRS_PATTERN_REFERENCE.md
│   ├── MEDIATR_IO_PATTERN_REFERENCE.md
│   ├── AUTOMAPPER_PATTERN_REFERENCE.md
│   └── HEALTHCARE_DATA_PATTERN_REFERENCE.md
├── cursor-agent/               # Báo cáo Cursor Agent
│   ├── README.md              # Hướng dẫn Cursor Agent
│   ├── reports/               # Báo cáo thực hiện
│   │   └── template_implementation_report.md
│   ├── logs/                  # Logs và transcripts
│   ├── decisions/             # Quyết định kiến trúc
│   ├── tasks/                 # Theo dõi task
│   └── workflows/             # GitHub Actions workflows documentation
│       ├── README.md          # Workflows overview
│       └── FIXES.md           # Workflow fixes and troubleshooting
└── deployment/                 # Tài liệu triển khai
    └── SECURITY_CHECKLIST.md
```

## 🔗 Quick Links

### API Documentation
- [API Overview](api/README.md)
- [FHIR Endpoints Specification](api/specifications/FHIR_ENDPOINTS_IMPLEMENTATION.md)
- [FHIR Import Guide](api/guides/FHIR_IMPORT_GUIDE.md)
- [Authentication Guide](api/guides/AUTHENTICATION_GUIDE.md)
- [Endpoint Verification Report](api/reports/ENDPOINT_VERIFICATION_REPORT.md)

### Cursor Agent Reports
- [Cursor Agent Overview](cursor-agent/README.md)
- [Implementation Report Template](cursor-agent/reports/template_implementation_report.md)
- [JWT Refresh Token Implementation Report](cursor-agent/reports/jwt_refresh_token_implementation_report.md)
- [Service Implementation Analysis Report](cursor-agent/reports/service_implementation_analysis_report.md)
- [GitHub Actions Workflows](cursor-agent/workflows/README.md)
- [Workflow Fixes](cursor-agent/workflows/FIXES.md)

### Architecture
- [Clean Architecture Reference](architecture/CLEAN_ARCHITECTURE_REFERENCE.md)
- [CQRS Pattern Reference](architecture/CQRS_PATTERN_REFERENCE.md)
- [MediatR I/O Pattern Reference](architecture/MEDIATR_IO_PATTERN_REFERENCE.md)
- [AutoMapper Pattern Reference](architecture/AUTOMAPPER_PATTERN_REFERENCE.md)
- [Healthcare Data Pattern Reference](architecture/HEALTHCARE_DATA_PATTERN_REFERENCE.md)

### Deployment
- [Security Checklist](deployment/SECURITY_CHECKLIST.md)

## 📋 Cập nhật Index

Khi thêm tài liệu mới:
1. Đặt file vào thư mục phù hợp
2. Cập nhật cấu trúc thư mục ở trên
3. Thêm link vào Quick Links nếu cần
4. Cập nhật ngày sửa đổi

## 📅 Lịch sử cập nhật

- **2024-12-19**: Tạo cấu trúc thư mục mới cho API và Cursor Agent
- **2024-12-28**: Tổ chức lại GitHub Actions workflows documentation
- **2025-08-28**: Dọn dẹp documentation - xóa các reports cũ và tài liệu dư thừa
- **2025-08-28**: Đơn giản hóa cấu trúc, chỉ giữ lại tài liệu cần thiết
- **2025-08-28**: Thêm Clean Architecture Reference Guide với các tiêu chuẩn bất biến
- **2025-08-28**: Thêm CQRS Pattern Reference Guide với các tiêu chuẩn bất biến
- **2025-08-28**: Thêm AutoMapper Pattern Reference Guide với các tiêu chuẩn bất biến
- **2025-08-28**: Thêm Healthcare Data Pattern Reference Guide với các tiêu chuẩn bất biến
- **2025-08-28**: Enhanced Cursor AI Rules với Pattern Recognition System
  - **API Layer Pattern Recognition**: FHIR → Minimal API, Business → Controller
  - **Automatic Pattern Application**: Dựa trên trigger keywords
  - **Quality Gates Enhancement**: Pattern validation và FHIR compliance
- **2025-08-28**: Cập nhật Clean Architecture Reference
  - **Loại bỏ ví dụ hardcode**: Tập trung vào nguyên tắc cốt lõi
  - **Thêm Official I/O Patterns**: Từ Uncle Bob, Greg Young, Jimmy Bogard, FluentValidation
  - **Chuẩn hóa Base Request/Response**: PageNumber, PageSize, SortBy, SortOrder, SearchTerm
  - **Cải tiến Implementation Patterns**: Mô tả tổng quát thay vì code cụ thể
- **2025-08-28**: Cập nhật CQRS Pattern Reference
  - **Loại bỏ ví dụ hardcode**: Tập trung vào nguyên tắc CQRS
  - **Thêm Official I/O Patterns**: Từ Greg Young và Jimmy Bogard
  - **Chuẩn hóa Command/Query Patterns**: BaseCommand, BaseQuery, BasePagedQuery
  - **Cải tiến Response Patterns**: BaseCommandResponse, BaseQueryResponse, PagedQueryResponse
  - **Thêm FluentValidation Standards**: BaseValidator, PaginationValidator
- **2025-08-28**: Tạo MediatR I/O Pattern Reference
  - **Tách riêng MediatR I/O Pattern**: Tài liệu chuyên biệt cho MediatR implementation
  - **Cập nhật Clean Architecture Reference**: Loại bỏ chi tiết MediatR, chỉ giữ nguyên tắc
  - **Cập nhật CQRS Pattern Reference**: Tham chiếu đến MediatR I/O Pattern document
  - **Chuẩn hóa MediatR Implementation**: BaseRequest, BaseResponse, Pipeline Behaviors

---

*Cập nhật lần cuối: 2025-08-28*
