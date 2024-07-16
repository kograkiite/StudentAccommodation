using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement
{
    public class RoomRequest
    {
        public int RequestID { get; set; }
        public string MaSinhVien { get; set; }
        public string TenSinhVien { get; set; }
        public string SoDienThoai { get; set; }
        public string SoPhong { get; set; }
        public DateTime NgayYeuCau { get; set; }
        public string TrangThaiYeuCau { get; set; }
    }
}


