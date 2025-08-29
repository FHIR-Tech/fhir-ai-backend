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
│   └── CLEAN_ARCHITECTURE_REFERENCE.md
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

---

*Cập nhật lần cuối: 2025-08-28*
