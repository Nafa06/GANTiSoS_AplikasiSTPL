USE SI_STPL_DB;

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