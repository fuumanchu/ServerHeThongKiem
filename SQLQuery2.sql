-- 1. Chèn thông tin thiết bị (Trang 3)
-- Lưu ý: EF Core thường tạo tên bảng là 'Devices' dựa trên tên DbSet trong DbContext
INSERT INTO [Devices] ([DeviceID], [DeviceName], [Customer], [Address], [Phone], [CreateDate], [Status], [IsAutoMode])
VALUES 
('TH0001', N'Hệ Thống Kiềm Trường Học', N'Trường THCS Hùng Vương 01', N'Số 01 Phố Nguyễn Không, Phường Số 1, TP Hà Nội', '09868686xx', '2025-12-12', 'Online', 1),
('TH0002', N'Hệ Thống Kiềm Trường Học', N'Trường THPT Chu Văn An', N'Số 10 Thụy Khuê, Tây Hồ, Hà Nội', '02438233xx', GETDATE(), 'Offline', 1);

-- Lấy ID của thiết bị đầu tiên để chèn I/O
DECLARE @DeviceId INT = (SELECT TOP 1 Id FROM Devices WHERE DeviceID = 'TH0001');

-- 2. Chèn dữ liệu Input cho TH0001 (Dựa trên bảng Trang 4)
-- EF Core thường tạo bảng DeviceInput với cột khóa ngoại là DeviceModelId
INSERT INTO [DeviceInput] ([Order], [Name], [Status], [Value], [SettingRange], [DeviceModelId])
VALUES 
(1, N'Phao Đầy 1', 'ON', '1', 'ON/OFF', @DeviceId),           -- 
(2, N'Phao Cạn 1', 'OFF', '0', 'ON/OFF', @DeviceId),          -- 
(3, N'CB TDS 1', 'ON', '85', '50-150', @DeviceId),            -- 
(4, N'CB Áp suất 1', 'ON', '1.2', '0.8-2.5', @DeviceId),       -- 
(5, N'CB Lưu lượng 1', 'ON', '1.5', '0.8-2.5', @DeviceId);    -- 

-- 3. Chèn dữ liệu Output cho TH0001 (Dựa trên bảng Trang 4)
INSERT INTO [DeviceOutput] ([Order], [Name], [Status], [Value], [SettingRange], [DeviceModelId])
VALUES 
(1, N'Bơm ngang', 'ON', '1', 'ON/OFF', @DeviceId),            -- 
(2, N'Bơm RO', 'ON', '1', 'ON/OFF', @DeviceId),               -- 
(3, N'Đèn cảnh báo', 'OFF', '0', 'ON/OFF', @DeviceId),        -- 
(6, N'Van sả đáy UF', 'OFF', '0', 'ON/OFF', @DeviceId),       -- 
(8, N'Bể Điện Phân 1', 'ON', '3.5', '0-5', @DeviceId);        --