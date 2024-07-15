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
