-- =========================================================================================
-- GODOTXR SEED DATA SCRIPT FOR POSTGRESQL (SUPABASE / VPS)
-- Tự động chèn dữ liệu mẫu cho toàn bộ hệ thống GodotXR:
-- 1. Roles (Admin, Teacher, Parent)
-- 2. Users (Admin, 2 Giáo viên, 3 Phụ huynh) - Mật khẩu mẫu: Admin@123
-- 3. SchoolYears & Semesters (Năm học & Học kỳ)
-- 4. Programs & Lessons (Chương trình & Bài học VR)
-- 5. ItemAssets & LessonSlots (Mô hình 3D, Hình ảnh, Âm thanh & Slot bài học)
-- 6. Classrooms (Lớp học)
-- 7. ChildProfiles & Enrollments (Hồ sơ trẻ em & Ghi danh lớp học)
-- 8. Results, PronunciationDetails & ChildSpeechAccuracies (Kết quả học & Chấm điểm phát âm)
-- 9. Analyzes & Reports (Đánh giá chuyên gia & Báo cáo)
-- =========================================================================================

BEGIN;

-- 1. ROLES
INSERT INTO "Roles" ("Id", "RoleName", "Description", "IsActive", "CreatedAt", "IsDeleted")
VALUES 
    (1, 'Admin', 'System Administrator', TRUE, NOW(), FALSE),
    (2, 'Teacher', 'Teacher / Specialist', TRUE, NOW(), FALSE),
    (3, 'Parent', 'Parent / Guardian', TRUE, NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE 
SET "RoleName" = EXCLUDED."RoleName", "Description" = EXCLUDED."Description";

-- 2. USERS
-- Mật khẩu mặc định: Hash BCrypt chuẩn tương thích hệ thống
-- Email đăng nhập test:
-- Admin:    admin@godotxr.com      | Mật khẩu: Admin@123 (hoặc 123456)
-- Teacher:  teacher.mai@godotxr.com | Mật khẩu: Admin@123
-- Teacher:  teacher.tuan@godotxr.com| Mật khẩu: Admin@123
-- Parent:   parent.nam@godotxr.com  | Mật khẩu: Admin@123
-- Parent:   parent.ha@godotxr.com   | Mật khẩu: Admin@123
-- Parent:   parent.tri@godotxr.com  | Mật khẩu: Admin@123
INSERT INTO "Users" (
    "Id", "FullName", "Email", "Phone", "PasswordHash", "RoleId", 
    "IsActive", "IsEmailVerified", "MustChangePassword", "Gender", "Specialty", "CreatedAt", "IsDeleted"
)
VALUES 
    (1, 'System Admin', 'admin@godotxr.com', '0123456789', '$2a$12$fm/KNqTHvTPZ.Xp17lMgpO7C5oJzF9lUi9niXMOX5mriX6xG9VPdu', 1, TRUE, TRUE, FALSE, 'Nam', 'Quản trị hệ thống', NOW(), FALSE),
    (2, 'Cô Nguyễn Thị Mai', 'teacher.mai@godotxr.com', '0912345671', '$2a$12$fm/KNqTHvTPZ.Xp17lMgpO7C5oJzF9lUi9niXMOX5mriX6xG9VPdu', 2, TRUE, TRUE, FALSE, 'Nữ', 'Giáo dục Mầm non & Can thiệp sớm', NOW(), FALSE),
    (3, 'Thầy Trần Văn Tuấn', 'teacher.tuan@godotxr.com', '0912345672', '$2a$12$fm/KNqTHvTPZ.Xp17lMgpO7C5oJzF9lUi9niXMOX5mriX6xG9VPdu', 2, TRUE, TRUE, FALSE, 'Nam', 'Âm ngữ trị liệu cho trẻ chậm nói', NOW(), FALSE),
    (4, 'Lê Văn Nam', 'parent.nam@godotxr.com', '0988776651', '$2a$12$fm/KNqTHvTPZ.Xp17lMgpO7C5oJzF9lUi9niXMOX5mriX6xG9VPdu', 3, TRUE, TRUE, FALSE, 'Nam', 'Phụ huynh bé Bo', NOW(), FALSE),
    (5, 'Hoàng Thu Hà', 'parent.ha@godotxr.com', '0988776652', '$2a$12$fm/KNqTHvTPZ.Xp17lMgpO7C5oJzF9lUi9niXMOX5mriX6xG9VPdu', 3, TRUE, TRUE, FALSE, 'Nữ', 'Phụ huynh bé Bông', NOW(), FALSE),
    (6, 'Phạm Minh Trí', 'parent.tri@godotxr.com', '0988776653', '$2a$12$fm/KNqTHvTPZ.Xp17lMgpO7C5oJzF9lUi9niXMOX5mriX6xG9VPdu', 3, TRUE, TRUE, FALSE, 'Nam', 'Phụ huynh bé Tin', NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE 
SET "FullName" = EXCLUDED."FullName", "Email" = EXCLUDED."Email", "PasswordHash" = EXCLUDED."PasswordHash", "IsEmailVerified" = TRUE;

-- 3. SCHOOL YEARS (NĂM HỌC)
INSERT INTO "SchoolYears" ("Id", "YearName", "ClassCount", "StartDate", "EndDate", "Status", "CreatedAt", "IsDeleted")
VALUES 
    (1, 'Năm học 2025 - 2026', 2, '2025-09-01 00:00:00', '2026-05-31 23:59:59', 'Active', NOW(), FALSE),
    (2, 'Năm học 2026 - 2027', 0, '2026-09-01 00:00:00', '2027-05-31 23:59:59', 'Planning', NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "YearName" = EXCLUDED."YearName";

-- 4. SEMESTERS (HỌC KỲ)
INSERT INTO "Semesters" ("Id", "SemesterName", "SchoolYearId", "TeacherId", "ClassCount", "Description", "StartDate", "EndDate", "Status", "CreatedAt", "IsDeleted")
VALUES 
    (1, 'Học kỳ 1 (2025 - 2026)', 1, 2, 2, 'Giai đoạn làm quen với không gian thực tế ảo và nhận diện đồ vật cơ bản', '2025-09-01 00:00:00', '2026-01-15 23:59:59', 'Active', NOW(), FALSE),
    (2, 'Học kỳ 2 (2025 - 2026)', 1, 3, 2, 'Giai đoạn tập trung can thiệp phát âm chuẩn và ghép từ thành câu', '2026-01-16 00:00:00', '2026-05-31 23:59:59', 'Active', NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "SemesterName" = EXCLUDED."SemesterName";

-- 5. PROGRAMS (CHƯƠNG TRÌNH HỌC)
INSERT INTO "Programs" ("Id", "ProgramName", "Description", "TargetAgeFrom", "TargetAgeTo", "Language", "Status", "CreatedAt", "IsDeleted")
VALUES 
    (1, 'Khám phá Thế giới Động vật VR', 'Chương trình rèn luyện kỹ năng ngôn ngữ và nhận biết động vật thông qua không gian tương tác 3D sinh động.', 3, 6, 'vi-VN', 'Active', NOW(), FALSE),
    (2, 'Luyện phát âm Tiếng Việt VR', 'Chương trình can thiệp phát âm các phụ âm đầu khó như /b/, /v/, /d/, /l/, /n/ cho trẻ chậm nói.', 4, 7, 'vi-VN', 'Active', NOW(), FALSE),
    (3, 'Thế giới Rau củ Quả & Đồ vật Quanh em', 'Nhận diện và tập phát âm tên các loại trái cây, đồ dùng quen thuộc trong cuộc sống hàng ngày.', 3, 5, 'vi-VN', 'Active', NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "ProgramName" = EXCLUDED."ProgramName";

-- 6. LESSONS (BÀI HỌC)
INSERT INTO "Lessons" (
    "Id", "ProgramId", "LessonName", "LessonOrder", "Description", "TargetSkill", 
    "EstimatedDuration", "Status", "MaxScore", "CompletionBonusPoints", "CorrectAnswerScore", "IncorrectAnswerScore", "CreatedAt", "IsDeleted"
)
VALUES 
    (1, 1, 'Bài 1: Động vật nông trại thân quen', 1, 'Bé học tên gọi và phát âm tiếng kêu của các con vật trong nông trại.', 'Nhận biết & Phát âm', 15, 'Active', 100, 20, 10, 0, NOW(), FALSE),
    (2, 1, 'Bài 2: Sinh vật biển kỳ thú', 2, 'Bé bơi lặn cùng các loài sinh vật biển rực rỡ sắc màu.', 'Phát âm mô tả', 20, 'Active', 100, 20, 10, 0, NOW(), FALSE),
    (3, 2, 'Bài 1: Luyện âm môi /b/ và /m/', 1, 'Rèn luyện chuyển động cơ môi khi phát âm các từ có phụ âm đầu b và m.', 'Âm vị học cơ bản', 15, 'Active', 100, 20, 10, 0, NOW(), FALSE),
    (4, 2, 'Bài 2: Luyện âm răng môi /v/ và /f/', 2, 'Hướng dẫn cách đặt răng trên chạm môi dưới để phát âm chuẩn âm v.', 'Âm vị học nâng cao', 15, 'Active', 100, 20, 10, 0, NOW(), FALSE),
    (5, 3, 'Bài 1: Vườn trái cây ngọt ngào', 1, 'Khám phá và gọi tên các loại quả nhiều màu sắc.', 'Từ vựng & Màu sắc', 12, 'Active', 100, 20, 10, 0, NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "LessonName" = EXCLUDED."LessonName";

-- 7. ITEM ASSETS (3D MODELS, HÌNH ẢNH & ÂM THANH CHO CLIENT VR)
INSERT INTO "ItemAssets" ("Id", "Name", "AnswerSentence", "ModelUrl", "ImageUrl", "AudioUrl", "CreatedAt", "IsDeleted")
VALUES 
    (1, 'Con Bò Sữa', 'Đây là con bò sữa', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/models/cow.glb', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/images/cow.png', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/audios/cow.mp3', NOW(), FALSE),
    (2, 'Con Gà Trống', 'Con gà trống gáy ò ó o', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/models/chicken.glb', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/images/chicken.png', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/audios/chicken.mp3', NOW(), FALSE),
    (3, 'Con Vịt Trắng', 'Con vịt bơi dưới nước', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/models/duck.glb', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/images/duck.png', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/audios/duck.mp3', NOW(), FALSE),
    (4, 'Con Cá Heo', 'Con cá heo thông minh', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/models/dolphin.glb', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/images/dolphin.png', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/audios/dolphin.mp3', NOW(), FALSE),
    (5, 'Con Rùa Biển', 'Con rùa bò chậm chạp', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/models/turtle.glb', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/images/turtle.png', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/audios/turtle.mp3', NOW(), FALSE),
    (6, 'Quả Táo', 'Quả táo màu đỏ ngọt ngào', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/models/apple.glb', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/images/apple.png', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/audios/apple.mp3', NOW(), FALSE),
    (7, 'Quả Chuối', 'Quả chuối màu vàng chín thơm', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/models/banana.glb', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/images/banana.png', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/audios/banana.mp3', NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "Name" = EXCLUDED."Name", "AnswerSentence" = EXCLUDED."AnswerSentence";

-- 8. LESSON SLOTS (CẤU HÌNH VỊ TRÍ TƯƠNG TÁC CHO BÀI HỌC)
INSERT INTO "LessonSlots" ("Id", "LessonId", "SlotName", "ItemAssetId", "CorrectPoints", "WrongPoints", "CreatedAt", "IsDeleted")
VALUES 
    (1, 1, 'Slot 1 - Con Bò Sữa', 1, 10, 0, NOW(), FALSE),
    (2, 1, 'Slot 2 - Con Gà Trống', 2, 10, 0, NOW(), FALSE),
    (3, 1, 'Slot 3 - Con Vịt Trắng', 3, 10, 0, NOW(), FALSE),
    (4, 2, 'Slot 1 - Con Cá Heo', 4, 10, 0, NOW(), FALSE),
    (5, 2, 'Slot 2 - Con Rùa Biển', 5, 10, 0, NOW(), FALSE),
    (6, 3, 'Slot 1 - Âm B (Con Bò)', 1, 10, 0, NOW(), FALSE),
    (7, 4, 'Slot 1 - Âm V (Con Vịt)', 3, 10, 0, NOW(), FALSE),
    (8, 5, 'Slot 1 - Quả Táo Đỏ', 6, 10, 0, NOW(), FALSE),
    (9, 5, 'Slot 2 - Quả Chuối Vàng', 7, 10, 0, NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "SlotName" = EXCLUDED."SlotName";

-- 9. CLASSROOMS (LỚP HỌC)
INSERT INTO "Classrooms" ("Id", "UserId", "ProgramId", "SemesterId", "ClassName", "Description", "StartDate", "EndDate", "Status", "CreatedAt", "IsDeleted")
VALUES 
    (1, 2, 1, 1, 'Lớp Mầm 1 - Ban Mai Rực Rỡ', 'Lớp học tương tác thế giới động vật dành cho các bé từ 3 - 4 tuổi.', '2025-09-05 08:00:00', '2026-01-10 17:00:00', 'Active', NOW(), FALSE),
    (2, 3, 2, 1, 'Lớp Chồi 2 - Hướng Dương Tự Tin', 'Lớp can thiệp chuyên biệt chỉnh âm lời nói và phát âm tròn vành rõ chữ.', '2025-09-05 08:00:00', '2026-01-10 17:00:00', 'Active', NOW(), FALSE),
    (3, 3, 2, 2, 'Lớp Can Thiệp 1-1 Cá Nhân Hóa', 'Lớp học trị liệu ngôn ngữ 1 kèm 1 cùng chuyên gia âm ngữ.', '2026-01-20 08:00:00', '2026-05-25 17:00:00', 'Active', NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "ClassName" = EXCLUDED."ClassName";

-- 10. CHILD PROFILES (HỒ SƠ TRẺ EM)
INSERT INTO "ChildProfiles" ("Id", "UserId", "FullName", "Age", "Gender", "LearningLevel", "ChildType", "Note", "Status", "CreatedAt", "IsDeleted")
VALUES 
    (1, 4, 'Lê Bảo Nam (Bé Bo)', 4, 'Nam', 'Cơ bản', 'Trẻ chậm nói nhẹ', 'Bé hào hứng khi tương tác với kính VR, thích các loài động vật.', 'Active', NOW(), FALSE),
    (2, 5, 'Trần Hoàng Mai Anh (Bé Bông)', 5, 'Nữ', 'Trung bình', 'Trẻ cần can thiệp phát âm', 'Bé hay nói ngọng các phụ âm đầu v và d, phản xạ ngôn ngữ tốt.', 'Active', NOW(), FALSE),
    (3, 6, 'Phạm Minh Quân (Bé Tin)', 4, 'Nam', 'Cơ bản', 'Trẻ phát triển bình thường', 'Bé tiếp thu rất nhanh, thích khám phá thế giới tự nhiên.', 'Active', NOW(), FALSE),
    (4, 4, 'Lê Bảo An (Bé Miu)', 3, 'Nữ', 'Mới bắt đầu', 'Trẻ chậm nói', 'Bé cần thời gian làm quen và cần giáo viên đồng hành kiên nhẫn.', 'Active', NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "FullName" = EXCLUDED."FullName";

-- 11. ENROLLMENTS (GHI DANH HỌC SINH VÀO LỚP)
INSERT INTO "Enrollments" ("Id", "ChildId", "ClassId", "EnrollmentDate", "Status", "CreatedAt", "IsDeleted")
VALUES 
    (1, 1, 1, '2025-09-05 08:30:00', 'Active', NOW(), FALSE),
    (2, 2, 2, '2025-09-05 09:00:00', 'Active', NOW(), FALSE),
    (3, 3, 1, '2025-09-06 08:30:00', 'Active', NOW(), FALSE),
    (4, 4, 3, '2026-01-20 09:00:00', 'Active', NOW(), FALSE)
ON CONFLICT ("Id") DO NOTHING;

-- 12. RESULTS (KẾT QUẢ PHIÊN LUYỆN TẬP / HỌC TẬP TRONG VR)
INSERT INTO "Results" (
    "Id", "ChildId", "LessonId", "IsExercise", "AttemptNumber", "CompletionStatus", 
    "Score", "ErrorCount", "CorrectCount", "DurationSeconds", "SessionId", 
    "StartedAt", "CompletedAt", "IsFinalized", "FeedbackText", "CreatedAt", "IsDeleted"
)
VALUES 
    (1, 1, 1, FALSE, 1, 'Completed', 85.5, 2, 8, 420, 'session-bo-lesson1-001', '2026-10-10 09:00:00', '2026-10-10 09:07:00', TRUE, 'Bé phát âm tốt từ "Con bò sữa" và "Con vịt", cần chú ý âm gió.', NOW(), FALSE),
    (2, 2, 3, FALSE, 1, 'Completed', 72.0, 4, 6, 510, 'session-bong-lesson3-001', '2026-10-12 14:30:00', '2026-10-12 14:38:30', TRUE, 'Bé đã biết mím môi khi phát âm âm /m/, âm /b/ cần khẩu hình mạnh hơn một chút.', NOW(), FALSE),
    (3, 3, 1, FALSE, 1, 'Completed', 95.0, 0, 10, 360, 'session-tin-lesson1-001', '2026-10-15 10:15:00', '2026-10-15 10:21:00', TRUE, 'Bé hoàn thành xuất sắc tất cả các thử thách phát âm trong bài.', NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "Score" = EXCLUDED."Score";

-- 13. PRONUNCIATION DETAILS (CHI TIẾT ÂM VỊ ĐƯỢC AZURE SPEECH ĐÁNH GIÁ)
INSERT INTO "PronunciationDetails" ("Id", "ResultId", "ExpectedPhoneme", "ActualPhoneme", "AccuracyScore", "IssueType", "CreatedAt", "IsDeleted")
VALUES 
    (1, 1, 'b', 'b', 92, 'None', NOW(), FALSE),
    (2, 1, 'ɔ', 'ɔ', 88, 'None', NOW(), FALSE),
    (3, 1, 's', 'th', 55, 'Mispronunciation', NOW(), FALSE),
    (4, 2, 'b', 'p', 50, 'Substitution', NOW(), FALSE),
    (5, 2, 'm', 'm', 85, 'None', NOW(), FALSE)
ON CONFLICT ("Id") DO NOTHING;

-- 14. CHILD SPEECH ACCURACIES (LƯU LỊCH SỬ CHẤM ĐIỂM CHI TIẾT TỪNG TỪ)
INSERT INTO "ChildSpeechAccuracies" (
    "Id", "ChildProfileId", "LessonId", "LessonSlotId", "ResultId", "SessionId", 
    "Word", "AccuracyScore", "FluencyScore", "PronunciationScore", "CompletenessScore", 
    "ErrorType", "SpeechErrorCategory", "AudioChunkIndex", "CreatedAt", "IsDeleted"
)
VALUES 
    (1, 1, 1, 1, 1, 'session-bo-lesson1-001', 'Con bò sữa', 88.0, 85.0, 88.0, 100.0, 'None', 'Normal', 0, NOW(), FALSE),
    (2, 1, 1, 2, 1, 'session-bo-lesson1-001', 'Gà trống', 75.0, 70.0, 75.0, 95.0, 'Mispronunciation', 'Consonant', 1, NOW(), FALSE),
    (3, 2, 3, 6, 2, 'session-bong-lesson3-001', 'Bé mím môi', 72.0, 68.0, 72.0, 90.0, 'None', 'Phoneme', 0, NOW(), FALSE)
ON CONFLICT ("Id") DO NOTHING;

-- 15. ANALYZES (CHẨN ĐOÁN & ĐÁNH GIÁ CỦA CHUYÊN GIA)
INSERT INTO "Analyzes" (
    "Id", "ChildId", "SpeechLevel", "Diagnosis", "Difficulties", "Strengths", "Weaknesses", 
    "CommunicationAbility", "PronunciationAbility", "LanguageComprehension", "LanguageExpression", "AttentionLevel", "SocialInteraction", 
    "InterventionGoals", "Recommendation", "Notes", "AssessmentDate", "NextAssessmentDate", "CreatedAt", "IsDeleted"
)
VALUES 
    (1, 1, 1, 'Chậm phát triển ngôn ngữ mức độ nhẹ', 'Khó diễn đạt các câu dài trên 5 từ, hay nói nhát gừng', 'Nhận diện hình ảnh và tương tác 3D rất trực quan, phản xạ vui vẻ', 'Dễ mất tập trung nếu bài học quá dài', 2, 2, 3, 2, 2, 3, 'Cải thiện độ rõ chữ của phụ âm đầu, mở rộng vốn từ 50 từ vựng mới', 'Tập luyện 15 phút mỗi ngày với các bài học nông trại VR', 'Gia đình cần khuyến khích bé nói chuyện ở nhà', '2026-10-01 08:30:00', '2026-12-01 08:30:00', NOW(), FALSE),
    (2, 2, 2, 'Rối loạn âm lời nói (ngọng âm v và d)', 'Thường thay thế âm /v/ bằng /d/ hoặc /b/', 'Biểu cảm tự nhiên, vốn từ phong phú, thích ca hát', 'Sai cấu âm khẩu hình môi răng', 3, 1, 3, 3, 3, 3, 'Sửa ngọng âm /v/, định hình vị trí đặt răng trên và môi dưới chính xác', 'Tập trung các bài tập luyện cơ môi và bài học số 4 trong chương trình', 'Tiến bộ rõ rệt qua các buổi học VR', '2026-10-05 10:00:00', '2026-12-05 10:00:00', NOW(), FALSE)
ON CONFLICT ("Id") DO UPDATE SET "Diagnosis" = EXCLUDED."Diagnosis";

-- 16. REPORTS (BÁO CÁO XUẤT RA DÀNH CHO PHỤ HUYNH)
INSERT INTO "Reports" ("Id", "AnalyzeId", "GeneratedBy", "ReportFormat", "FileUrl", "CreatedAt", "IsDeleted")
VALUES 
    (1, 1, 2, 'PDF', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/reports/report_bo_nam.pdf', NOW(), FALSE),
    (2, 2, 3, 'PDF', 'https://pub-832f91ebff014ad3b482ca255cb0f9dd.r2.dev/reports/report_bong_mai_anh.pdf', NOW(), FALSE)
ON CONFLICT ("Id") DO NOTHING;

-- =========================================================================================
-- ĐỒNG BỘ LẠI TẤT CẢ SEQUENCE IDENTITY CHO POSTGRESQL (TRÁNH LỖI DUPLICATE KEY KHI TẠO MỚI)
-- =========================================================================================
SELECT setval(pg_get_serial_sequence('"Roles"', 'Id'), coalesce((SELECT MAX("Id") FROM "Roles"), 1));
SELECT setval(pg_get_serial_sequence('"Users"', 'Id'), coalesce((SELECT MAX("Id") FROM "Users"), 1));
SELECT setval(pg_get_serial_sequence('"SchoolYears"', 'Id'), coalesce((SELECT MAX("Id") FROM "SchoolYears"), 1));
SELECT setval(pg_get_serial_sequence('"Semesters"', 'Id'), coalesce((SELECT MAX("Id") FROM "Semesters"), 1));
SELECT setval(pg_get_serial_sequence('"Programs"', 'Id'), coalesce((SELECT MAX("Id") FROM "Programs"), 1));
SELECT setval(pg_get_serial_sequence('"Lessons"', 'Id'), coalesce((SELECT MAX("Id") FROM "Lessons"), 1));
SELECT setval(pg_get_serial_sequence('"ItemAssets"', 'Id'), coalesce((SELECT MAX("Id") FROM "ItemAssets"), 1));
SELECT setval(pg_get_serial_sequence('"LessonSlots"', 'Id'), coalesce((SELECT MAX("Id") FROM "LessonSlots"), 1));
SELECT setval(pg_get_serial_sequence('"Classrooms"', 'Id'), coalesce((SELECT MAX("Id") FROM "Classrooms"), 1));
SELECT setval(pg_get_serial_sequence('"ChildProfiles"', 'Id'), coalesce((SELECT MAX("Id") FROM "ChildProfiles"), 1));
SELECT setval(pg_get_serial_sequence('"Enrollments"', 'Id'), coalesce((SELECT MAX("Id") FROM "Enrollments"), 1));
SELECT setval(pg_get_serial_sequence('"Results"', 'Id'), coalesce((SELECT MAX("Id") FROM "Results"), 1));
SELECT setval(pg_get_serial_sequence('"PronunciationDetails"', 'Id'), coalesce((SELECT MAX("Id") FROM "PronunciationDetails"), 1));
SELECT setval(pg_get_serial_sequence('"ChildSpeechAccuracies"', 'Id'), coalesce((SELECT MAX("Id") FROM "ChildSpeechAccuracies"), 1));
SELECT setval(pg_get_serial_sequence('"Analyzes"', 'Id'), coalesce((SELECT MAX("Id") FROM "Analyzes"), 1));
SELECT setval(pg_get_serial_sequence('"Reports"', 'Id'), coalesce((SELECT MAX("Id") FROM "Reports"), 1));

COMMIT;
