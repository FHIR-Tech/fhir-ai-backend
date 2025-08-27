# Cursor Agent Reports & Documentation

Thư mục này chứa tất cả các báo cáo thực hiện, logs và tài liệu liên quan đến Cursor Agent trong quá trình phát triển FHIR-AI Backend.

## Cấu trúc thư mục

### `/reports/`
- **Mục đích**: Báo cáo thực hiện chi tiết của Cursor Agent
- **Nội dung**:
  - Implementation progress reports
  - Code generation reports
  - Architecture decisions
  - Feature completion reports

### `/logs/`
- **Mục đích**: Logs và transcripts của các phiên làm việc với Cursor Agent
- **Nội dung**:
  - Conversation transcripts
  - Code generation logs
  - Error logs
  - Performance logs

### `/decisions/`
- **Mục đích**: Ghi lại các quyết định kiến trúc và thiết kế
- **Nội dung**:
  - Architecture Decision Records (ADRs)
  - Design decisions
  - Technology choices
  - Implementation strategies

### `/tasks/`
- **Mục đích**: Theo dõi các task và milestone
- **Nội dung**:
  - Task completion reports
  - Milestone achievements
  - Sprint reports
  - Feature delivery reports

## Quy ước đặt tên

- **Reports**: `{feature}_{date}_report.md`
- **Logs**: `{session-type}_{date}_{time}.md`
- **Decisions**: `ADR_{number}_{title}.md`
- **Tasks**: `{task-type}_{date}_summary.md`

## Template cho báo cáo

### Implementation Report Template
```markdown
# Implementation Report: {Feature Name}

## Date: {YYYY-MM-DD}
## Agent: Cursor AI
## Status: {Completed/In Progress/Failed}

## Summary
Brief description of what was implemented

## Changes Made
- List of specific changes
- Files modified/created
- Dependencies added

## Technical Details
- Architecture decisions
- Implementation approach
- Challenges faced

## Testing
- Test coverage
- Validation results
- Issues found

## Next Steps
- Remaining work
- Recommendations
- Follow-up actions
```

## Cập nhật báo cáo

Khi tạo báo cáo mới:
1. Sử dụng template phù hợp
2. Đặt file vào thư mục đúng
3. Cập nhật index nếu cần
4. Thêm metadata (date, agent, status)
