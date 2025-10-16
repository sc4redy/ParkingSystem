# Parking Lot System

Sistem parkir berbasis konsol ini memungkinkan pengguna untuk mengelola parkir kendaraan di tempat parkir dengan sejumlah slot terbatas. Setiap slot hanya dapat digunakan oleh satu kendaraan (mobil atau motor) dan perhitungan biaya parkir dihitung per jam.

Proyek ini adalah bagian dari tugas **.NET Developer** untuk **PT NTT Indonesia Technology**.

## Fitur Utama

- **Membuat Tempat Parkir**: Membuat tempat parkir dengan jumlah slot tertentu.
- **Parkir Kendaraan**: Menempatkan kendaraan ke slot yang tersedia dan mencatat nomor registrasi serta jenis kendaraan.
- **Keluar Kendaraan**: Mengeluarkan kendaraan dari slot yang sudah terisi dan menghitung biaya parkir berdasarkan durasi parkir.
- **Status Tempat Parkir**: Menampilkan status tempat parkir terkini termasuk kendaraan yang terparkir, nomor slot, dan informasi kendaraan.
- **Laporan Kendaraan**: Laporan jumlah kendaraan berdasarkan jenis, nomor registrasi ganjil/genap, dan warna kendaraan.
- **Pencarian Kendaraan**:
  - Cari kendaraan berdasarkan nomor registrasi.
  - Cari slot kendaraan berdasarkan warna.
  - Cari kendaraan berdasarkan nomor registrasi ganjil/genap.
 
## Command Yang Bisa Digunakan
```
--------------------------------------------------------------------------------
cpl <number>                     (create_parking_lot)  - Buat parking lot dengan <n> slot.
pk <registration> <color> <type> (park)             - Parkirkan kendaraan.
lv <slot_number>                 (leave)            - Keluarkan kendaraan dari slot.
st                               (status)           - Tampilkan status saat ini.
tov                              (type_of_vehicles) - Tampilkan jumlah kendaraan per tipe.
rnvc <color>                     (registration_numbers_for_vehicles_with_colour)  - Nomor registrasi berdasarkan warna.
snvc <color>                     (slot_numbers_for_vehicles_with_colour)  - Nomor slot kendaraan berdasarkan warna.
snr <registration>               (slot_number_for_registration_number)    - Nomor slot untuk registrasi.
rno                              (registration_numbers_for_vehicles_with_odd_plate)  - Nomor registrasi dengan digit akhir ganjil.
rne                              (registration_numbers_for_vehicles_with_even_plate) - Nomor registrasi dengan digit akhir genap.
h                                (help)             - Tampilkan help ini.
e                                (exit)             - Keluar dari aplikasi.
--------------------------------------------------------------------------------

```
## Contoh Penggunaan

### 1. Membuat Tempat Parkir
Membuat tempat parkir dengan 10 slot.
```bash
cpl 10
```


### 2. Parkir Kendaraan
Kendaraan dengan nomor registrasi "B-1234-XXX" warna "Hitam" dan jenis "Mobil" akan diparkir di slot pertama.
```bash
pk B-1234-XXX Hitam Mobil
```
Kendaraan dengan nomor registrasi "B-9999-XYZ" warna "Putih" dan jenis "Motor" akan diparkir di slot kedua.
```bash
pk B-9999-XYZ Putih Motor
```
Kendaraan dengan nomor registrasi "B-7777-XSS" warna "Putih" dan jenis "Mobil" akan diparkir di slot ketiga.
```bash
pk B-7777-XSS Putih Mobil
```  


### 3. Mengeluarkan Kendaraan
```bash
lv 2
```  


### 4. Melihat Status Tempat Parkir
```bash
st
```
Menampilkan status parkir saat ini:
```bash
Slot No. | Registration No | Colour    | Type  | CheckIn
------------------------------------------------------------------------
      1 | B-1234-XXX      | Hitam      | Mobil | 2025-10-16 22:57:58
      3 | B-7777-XSS      | Putih      | Mobil | 2025-10-16 23:03:42
```

### 5. Mencari Kendaraan Berdasarkan Jenis
Menampilkan jumlah kendaraan jenis "Mobil" dan "Motor".
```bash
tov
```

### 6. Mencari Nomor Registrasi Berdasarkan Warna
Menampilkan nomor registrasi kendaraan berwarna "Putih".
```bash
rnvc <color>
```


### 7. Mencari Slot Berdasarkan Warna
Menampilkan nomor slot kendaraan berwarna "Putih".
```bash
snvc <color>
```


### 8. Mencari Slot Berdasarkan Nomor Registrasi
Menampilkan nomor slot untuk kendaraan dengan nomor registrasi "B-3141-ZZZ".
```bash
snr <registration>
```


### 9. Keluar dari Aplikasi
```bash
exit
```

==========================================================================================
### Persyaratan
.NET 5 atau versi lebih baru.

### Cara Menjalankan
1. Jika sudah menginstal .NET SDK pada Visual Studio Code, maka buka terminal dan jalankan:
```bash
dotnet run
```
2. Ketik help untuk menampilkan semua command bantuan
```bash
help
```
