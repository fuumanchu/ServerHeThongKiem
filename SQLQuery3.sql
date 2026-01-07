-- 1. Khởi tạo thông tin thiết bị (Trang 3)
INSERT INTO [Devices] ([DeviceID], [DeviceName], [Customer], [Address], [Phone], [CreateDate], [Status], [IsAutoMode])
VALUES 
('TH0001', N'Hệ Thống Kiềm Trường Học', N'Trường THCS Hùng Vương 01', N'Số 01 Phố Nguyễn Không, Phường Số 1, TP Hà Nội', '09868686xx', '2025-12-12', 'Online', 1);

-- Lấy ID của thiết bị vừa tạo
DECLARE @DeviceId INT = (SELECT TOP 1 Id FROM Devices WHERE DeviceID = 'TH0001');

-- 2. Chèn dữ liệu Input cho TH0001 (Đầy đủ 23 thông số - Trang 4)
INSERT INTO [DeviceInput] ([Order], [Name], [Status], [Value], [SettingRange], [DeviceModelId])
VALUES 
(1, N'Phao Đầy 1', 'OFF', '0', 'ON/OFF', @DeviceId),
(2, N'Phao Cạn 1', 'ON', '1', 'ON/OFF', @DeviceId),
(3, N'CB TDS 1', 'ON', '82', '50 ÷ 150', @DeviceId),
(4, N'CB Áp suất 1', 'ON', '1.4', '0.8 ÷ 2.5', @DeviceId),
(5, N'CB lưu lượng 1', 'ON', '1.8', '0.8 ÷ 2.5', @DeviceId),
(6, N'CB Lưu lượng 2', 'ON', '1.6', '0.8 ÷ 2.5', @DeviceId),
(7, N'Phao Đầy 2', 'ON', '1', 'ON/OFF', @DeviceId),
(8, N'Phao Cạn 2', 'OFF', '0', 'ON/OFF', @DeviceId),
(9, N'CB lưu lượng nc thải', 'ON', '0.9', '0.8 ÷ 2.5', @DeviceId),
(10, N'CB lưu lượng trước màng', 'ON', '2.1', '0.8 ÷ 2.5', @DeviceId),
(11, N'CB lưu lượng nước RO', 'ON', '1.3', '0.8 ÷ 2.5', @DeviceId),
(12, N'Phao Đầy 3', 'OFF', '0', 'ON/OFF', @DeviceId),
(13, N'Phao Cạn 3', 'OFF', '0', 'ON/OFF', @DeviceId),
(14, N'CB lưu lượng ra kiềm', 'ON', '1.7', '0.8 ÷ 2.5', @DeviceId),
(15, N'CB Dòng bể 1', 'ON', '3.2', '1 ÷ 5', @DeviceId),
(16, N'CB Dòng bể 2', 'ON', '2.8', '1 ÷ 5', @DeviceId),
(17, N'CB Dòng bể 3', 'ON', '3.5', '1 ÷ 5', @DeviceId),
(18, N'CB Dòng bể 4', 'ON', '4.1', '1 ÷ 5', @DeviceId),
(19, N'CB dòng bể 5', 'ON', '3.9', '1 ÷ 5', @DeviceId),
(20, N'Van áp cao 1', 'ON', '1', 'ON/OFF', @DeviceId),
(21, N'Van áp cao 2', 'OFF', '0', 'ON/OFF', @DeviceId),
(22, N'Van áp cao 3', 'OFF', '0', 'ON/OFF', @DeviceId),
(23, N'Dừng Khẩn', 'OFF', '0', 'ON/OFF', @DeviceId);

-- 3. Chèn dữ liệu Output cho TH0001 (Đầy đủ 16 thông số - Trang 4)
INSERT INTO [DeviceOutput] ([Order], [Name], [Status], [Value], [SettingRange], [DeviceModelId])
VALUES 
(1, N'Bơm ngang', 'ON', '1', 'ON/OFF', @DeviceId),
(2, N'Bơm RO', 'ON', '1', 'ON/OFF', @DeviceId),
(3, N'Đèn cảnh báo', 'OFF', '0', 'ON/OFF', @DeviceId),
(4, N'Lượng nước tiêu thụ', 'ON', '450.5', N'Chỉ Số', @DeviceId),
(5, N'Điện năng tiêu thụ', 'ON', '128.2', N'Chỉ Số', @DeviceId),
(6, N'Van sả đáy UF', 'OFF', '0', 'ON/OFF', @DeviceId),
(7, N'Van sả bể điện phân', 'OFF', '0', 'ON/OFF', @DeviceId),
(8, N'Bể Điện Phân 1', 'ON', '3.2', '0 ÷ 5', @DeviceId),
(9, N'Bể Điện Phân 2', 'ON', '3.0', '0 ÷ 5', @DeviceId),
(10, N'Bể Điện Phân 3', 'ON', '3.4', '0 ÷ 5', @DeviceId),
(11, N'Bể Điện Phân 4', 'ON', '3.1', '0 ÷ 5', @DeviceId),
(12, N'Bể Điện Phân 5', 'ON', '2.9', '0 ÷ 5', @DeviceId),
(13, N'Bể Điện Phân 6', 'ON', '3.5', '0 ÷ 5', @DeviceId),
(14, N'Bể Điện Phân 7', 'ON', '3.3', '0 ÷ 5', @DeviceId),
(15, N'Bể Điện Phân 8', 'ON', '3.1', '0 ÷ 5', @DeviceId),
(16, N'Bể Điện Phân 9', 'ON', '3.2', '0 ÷ 5', @DeviceId);