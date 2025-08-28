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
│   ├── guides/                 # Hướng dẫn sử dụng
│   ├── reports/                # Báo cáo API
│   └── examples/               # Ví dụ sử dụng
├── architecture/               # Tài liệu kiến trúc
├── cursor-agent/               # Báo cáo Cursor Agent
│   ├── README.md              # Hướng dẫn Cursor Agent
│   ├── reports/               # Báo cáo thực hiện
│   ├── logs/                  # Logs và transcripts
│   ├── decisions/             # Quyết định kiến trúc
│   ├── tasks/                 # Theo dõi task
│   └── workflows/             # GitHub Actions workflows documentation
│       ├── README.md          # Workflows overview
│       └── FIXES.md           # Workflow fixes and troubleshooting
└── deployment/                 # Tài liệu triển khai
```

## 🔗 Quick Links

### API Documentation
- [API Overview](api/README.md)
- [FHIR Endpoints Specification](api/specifications/FHIR_ENDPOINTS_IMPLEMENTATION.md)
- [FHIR Import Guide](api/guides/FHIR_IMPORT_GUIDE.md)
- [Endpoint Verification Report](api/reports/ENDPOINT_VERIFICATION_REPORT.md)
- [FHIR Endpoint Refactoring Report](api/reports/fhir_endpoint_refactoring_2024-12-19_report.md)

### Cursor Agent Reports
- [Cursor Agent Overview](cursor-agent/README.md)
- [Reports Directory](cursor-agent/reports/)
- [Architecture Decisions](cursor-agent/decisions/)
- [Task Tracking](cursor-agent/tasks/)
- [GitHub Actions Workflows](cursor-agent/workflows/README.md)
- [Workflow Fixes](cursor-agent/workflows/FIXES.md)

### Architecture
- [Architecture Documentation](architecture/)

## 📋 Cập nhật Index

Khi thêm tài liệu mới:
1. Đặt file vào thư mục phù hợp
2. Cập nhật cấu trúc thư mục ở trên
3. Thêm link vào Quick Links nếu cần
4. Cập nhật ngày sửa đổi

## 📅 Lịch sử cập nhật

- **2024-12-19**: Tạo cấu trúc thư mục mới cho API và Cursor Agent
- **2024-12-19**: Di chuyển các file hiện có vào thư mục phù hợp
- **2024-12-19**: Tạo templates và quy ước đặt tên
- **2024-12-19**: Di chuyển README_ENDPOINT_REFACTOR.md vào docs/api/reports/ với tên fhir_endpoint_refactoring_2024-12-19_report.md
- **2024-12-28**: Tổ chức lại GitHub Actions workflows documentation, di chuyển vào docs/cursor-agent/workflows/
- **2024-12-28**: Xóa file test-simple.yml và dọn dẹp thư mục .github/workflows/
- **2024-12-28**: Cập nhật documentation structure để tuân thủ quy tắc dự án

---

*Cập nhật lần cuối: 2024-12-19*
