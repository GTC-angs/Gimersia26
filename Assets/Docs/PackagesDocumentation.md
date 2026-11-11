# 📦 Dokumentasi Package Store & Library Unity
## Proyek: *No, Please Clean the Room!* (#GIMERSIA2026)

Dokumen ini berisi daftar lengkap package pihak ketiga (Unity Asset Store / UPM) serta package esensial yang terpasang di dalam proyek, fungsi spesifiknya untuk game ini, dan panduan dasar penggunaannya beserta contoh kode praktis.

---

## 📑 Daftar Isi
1. [Linework Lite (Free Outline)](#1-linework-lite-free-outline)
2. [DOTween (Demigiant)](#2-dotween-demigiant)
3. [NeatoTags (CharlieMadeAThing)](#3-neatotags-charliemadeathing)
4. [Sortify (Hierarchy & Inspector Tool)](#4-sortify-hierarchy--inspector-tool)
5. [PlayerPrefs Editor (Cyber Chaos Games / bgTools)](#5-playerprefs-editor-cyber-chaos-games--bgtools)
6. [Icon Studio](#6-icon-studio)
7. [nTools UV Inspector](#7-ntools-uv-inspector)
8. [Package Resmi Unity yang Digunakan](#8-package-resmi-unity-yang-digunakan)
   * [Unity Input System](#81-unity-input-system)
   * [AI Navigation (NavMesh)](#82-ai-navigation-navmesh)
   * [Universal Render Pipeline (URP)](#83-universal-render-pipeline-urp)

---

## 1. Linework Lite (Free Outline)

* **Kategori:** Shader / Rendering / VFX
* **Lokasi di Proyek:** `Packages/dev.ameye.linework-lite`
* **Pembuat:** Alexander Ameye
* **Peran dalam Game:** Memberikan efek siluet / outline menyala pada objek interaktif (furnitur yang bisa diangkat, alat pel/sapu, sampah, dan drop-zone target penataan).

### 🚀 Cara Setup di Editor
1. **Cek Kompatibilitas:** Buka menu `Window > Linework Lite > Compatibility`. Pastikan semua indikator berwarna hijau.
2. **Pasang di URP Renderer Data:**
   * Buka asset Universal Renderer Data di proyek (di folder `Assets/Settings/` atau URP Asset).
   * Pada Inspector, klik **Add Renderer Feature** ➔ pilih **Free Outline**.
3. **Buat Outline Settings:**
   * Klik kanan di Project Window ➔ `Create > Linework Lite > Free Outline Settings`.
   * Drag asset settings tersebut ke slot setting di Renderer Feature *Free Outline*.
4. **Atur Rendering Layer (Target Objek):**
   * Di dalam Outline Settings, tentukan layer rendering mana yang akan menerima garis luar (misal: layer khusus `Interactable` atau `HeavyObject`).
   * Pada Mesh Renderer objek di scene, aktifkan opsi **Rendering Layer Mask** sesuai layer outline tersebut.

---

## 2. DOTween (Demigiant)

* **Kategori:** Animation Engine / Game Feel & "Juice"
* **Lokasi di Proyek:** `Assets/Plugins/Demigiant/DOTween`
* **Pembuat:** Daniele Giardini (Demigiant)
* **Peran dalam Game:** Sangat vital untuk memoles *Game Feel & Juice* (tim Abdi & Fito), seperti efek membal (*squash & stretch*) saat barang diangkat/dilempar, animasi pop-up icon UI, teks skor melompat, timer berkedip merah saat waktu kritis, dan pergerakan kamera halus.

### 🚀 Cara Setup Pertama Kali
1. Jika baru diimpor atau muncul dialog, buka menu: `Tools > Demigiant > DOTween Utility Panel`.
2. Klik tombol **"Setup DOTween..."** untuk mengompilasi modul Unity.

### 💻 Contoh Penggunaan Dasar (C#)

Tambahkan namespace di script Anda:
```csharp
using DG.Tweening;
using UnityEngine;
```

#### A. Efek Membal Saat Objek Diambil (Punch Scale)
```csharp
public void OnGrabItem(Transform itemTransform)
{
    // Membal sebentar lalu kembali ke ukuran semula
    itemTransform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), duration: 0.25f, vibrato: 5, elasticity: 0.5f);
}
```

#### B. Menggeser Objek / Perabot ke Posisi Target (DOMove)
```csharp
// Pindah ke posisi target dengan easing elastis
targetTransform.DOMove(new Vector3(2f, 0f, 5f), 0.5f)
               .SetEase(Ease.OutBack)
               .OnComplete(() => Debug.Log("Furnitur terpasang!"));
```

#### C. Efek Snap ke Drop Zone (Sequence)
```csharp
Sequence snapSeq = DOTween.Sequence();
snapSeq.Append(transform.DOMove(dropZone.position, 0.2f).SetEase(Ease.OutQuad))
       .Join(transform.DORotate(dropZone.rotation.eulerAngles, 0.2f))
       .Append(transform.DOPunchScale(Vector3.up * 0.2f, 0.15f));
```

#### D. Berkedip Merah pada UI Timer saat Kritis (DOColor / Fade)
```csharp
using TMPro;

[SerializeField] private TextMeshProUGUI timerText;

public void StartTimeWarning()
{
    timerText.DOColor(Color.red, 0.4f)
             .SetLoops(-1, LoopType.Yoyo)
             .SetEase(Ease.InOutSine);
}
```

> 💡 **Best Practice:** Selalu panggil `.Kill()` atau gunakan `.SetLink(gameObject)` jika tween dijalankan pada GameObject yang bisa dihancurkan/dinonaktifkan sewaktu-waktu:
> ```csharp
> transform.DOMove(targetPos, 1f).SetLink(gameObject);
> ```

---

## 3. NeatoTags (CharlieMadeAThing)

* **Kategori:** Gameplay Architecture / Tagging System
* **Lokasi di Proyek:** `Assets/CharlieMadeAThing/NeatoTags`
* **Pembuat:** CharlieMadeAThing
* **Peran dalam Game:** Menggantikan sistem Single-Tag bawaan Unity yang kaku. Dengan NeatoTags, satu GameObject bisa memiliki banyak tag berbasis ScriptableObject (misal: satu objek kulkas bisa punya tag `[Heavy]`, `[Movable]`, `[Kitchen]`, `[Dirty]`).

### 🚀 Cara Membuat Tag Baru
1. Di Project Window, klik kanan ➔ `Create > NeatoTags > Tag` (atau sudah tersedia folder di `Assets/Resources/Tags/`).
2. Beri nama tag sesuai kategori gameplay (contoh: `HeavyObject`, `Cleanable`, `Trash`, `Furniture`, `P1Only`, `P2Only`).

### 🚀 Menempelkan Tag ke Objek di Scene
1. Pasang komponen **`Tagger`** ke GameObject yang diinginkan.
2. Drag asset tag yang sudah dibuat ke dalam daftar tag di komponen `Tagger`.

### 💻 Contoh Penggunaan Dasar (C#)

```csharp
using UnityEngine;
using CharlieMadeAThing.NeatoTags.Core;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private NeatoTag heavyTag;
    [SerializeField] private NeatoTag cleanableTag;

    private void OnTriggerEnter(Collider other)
    {
        // Pastikan objek memiliki komponen Tagger
        if (!other.gameObject.HasTagger()) return;

        // Cek satu tag spesifik
        if (other.gameObject.HasTag(heavyTag))
        {
            Debug.Log("Objek ini berat! Butuh P1 (Suami) atau diangkat berdua.");
        }

        // Cek tag dengan filter fluent
        if (other.gameObject.FilterTags().WithTag(cleanableTag).WithoutTag(heavyTag).IsMatch())
        {
            Debug.Log("Bisa dibersihkan langsung oleh P2!");
        }
    }

    // Mencari semua perabotan berat yang belum dibereskan di scene
    public void FindAllHeavyObjects()
    {
        var heavyItems = Tagger.FilterGameObjects().WithTag(heavyTag).GetMatches();
        Debug.Log($"Ditemukan {heavyItems.Count} barang berat di ruangan.");
    }
}
```

---

## 4. Sortify (Hierarchy & Inspector Tool)

* **Kategori:** Editor Utility / Workflow & Productivity
* **Lokasi di Proyek:** `Assets/Sortify`
* **Peran dalam Game:** 
  1. Merapikan jendela Hierarchy scene agar terstruktur rapi berdasarkan section (misal: `--- ENVIRONMENT ---`, `--- LIGHTS ---`, `--- PLAYERS ---`).
  2. Menyediakan atribut Inspector yang mempercepat testing programmer tanpa harus membuat custom editor manual.

### 🚀 Fitur Hierarchy Organizer
* Menampilkan ikon komponen langsung di sebelah nama GameObject di Hierarchy.
* Memungkinkan pembuatan divider / separator visual untuk mengelompokkan objek level.
* Quick Add Component dan GameObject note/catatan kecil langsung dari hierarchy.
* Pengaturan warna / styling hierarchy dapat diakses via menu `Tools > Sortify` atau icon gear di pojok hierarchy.

### 💻 Fitur Inspector Attributes (Untuk Programmer)

Sortify menyediakan atribut praktis di file script C# (Namespace: `Sortify`):

```csharp
using UnityEngine;
#if SORTIFY_ATTRIBUTES
using Sortify;
#endif

public class LevelManagerTest : MonoBehaviour
{
    [BetterHeader("Pengaturan Gameplay", 16, TextAnchor.MiddleLeft)]
    [Unit("detik")]
    public float levelDuration = 180f;

    [Unit("kg")]
    public float itemWeight = 15f;

    [ReadOnly]
    public int currentCleanPercentage = 0;

    [MinMaxSlider(0, 100)]
    public Vector2 spawnRange = new Vector2(10, 50);

    public bool isTimeAttack = true;

    // Hanya tampil di inspector jika isTimeAttack bernilai true
    [ShowIf("isTimeAttack")]
    public float bonusTime = 30f;

    // Menampilkan tombol langsung di Inspector Unity untuk trigger fungsi!
    [Button("Instan Bersihkan Semua Noda")]
    public void DebugCleanAll()
    {
        Debug.Log("Membersihkan semua noda seketika untuk testing!");
    }

    [Button("Trigger Waktu Habis")]
    public void DebugTimeout()
    {
        Debug.Log("Simulasi game over!");
    }
}
```

---

## 5. PlayerPrefs Editor (Cyber Chaos Games / bgTools)

* **Kategori:** Debugging / Save Data Tool
* **Lokasi di Proyek:** `Assets/PlayerPrefsEditor`
* **Pembuat:** Dysman / Cyber Chaos Games
* **Peran dalam Game:** Melihat, mengedit, menambah, dan menghapus data yang tersimpan di `PlayerPrefs` (seperti High Score, status bintang level 1/2/3, pengaturan audio volume, dan status unlockable) secara visual tanpa harus membongkar registry Windows.

### 🚀 Cara Menggunakan
1. Buka melalui menu atas Unity:
   `Tools > CCG-Tools > PlayerPrefs Editor`
2. Jendela editor dapat di-dock di samping Console atau Inspector.
3. Fitur Utama:
   * **View All Keys:** Menampilkan seluruh key yang tersimpan saat game dijalankan.
   * **Edit Value:** Mengubah value integer, float, atau string langsung dari tabel.
   * **Add Key:** Menambahkan key baru untuk simulasi data save.
   * **Delete / Delete All:** Menghapus data spesifik atau mereset seluruh save data untuk memulai playtest dari kondisi awal (*clean state*).

---

## 6. Icon Studio

* **Kategori:** 2D Asset Generator / UI Production
* **Lokasi di Proyek:** `Assets/IconStudio`
* **Peran dalam Game:** Membuat icon 2D berlatar belakang transparan secara instan dari model 3D perabot (sapu, pel, ember, kardus, cangkir, sofa). Sangat mempermudah tim UI (Irfan & Nikkow) membuat icon inventori/task list tanpa perlu rendering eksternal di Blender/Photoshop.

### 🚀 Cara Cepat Membuat Ikon (Alur 1 Menit)
1. Buka scene generator di: `Assets/IconStudio/Scenes/IconStudio.unity`.
2. Tarik (*drag*) prefab atau model 3D barang yang ingin difoto ke bawah GameObject **`ItemSpawn`** di Hierarchy.
3. Sesuaikan posisi, rotasi, atau sudut kamera agar model tampak jelas di jendela Game View (*Preview Camera*).
4. Klik menu atas Unity:
   `Tools > Icon Studio > Export Icon (Auto Save to Project)`
5. Hasil foto otomatis:
   * Disimpan dalam format PNG transparan di folder: `Assets/IconStudio/Exports/`.
   * Otomatis di-set Texture Type menjadi **Sprite (2D and UI)** dengan pengaturan pixel-perfect yang siap ditarik ke komponen Image UI.

---

## 7. nTools UV Inspector

* **Kategori:** 3D Art & Modeling Tool
* **Lokasi di Proyek:** `Assets/nTools/UVInspector`
* **Peran dalam Game:** Membantu tim 3D Environment (Faris) dan Character (Haidar) memvalidasi UV layout model 3D langsung di dalam Unity tanpa perlu bolak-balik membuka software 3D eksternal.

### 🚀 Cara Menggunakan
1. Buka menu atas: `Window > nTools > UV Inspector`.
2. Pilih GameObject bermesh atau asset model 3D di Project Window.
3. Jendela UV Inspector akan menampilkan proyeksi UV map channel 1 atau channel 2 (lightmap) untuk mendeteksi distorsi, overlapping UV, atau kerapatan texel (*texel density*).

---

## 8. Package Resmi Unity yang Digunakan

### 8.1. Unity Input System
* **Package:** `com.unity.inputsystem` (v1.14.0)
* **File Asset Pengaturan:** `Assets/InputSystem_Actions.inputactions`
* **Penggunaan:** Mendukung kontrol *Local Co-op* (Player 1 menggunakan Gamepad/Keyboard A, Player 2 menggunakan Gamepad/Keyboard B atau split keyboard).
* **Binding Utama:**
  * `Move`: Vector2 (WASD / Left Stick)
  * `Sprint / Dash`: Button (Shift / East Button)
  * `Interact / Grab`: Button (E / South Button)
  * `Throw / Drop`: Button (Space / West Button)
  * `Clean Action`: Button (F / North Button)

### 8.2. AI Navigation (NavMesh)
* **Package:** `com.unity.ai.navigation` (v2.0.8)
* **Penggunaan:** Digunakan untuk memetakan area jalan ruangan (*walkable surface*), mendeteksi rintangan dinamis (furnitur yang ditaruh sembarangan sebagai NavMesh Obstacle), atau NPC keluarga/tamu yang bergerak memasuki rumah saat level selesai.

### 8.3. Universal Render Pipeline (URP)
* **Package:** `com.unity.render-pipelines.universal` (v17.1.0)
* **Penggunaan:** Pipeline grafis utama proyek yang mendukung integrasi Custom Renderer Feature (seperti *Free Outline* dari Linework Lite), performa tinggi 60 FPS, serta shader stylized untuk nuansa *The Romantic*.

---

## 📌 Ringkasan Distribusi Pemakaian oleh Tim

| Role / Anggota Tim | Package yang Relevan |
| :--- | :--- |
| **Abdi & Raffa** (Gameplay & Character) | `NeatoTags`, `Input System`, `AI Navigation` |
| **Fito** (Juice & VFX) | `DOTween`, `Linework Lite`, `Sortify Attributes` |
| **Rasyid** (UI & Shader) | `DOTween`, `Linework Lite`, `PlayerPrefs Editor` |
| **Faris & Haidar** (3D Env & Character) | `nTools UV Inspector`, `Icon Studio` |
| **Irfan & Nikkow** (UI/UX & Sound) | `Icon Studio`, `DOTween` (UI animation) |

---
*Dokumentasi ini dibuat untuk referensi tim pengembang Gimersia 2026.*
