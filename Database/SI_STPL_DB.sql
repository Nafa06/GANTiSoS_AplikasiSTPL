DROP DATABASE SI_STPL_DB;
CREATE DATABASE SI_STPL_DB;
USE SI_STPL_DB;

CREATE TABLE tb_petugas (
    nrp VARCHAR(20) PRIMARY KEY,
    nama_petugas VARCHAR(100) NOT NULL,
    pangkat VARCHAR(50),
    password_petugas VARCHAR(255) NOT NULL
);

CREATE TABLE tb_laporan_hilang (
    no_stpl VARCHAR(50) PRIMARY KEY,
    nik_pelapor VARCHAR(16) NOT NULL,
    nama_pelapor VARCHAR(100) NOT NULL,
    jenis_barang VARCHAR(50) NOT NULL,
    ciri_khusus TEXT,
    tkp VARCHAR(255),
    waktu_kejadian DATETIME,
    status_pencarian ENUM('Dicari', 'Tersimpan') DEFAULT 'Dicari',
    nrp_petugas VARCHAR(20),
    CONSTRAINT FK_Petugas_Hilang FOREIGN KEY (nrp_petugas) 
    REFERENCES tb_petugas(nrp) ON UPDATE CASCADE ON DELETE SET NULL
);

CREATE TABLE tb_barang_temuan (
    id_temuan VARCHAR(50) PRIMARY KEY,
    nik_penemu varchar(16),
    nama_penemu VARCHAR(100),
    jenis_barang VARCHAR(50) NOT NULL,
    ciri_ciri TEXT,
    lokasi_ditemukan VARCHAR(255),
    waktu_ditemukan DATETIME,
    status_gudang ENUM('Tersimpan', 'Dikembalikan') DEFAULT 'Tersimpan',
    nrp_petugas VARCHAR(20),
    CONSTRAINT FK_Petugas_Temuan FOREIGN KEY (nrp_petugas) 
    REFERENCES tb_petugas(nrp) ON UPDATE CASCADE ON DELETE SET NULL
);

INSERT INTO tb_petugas (nrp, nama_petugas, pangkat, password_petugas)
VALUES 
('20240140200', 'Naufal', 'Bripda', 'admin123'),
('20240140201', 'Name', 'Jabatan', '123'),
('20240140202', 'Dummy', 'Dummy', 'wasd');

INSERT INTO tb_laporan_hilang 
(no_stpl, nik_pelapor, nama_pelapor, waktu_kejadian, nrp_petugas, jenis_barang, ciri_khusus, tkp, status_pencarian) 
VALUES
('L-001', '3404011203990001', 'Budi Santoso', '2026-04-10 08:30:00', '20240140200', 'Dompet Kulit', 'Warna coklat, isi KTP dan SIM C', 'Kantin Fakultas Teknik', 'Dicari'),
('L-002', '3404021508980002', 'Siti Aminah', '2026-04-11 10:15:00', '20240140200', 'HP iPhone 13', 'Warna Pink, casing bening, layar retak dikit', 'Parkiran Motor Depan', 'Tersimpan'),
('L-003', '3404031101970003', 'Andi Permana', '2026-04-12 13:00:00', '20240140200', 'Laptop Asus ROG', 'Ada stiker Gundam di cover depan', 'Perpustakaan Lantai 2', 'Dicari'),
('L-004', '3404042211990004', 'Rina Kartika', '2026-04-13 09:45:00', '20240140200', 'Kunci Motor Vario', 'Gantungan kunci boneka beruang biru', 'Jalan Raya Kampus', 'Dicari'),
('L-005', '3404050505950005', 'Dimas Anggara', '2026-04-14 16:20:00', '20240140200', 'Tas Ransel Eiger', 'Warna hitam, resleting depan rusak', 'Gedung Olahraga', 'Tersimpan'),
('L-006', '3404061802960006', 'Putri Ayu', '2026-04-15 11:10:00', '20240140200', 'Kacamata Minus', 'Frame besi bulat warna gold', 'Ruang Kelas 302', 'Dicari'),
('L-007', '3404070909980007', 'Gilang Ramadhan', '2026-04-15 14:00:00', '20240140200', 'STNK Motor Beat', 'Atas nama Gilang, plat AB 1234 XY', 'Sekitar Gerbang Utama', 'Dicari'),
('L-008', '3404082512990008', 'Nadia Safitri', '2026-04-16 08:05:00', '20240140200', 'Botol Minum Tupperware', 'Warna ungu ukuran 1 Liter', 'Taman Lapangan', 'Tersimpan'),
('L-009', '3404091407940009', 'Reza Rahadian', '2026-04-16 15:30:00', '20240140200', 'Helm Bogo', 'Warna cream, kaca datar hitam', 'Parkiran Belakang', 'Dicari'),
('L-010', '3404100303930010', 'Fahmi Idris', '2026-04-17 19:45:00', '20240140200', 'Jaket Bomber', 'Warna hijau army, polos', 'Masjid Kampus', 'Dicari');

INSERT INTO tb_barang_temuan 
(id_temuan, nik_penemu, nama_penemu, waktu_ditemukan, nrp_petugas, jenis_barang, ciri_ciri, lokasi_ditemukan, status_gudang) 
VALUES
('F-001', '3404112101990011', 'Tono Purnomo', '2026-04-10 09:10:00', '20240140200', 'Kunci Motor', 'Gantungan kunci boneka beruang biru', 'Jalan Raya Kampus', 'Tersimpan'),
('F-002', '3404121503980012', 'Vina Amelia', '2026-04-11 11:20:00', '20240140200', 'HP iPhone', 'Warna Pink, casing bening', 'Sekitar Halte Bis', 'Dikembalikan'),
('F-003', '3404130808970013', 'Hendra Cipta', '2026-04-12 07:30:00', '20240140200', 'Flashdisk Sandisk', 'Kapasitas 32GB, warna merah hitam', 'Lab Komputer', 'Tersimpan'),
('F-004', '3404141910990014', 'Dewi Lestari', '2026-04-13 13:15:00', '20240140200', 'Dompet Wanita', 'Motif bunga, isi KTP atas nama Rina', 'Toilet Wanita Lt 1', 'Tersimpan'),
('F-005', '3404152706950015', 'Kiki Saputra', '2026-04-14 16:45:00', '20240140200', 'Kacamata Hitam', 'Merk Rayban, frame plastik', 'Bangku Taman', 'Dikembalikan'),
('F-006', '3404161109960016', 'Maya Wulan', '2026-04-15 10:05:00', '20240140200', 'Tas Selempang', 'Warna navy, isi buku tulis', 'Ruang Dosen', 'Tersimpan'),
('F-007', '3404172204980017', 'Agus Supriatna', '2026-04-15 15:50:00', '20240140200', 'STNK Mobil', 'Atas nama Budi Santoso', 'Lantai Masjid', 'Dikembalikan'),
('F-008', '3404183011990018', 'Citra Kirana', '2026-04-16 09:10:00', '20240140200', 'Jam Tangan Eiger', 'Strap karet hitam, digital', 'Kantin Teknik', 'Tersimpan'),
('F-009', '3404191705940019', 'Ridho Illahi', '2026-04-16 14:25:00', '20240140200', 'Earphone TWS', 'Merk Baseus warna putih', 'Area Lapangan Basket', 'Tersimpan'),
('F-010', '3404200402930020', 'Bella Saphira', '2026-04-17 18:00:00', '20240140200', 'Payung Lipat', 'Warna biru dongker', 'Teras Gedung C', 'Tersimpan');

select * from tb_petugas;
select * from tb_laporan_hilang;
select * from tb_barang_temuan;

DELIMITER $$
CREATE PROCEDURE SP_InsertLaporanHilang(
    IN p_no_stpl VARCHAR(50),
    IN p_nik_pelapor CHAR(16),
    IN p_nama_pelapor VARCHAR(100),
    IN p_jenis_barang VARCHAR(100),
    IN p_waktu_kejadian DATE,
    IN p_ciri_khusus TEXT,
    IN p_tkp VARCHAR(255),
    IN p_nrp_petugas CHAR(20)
)
BEGIN
    IF p_no_stpl = '' OR p_nik_pelapor = '' OR p_nama_pelapor = '' THEN
        SIGNAL SQLSTATE '45000' 
        SET MESSAGE_TEXT = 'Gagal: Kolom utama (No STPL, NIK, Nama) tidak boleh kosong!';
    ELSE
        INSERT INTO tb_laporan_hilang (no_stpl, nik_pelapor, nama_pelapor, jenis_barang, waktu_kejadian, ciri_khusus, tkp, nrp_petugas)
        VALUES (p_no_stpl, p_nik_pelapor, p_nama_pelapor, p_jenis_barang, p_waktu_kejadian, p_ciri_khusus, p_tkp, p_nrp_petugas);
    END IF;
END $$
DELIMITER ;

CREATE OR REPLACE VIEW vw_laporan_hilang_lengkap AS
SELECT 
    l.no_stpl AS 'No STPL',
    l.nik_pelapor AS 'NIK Pelapor',
    l.nama_pelapor AS 'Nama Pelapor',
    l.jenis_barang AS 'Jenis Barang',
    l.ciri_khusus AS 'Ciri Khusus',
    l.tkp AS 'TKP',
    l.waktu_kejadian AS 'Waktu Kejadian',
    l.status_pencarian AS 'Status',
    l.nrp_petugas AS 'NRP Petugas',
    p.nama_petugas AS 'Nama Petugas'
FROM tb_laporan_hilang l
LEFT JOIN tb_petugas p ON l.nrp_petugas = p.nrp;

CREATE OR REPLACE VIEW vw_laporan_hilang_lengkap AS
SELECT 
    l.no_stpl, l.nik_pelapor, l.nama_pelapor, l.jenis_barang, 
    l.ciri_khusus, l.tkp, l.waktu_kejadian, l.status_pencarian, 
    l.nrp_petugas, p.nama_petugas
FROM tb_laporan_hilang l
LEFT JOIN tb_petugas p ON l.nrp_petugas = p.nrp;

CREATE OR REPLACE VIEW vw_barang_temuan_lengkap AS
SELECT 
    b.id_temuan, b.nik_penemu, b.nama_penemu, b.jenis_barang, 
    b.ciri_ciri, b.lokasi_ditemukan, b.waktu_ditemukan, b.status_gudang, 
    b.nrp_petugas, p.nama_petugas
FROM tb_barang_temuan b
LEFT JOIN tb_petugas p ON b.nrp_petugas = p.nrp;

DELIMITER $$
CREATE PROCEDURE SP_UpdateStatusLaporan(
    IN p_no_stpl VARCHAR(50),
    IN p_jenis_barang VARCHAR(50),
    IN p_ciri_khusus TEXT,
    IN p_tkp VARCHAR(255),
    IN p_status ENUM('Dicari', 'Tersimpan')
)
BEGIN
    -- Validasi tingkat lanjut agar memenuhi syarat penilaian dosen
    IF NOT EXISTS (SELECT 1 FROM tb_laporan_hilang WHERE no_stpl = p_no_stpl) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Gagal mengubah: Nomor STPL tidak ditemukan di database!';
    ELSEIF p_jenis_barang = '' OR p_tkp = '' THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Gagal mengubah: Kolom Jenis Barang dan TKP tidak boleh dikosongkan!';
    ELSE
        UPDATE tb_laporan_hilang 
        SET jenis_barang = p_jenis_barang,
            ciri_khusus = p_ciri_khusus,
            tkp = p_tkp,
            status_pencarian = p_status 
        WHERE no_stpl = p_no_stpl;
    END IF;
END $$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE SP_DeleteLaporanHilang(
    IN p_no_stpl VARCHAR(50)
)
BEGIN
    -- Validasi: Pastikan data memang ada sebelum dihapus
    IF NOT EXISTS (SELECT 1 FROM tb_laporan_hilang WHERE no_stpl = p_no_stpl) THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Gagal menghapus: Nomor STPL tidak ditemukan!';
    ELSE
        DELETE FROM tb_laporan_hilang WHERE no_stpl = p_no_stpl;
    END IF;
END $$
DELIMITER ;

-- Tabel Log Laporan
CREATE TABLE IF NOT EXISTS tb_log_laporan (
    id_log INT AUTO_INCREMENT PRIMARY KEY,
    no_stpl VARCHAR(50) NOT NULL,
    aksi VARCHAR(20) NOT NULL,
    waktu DATETIME DEFAULT CURRENT_TIMESTAMP,
    detail TEXT
);

-- Trigger AFTER INSERT pada tb_laporan_hilang
DROP TRIGGER IF EXISTS trg_laporan_insert;
DELIMITER $$
CREATE TRIGGER trg_laporan_insert
AFTER INSERT ON tb_laporan_hilang
FOR EACH ROW
BEGIN
    INSERT INTO tb_log_laporan (no_stpl, aksi, detail)
    VALUES (NEW.no_stpl, 'INSERT', CONCAT('Laporan baru ditambahkan oleh Petugas NRP: ', NEW.nrp_petugas, ', Pelapor: ', NEW.nama_pelapor, ', Barang: ', NEW.jenis_barang));
END $$
DELIMITER ;

-- Trigger AFTER UPDATE pada tb_laporan_hilang
DROP TRIGGER IF EXISTS trg_laporan_update;
DELIMITER $$
CREATE TRIGGER trg_laporan_update
AFTER UPDATE ON tb_laporan_hilang
FOR EACH ROW
BEGIN
    INSERT INTO tb_log_laporan (no_stpl, aksi, detail)
    VALUES (NEW.no_stpl, 'UPDATE', CONCAT('Detail diubah. Status: ', OLD.status_pencarian, ' -> ', NEW.status_pencarian, ', Barang: ', OLD.jenis_barang, ' -> ', NEW.jenis_barang));
END $$
DELIMITER ;

-- Trigger AFTER DELETE pada tb_laporan_hilang
DROP TRIGGER IF EXISTS trg_laporan_delete;
DELIMITER $$
CREATE TRIGGER trg_laporan_delete
AFTER DELETE ON tb_laporan_hilang
FOR EACH ROW
BEGIN
    INSERT INTO tb_log_laporan (no_stpl, aksi, detail)
    VALUES (OLD.no_stpl, 'DELETE', CONCAT('Laporan dihapus dari sistem. Pelapor lama: ', OLD.nama_pelapor, ', Barang: ', OLD.jenis_barang));
END $$
DELIMITER ;

-- Tabel Log Sesi Petugas
CREATE TABLE IF NOT EXISTS tb_log_sesi (
    id_sesi INT AUTO_INCREMENT PRIMARY KEY,
    nrp VARCHAR(20) NOT NULL,
    waktu DATETIME DEFAULT CURRENT_TIMESTAMP,
    aktivitas VARCHAR(50) NOT NULL
);

-- SP untuk Log Sesi Petugas
DROP PROCEDURE IF EXISTS sp_LogSesiPetugas;
DELIMITER $$
CREATE PROCEDURE sp_LogSesiPetugas(
    IN p_nrp VARCHAR(20),
    IN p_aktivitas VARCHAR(50)
)
BEGIN
    INSERT INTO tb_log_sesi (nrp, aktivitas) VALUES (p_nrp, p_aktivitas);
END $$
DELIMITER ;

-- Tabel Log Error Umum
CREATE TABLE IF NOT EXISTS tb_log_pesan (
    id_log_pesan INT AUTO_INCREMENT PRIMARY KEY,
    waktu DATETIME DEFAULT CURRENT_TIMESTAMP,
    pesan TEXT NOT NULL
);

-- SP untuk Log Error
DROP PROCEDURE IF EXISTS sp_LogMessage;
DELIMITER $$
CREATE PROCEDURE sp_LogMessage(
    IN psn TEXT
)
BEGIN
    INSERT INTO tb_log_pesan (pesan) VALUES (psn);
END $$
DELIMITER ;