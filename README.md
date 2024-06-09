# Tubes Stima 3 - Pencocokkan Sidik Jari Menggunakan Algoritma KMP, BM, dan Regex

![Screenshot 2024-06-08 215211](https://github.com/Akmal2205/Tubes3_C-sharp-apek/assets/131881654/fdea3964-cdd1-4e25-89fe-0abc33290ef9)

## Daftar Isi
- [Informasi Umum](#informasi-umum)
- [Setup](#setup)
- [Cara Penggunaan](#cara-penggunaan)
- [Kontributor](#kontributor)

## Informasi Umum

Repository ini berisi program yang mengimplementasikan algoritma pattern matching Knuth-Morris-Pratt (KMP), Boyer-Moore (BM), dan juga regex untuk mengidentifikasi pemilik sebuah sidik jari berdasarkan nilai ASCII-8bit dari citra dan database identitas kepemilikan sidk jari.
Pencocokan citra dilakukan dengan cara mengubah citra menjadi representasi biner berdasarkan nilai gelap-terangnya sidik jari lalu mengubahnya lagi menjadi representasi ASCII-8bit yang nantinya akan dilakukan komparasi menggunakan algoritma KMP dan BM:
## - KMP:
- Pencocokan algorimta KMP memiliki fitur utama yaitu tabel prefix. Tabel ini digunakan sebagai penanda perpindahan indeks pada pattern saat terjadi mismatch dengan tujuan agar tidak melakukan backtrack terus menerus seperti pada algoritma naive (bruteforce).
- Saat terjadi mismatch maka proses pencocokan akan memindahkan indeks pattern sesuai pada tabel prefix
## - BM:
- Pencocokan algoritma BM memiliki fitur utama yaitu last occurence table, last occurence table adalah tabel yang berisi lokasi indeks terakhir dari karakter-karakter pada string yang berada pada pattern, apabila tidak ada maka akan bernilai -1.
- Pencocokan algoritma ini juga dilakukan dengan cara mundur "looking-glass technique". 
- Berbeda dengan KMP, tabel ini digunakan untuk melakukan "smart jumping" dari index penelusuran (indeks yang menjadi acuan "looking-glass technique") pada string berdasarkan 3 kasus mismatch yaitu,
- 1) Saat karakter mismatch pada string terkandung di sebelah kiri (indeks lebih kecil) pada pattern.
  2) Saat karakter mismatch pada string terkandung di sebelah kanan (indeks lebih besar) pada pattern.
  3) Saat karakter mismatch pada string tidak terkandung dalam pattern.
Regex:
- Regex digunakan untuk memperbaiki data-data yang telah corrupt di database.

## Setup
- IDE Visual Studio 2022
### Dependencies
- package .NET versi 6.0
- library Sixlabors
- MySql, perlu terlebih dahulu melakukan dump di server database mysql masing-masing dengan cara:
  1) Membuka cmd pada direktori /Main.
  2) Login dengan akun root.
  3) Buat database baru khusus untuk menampung database dari program.
    ```
    create database {nama database};
    ```
  4) Logout lalu login lagi dengan melakukan dump data pada main.sql
    ```
    mysqldump -u [username] -p [database_name] > [dump_file.sql]
    ```
  5) Ubah password root pada file program.cs di kelas Database pada direktori mainApp menjadi password root masing-masing.
## Cara Penggunaan
1. Buka program melalui Visual Studio 2022 dengan membuka "a project or a solution" dan membuka mainApp.sln pada direktori ../Main/mainApp
2. Jalankan program dengan memencet tombol debug dengan panah hijau yang bertuliskan 'mainApp'.
![Screenshot 2024-06-09 193943](https://github.com/Akmal2205/Tubes3_C-sharp-apek/assets/131881654/f4338969-0920-49f4-a93e-3c4c4729c5c0)

## Kontributor
- Mohammad Nugraha Eka Prawira - 13522001
- Bastian H. Suryapratama - 13522034
- Muhammad Syarafi Akmal - 13522076
