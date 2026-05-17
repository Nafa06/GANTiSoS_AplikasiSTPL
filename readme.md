# Aplikasi Pencatatan STPL - STPLAPP

Sistem pencatatan Surat Tanda Penerimaan Laporan (STPL) untuk Laporan Kehilangan dan Barang Temuan yang dibuat dengan:

- C#
- Windows Forms
- MySQL
- ADO.NET (MySql.Data)

## Fitur Utama

- Tes koneksi database
- Autentikasi Login
- CRUD
- CREATE: Tambah data (Laporan Hilang & Barang Temuan)
- UPDATE: Ubah detail data & status barang
- DELETE: Hapus data laporan
- READ: Menampilkan data pada DataGridView
- Fitur pencarian

## Tampilan Aplikasi

### Tes Koneksi
![Connection](Assets/ConnectionTest.png)

### Halaman Login
![Login](Assets/LoginFill.png)

### Menu Utama
![Menu](Assets/Menu.png)

### Halaman Input Data
![Input](Assets/InputLostBlank.png)
![Input](Assets/InputFoundBlank.png)

### Bukti Simpan Data Sukses
![Insert](Assets/InputLostFill.png)

### Tampilan Data (Gudang)
![Display](Assets/SearchBlank.png)

### Hasil Pencarian Data
![Search](Assets/SearchFilter.png)

### Bukti Ubah Data Sukses
![Update](Assets/SearchUpdateNoticeSuccess.png)

### Bukti Hapus Data Sukses
![Delete](Assets/SearchDeleteSuccessNotice.png)

## Lampiran

![Login](Assets/LoginBlankNotice.png)
![Login](Assets/LoginFillWrong.png)
![Login](Assets/LoginSuccessNotice.png)
![Input](Assets/InputLostFill.png)
![Search](Assets/SearchDeleteConfirmation.png)
![Menu](Assets/MenuLogoutNotice.png)

## Skenario Pengujian SQL Injection

**UNION-Based SQL Injection** pada fitur pencarian di **Form Search**.

### 1. Kode Program yang Rentan (Vulnerable Code)
Pada tombol simulasi injeksi di `FormSearch.cs`, input string dari pengguna langsung digabungkan ke dalam perintah query SQL String Concatenation tanpa melalui proses sanitasi data ataupun Parameterized Query:
```csharp
string query = "SELECT * FROM vw_laporan_hilang_lengkap WHERE no_stpl = '" + payloadSQLi + "' OR nama_pelapor = '" + payloadSQLi + "'";