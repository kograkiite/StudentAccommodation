create database StudentManagement

use StudentManagement

CREATE TABLE Phong (
    SoPhong VARCHAR(10) PRIMARY KEY,
    SucChua INT NOT NULL,
    GiaThue DECIMAL(18, 2) NOT NULL,
    SoLuongSinhVienHienTai INT NOT NULL DEFAULT 0,
    TrangThaiPhong BIT NOT NULL DEFAULT 1 -- 1: Còn phòng, 0: Hết phòng
);

CREATE TABLE SinhVien (
    id VARCHAR(50) NOT NULL PRIMARY KEY,
    fullname NVARCHAR(255) NOT NULL,
    phoneNumber VARCHAR(20),
    sex NVARCHAR(3),
    dateOfBirth DATE,
    SoPhong VARCHAR(10) NOT NULL,
    FOREIGN KEY (SoPhong) REFERENCES Phong(SoPhong)
);

CREATE TABLE HopDong (
    MaHopDong INT PRIMARY KEY IDENTITY(1,1),
    MaSinhVien VARCHAR(50) NOT NULL,
    TenSinhVien NVARCHAR(255) NOT NULL,
    SoDienThoai VARCHAR(20),
    SoPhong VARCHAR(10) NOT NULL,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    TrangThai BIT NOT NULL DEFAULT 1, -- Sử dụng kiểu BIT để lưu trạng thái, mặc định là 1 (true)
    FOREIGN KEY (MaSinhVien) REFERENCES SinhVien(id),
    FOREIGN KEY (SoPhong) REFERENCES Phong(SoPhong)
);

CREATE TABLE ThuTien (
    MaThuTien INT PRIMARY KEY IDENTITY(1,1),
    MaSinhVien VARCHAR(50) NOT NULL,
    TenSinhVien NVARCHAR(255) NOT NULL,
    SoDienThoai VARCHAR(20),
    SoPhong VARCHAR(10) NOT NULL,
    GiaThue DECIMAL(18, 2) NOT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    MaHopDong INT,
    FOREIGN KEY (MaSinhVien) REFERENCES SinhVien(id),
    FOREIGN KEY (SoPhong) REFERENCES Phong(SoPhong),
    FOREIGN KEY (MaHopDong) REFERENCES HopDong(MaHopDong)
);

-- Table: Users (for login and role management)
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('student', 'admin')),
    SinhVienID VARCHAR(50),
    FOREIGN KEY (SinhVienID) REFERENCES SinhVien(id)
);
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('student', 'admin')),
    SinhVienID VARCHAR(50) UNIQUE,
    FOREIGN KEY (SinhVienID) REFERENCES SinhVien(id)
);

















-- Bảng: Phong
CREATE TABLE Phong (
    SoPhong VARCHAR(10) PRIMARY KEY,
    SucChua INT NOT NULL,
    GiaThue DECIMAL(18, 2) NOT NULL,
    SoLuongSinhVienHienTai INT NOT NULL DEFAULT 0,
    TrangThaiPhong BIT NOT NULL DEFAULT 1 -- 1: Còn phòng, 0: Hết phòng
);

-- Bảng: SinhVien
CREATE TABLE SinhVien (
    id VARCHAR(50) NOT NULL PRIMARY KEY,
    fullname NVARCHAR(255) NOT NULL,
    phoneNumber VARCHAR(20),
    sex NVARCHAR(3),
    dateOfBirth DATE,
    SoPhong VARCHAR(10) NULL,
    FOREIGN KEY (SoPhong) REFERENCES Phong(SoPhong)
);

-- Bảng: HopDong
CREATE TABLE HopDong (
    MaHopDong INT PRIMARY KEY IDENTITY(1,1),
    MaSinhVien VARCHAR(50) NOT NULL,
    TenSinhVien NVARCHAR(255) NOT NULL,
    SoDienThoai VARCHAR(20),
    SoPhong VARCHAR(10) NOT NULL,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    TrangThai BIT NOT NULL DEFAULT 1, -- 1: Hoạt động, 0: Ngừng hoạt động
    FOREIGN KEY (MaSinhVien) REFERENCES SinhVien(id),
    FOREIGN KEY (SoPhong) REFERENCES Phong(SoPhong)
);

-- Bảng: ThuTien
CREATE TABLE ThuTien (
    MaThuTien INT PRIMARY KEY IDENTITY(1,1),
    MaSinhVien VARCHAR(50) NOT NULL,
    TenSinhVien NVARCHAR(255) NOT NULL,
    SoDienThoai VARCHAR(20),
    SoPhong VARCHAR(10) NOT NULL,
    GiaThue DECIMAL(18, 2) NOT NULL,
    TrangThai BIT NOT NULL DEFAULT 1,
    MaHopDong INT,
    FOREIGN KEY (MaSinhVien) REFERENCES SinhVien(id),
    FOREIGN KEY (SoPhong) REFERENCES Phong(SoPhong),
    FOREIGN KEY (MaHopDong) REFERENCES HopDong(MaHopDong)
);

-- Bảng: Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('student', 'admin')),
    SinhVienID VARCHAR(50) NOT NULL,
    fullname NVARCHAR(255) NOT NULL,
    phoneNumber VARCHAR(20),
    sex NVARCHAR(3),
    dateOfBirth DATE,
    FOREIGN KEY (SinhVienID) REFERENCES SinhVien(id)
);

-- Chèn dữ liệu vào bảng Phong
INSERT INTO Phong (SoPhong, SucChua, GiaThue, SoLuongSinhVienHienTai, TrangThaiPhong)
VALUES 
('101', 2, 1500.00, 0, 1),
('102', 3, 2000.00, 1, 1),
('201', 4, 2500.00, 0, 1);

-- Chèn dữ liệu vào bảng SinhVien
INSERT INTO SinhVien (id, fullname, phoneNumber, sex, dateOfBirth, SoPhong)
VALUES 
('SV001', 'Nguyen Van A', '0123456789', 'Mal', '2000-01-01', '101'),
('SV002', 'Tran Thi B', '0987654321', 'Fel', '2001-02-02', '102'),
('SV003', 'Le Van C', '0912345678', 'Mal', '2000-03-03', NULL); -- Chưa có phòng

-- Chèn dữ liệu vào bảng HopDong
INSERT INTO HopDong (MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, NgayBatDau, NgayKetThuc, TrangThai)
VALUES 
('SV001', 'Nguyen Van A', '0123456789', '101', '2023-08-01', '2024-07-31', 1),
('SV002', 'Tran Thi B', '0987654321', '102', '2023-08-15', '2024-07-31', 1);

-- Chèn dữ liệu vào bảng ThuTien
INSERT INTO ThuTien (MaSinhVien, TenSinhVien, SoDienThoai, SoPhong, GiaThue, TrangThai, MaHopDong)
VALUES 
('SV001', 'Nguyen Van A', '0123456789', '101', 1500.00, 1, 1),
('SV002', 'Tran Thi B', '0987654321', '102', 2000.00, 1, 2);

-- Chèn dữ liệu vào bảng Users
INSERT INTO Users (Username, PasswordHash, Role, SinhVienID, fullname, phoneNumber, sex, dateOfBirth)
VALUES 
('user1', 'hashed_password_1', 'student', 'SV001', 'Nguyen Van A', '0123456789', 'Mal', '2000-01-01'),
('admin', 'hashed_password_2', 'admin', 'SV002', 'Tran Thi B', '0987654321', 'Fel', '2001-02-02');