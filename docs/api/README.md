# API Documentation

Thư mục này chứa tất cả các tài liệu liên quan đến API của FHIR-AI Backend.

## Cấu trúc thư mục

### `/specifications/`
- **Mục đích**: Chứa các đặc tả kỹ thuật chi tiết về API
- **Nội dung**: 
  - FHIR Resource specifications
  - API endpoint definitions
  - Request/Response schemas
  - Authentication & Authorization specs

### `/guides/`
- **Mục đích**: Hướng dẫn sử dụng API cho developers
- **Nội dung**:
  - Integration guides
  - Import/Export guides
  - Best practices
  - Code examples

### `/reports/`
- **Mục đích**: Báo cáo thực hiện và kiểm tra API
- **Nội dung**:
  - Implementation status reports
  - Testing reports
  - Performance reports
  - Cursor Agent execution reports

### `/examples/`
- **Mục đích**: Ví dụ thực tế về sử dụng API
- **Nội dung**:
  - Sample requests/responses
  - Use case examples
  - Integration examples

## Quy ước đặt tên

- **Specifications**: `{resource-type}_specification.md`
- **Guides**: `{feature}_guide.md`
- **Reports**: `{report-type}_{date}.md`
- **Examples**: `{use-case}_example.md`

## Cập nhật tài liệu

Khi thêm tài liệu mới:
1. Đặt file vào thư mục phù hợp
2. Cập nhật README.md này
3. Thêm link trong index nếu cần thiết
