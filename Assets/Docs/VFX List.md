# ✨ DAFTAR LENGKAP VISUAL EFFECTS (VFX LIST)
## *No, Please Clean the Room!* (#GIMERSIA2026)
### *Panduan Produksi & Implementasi Efek Visual "Cozy Toy Diorama"*

> Dokumen ini membedah seluruh kebutuhan efek visual (VFX) untuk game *No, Please Clean the Room!*. Dirancang khusus untuk **Fito** (Juice & VFX Artist) dan programmer terkait (**Abdi, Raffa, Rasyid**).
> 
> **Rujukan Terkait:**
> - [**`GDD.md`**](file:///D:/UnityProject/Gimersia226/Assets/Docs/GDD.md) (Visi Game & Core Loop)
> - [**`Art Direction.md`**](file:///D:/UnityProject/Gimersia226/Assets/Docs/Art%20Direction.md) (§13 VFX dan Juice, §12 State Kotor ↔ Bersih)
> - [**`Animation Documentation.md`**](file:///D:/UnityProject/Gimersia226/Assets/Docs/Animation%20Documentation.md) (§12 Animation Events)
> - [**`Pretelan Seluruh System Mekanik.md`**](file:///D:/UnityProject/Gimersia226/Assets/Docs/Pretelan%20Seluruh%20System%20Mekanik.md) (Bagian 3: Feel & Juice)

---

## 🎨 ATURAN DASAR & BUDGET TEKNIS VFX

1. **Gaya Visual:** Kartun 2D/3D non-realistis (*Exaggerated Comic / Toy Diorama*). Bentuk bulat membal, bevel lembut, tanpa partikel fotorealistik atau asap kabur realistis.
2. **Strictly NO Generative AI Assets (ò_ó):** Seluruh tekstur sprite partikel digambar manual (Krita/Photoshop) dalam resolusi ringan (128×128 atau 256×256 PNG transparan).
3. **Budget Partikel (URP 17 - Unity 6):**
   * Maksimal **≤ 300 partikel aktif** total per viewport kamera (sangat krusial untuk performa stabil 60 FPS saat split-screen).
   * Material: Shader **`Universal Render Pipeline/Particles/Unlit`**.
   * Blend Mode: *Alpha Blend* untuk elemen organik (debu, sabun, hati), *Additive* khusus kilau cahaya bintang (*sparkle*).
4. **Palet Warna Resmi ("The Romantic"):**
   * 🌸 **Rose Salmon:** `#F2AAAA`
   * 🌺 **Coral Blush:** `#E38382` (Aksen P1, hati peluk, alert)
   * 🩵 **Pastel Aqua:** `#A8D8DF` (Aksen P2, busa sabun, air)
   * 🌌 **Deep Plum:** `#251F44` (Garis komik, shadow)
   * 💛 **Star Yellow / Gold:** `#FFD166` / `#F2C14E` (Kilau bersih & bintang reward)
   * 🤍 **Paper Cream:** `#FFF9F2` (Core highlight awan debu)
   * 🟤 **Grime Brown:** `#B9A38A` / `#8E7A5F` / `#5F5240` (Debu & noda kotor)

---

## 🏷️ SISTEM LABELING PRIORITAS

* **`[P0 - CORE MVP]`**: Wajib selesai di **Hari 1–3**. Efek esensial yang menjadi indikator langsung gameplay kebersihan dan penataan.
* **`[P1 - CLARITY & ANTI-BUG]`**: Wajib selesai di **Hari 4–5**. Efek visual penjelas aksi, peringatan bahaya, dan pencegah kebingungan pemain.
* **`[P2 - FEEL & JUICE]`**: Diselesaikan di **Hari 6**. Efek pemoles sensasi komedi, kepuasan visual, dan dinamika chemistry pasangan.

---

## 1. KATEGORI SANITASI & KEBERSIHAN (CLEANING CORE)

Efek visual inti yang menjadi indikator bahwa lantai dan perabot sedang/sudah dibersihkan.

### 1.1. Busa Sabun Pel (`VFX_SoapBubble`) `[P0 - CORE MVP]`
* **Trigger (Pemicu):** Dipicu dari Animation Event `MopSplash` (frame 8 & 32 pada clip animasi `ANIM_Clean_Mop`) saat karakter mengayunkan kain pel di atas noda genangan air/sirup.
* **Deskripsi Visual:** 4–6 gelembung sabun bulat transparan warna Pastel Aqua (`#A8D8DF`, alpha 0.6) mengapung naik perlahan ke udara dengan goyangan halus, lalu meletup (*pop*) satu per satu.
* **Spesifikasi Partikel:**
  * Durasi / Lifetime: `0.6 – 0.8 detik`
  * Start Size: `0.15m – 0.25m`
  * Start Speed: `0.8 m/s` (naik vertikal + drifting XZ)
  * Material: `M_VFX_Bubble` (URP Unlit, Alpha Blend)
  * Target Script: `CleaningSystemManager.cs` ➔ `ParticleEffectTrigger.cs`

---

### 1.2. Kilau Bersih Kinclong (`VFX_CleanSparkle`) `[P0 - CORE MVP]`
* **Trigger (Pemicu):**
  1. Saat suatu petak noda berhasil dibersihkan hingga 0%.
  2. Saat seluruh ruangan mencapai threshold kebersihan ≥ 95% (Room Complete).
* **Deskripsi Visual:** Letupan 6–10 bintang 4-sudut (*4-point comic star*) warna Star Yellow (`#FFD166`) dan Paper Cream (`#FFF9F2`) yang memancar ke luar sambil berputar dan membesar lalu mengecil.
* **Spesifikasi Partikel:**
  * Durasi / Lifetime: `0.4 detik` (burst instan)
  * Start Size: `0.2m – 0.35m` (punch size curve)
  * Material: `M_VFX_Sparkle` (URP Unlit, Additive Blend)
  * Audio Sinkron: `SFX_Snap_Success` ("Ting!")

---

### 1.3. Kibasan Debu Kemoceng (`VFX_DustWipe`) `[P0 - CORE MVP]`
* **Trigger (Pemicu):** Saat alat kemoceng/sapu disapukan ke rak buku atau meja berdebu (Grime L1).
* **Deskripsi Visual:** Partikel remah debu tipis warna Grime Light (`#B9A38A`) terlempar ke samping searah kibasan kemoceng lalu memudar cepat di udara.
* **Spesifikasi Partikel:**
  * Durasi: `0.3 detik` (8 partikel)
  * Emission: Burst 8 partikel per ayunan

---

### 1.4. Transisi Pudar Noda (`VFX_GrimeOverlay_Fade`) `[P0 - CORE MVP]`
* **Trigger (Pemicu):** Realtime saat pemain menahan tombol pel di atas noda 3-level.
* **Deskripsi Visual:** Mesh overlay noda di lantai (quad transparan) memudarkan nilai `Alpha` materialnya secara bertahap dari 1.0 ke 0.0 seiring progres persentase kebersihan naik.
* **Teknis Implementasi:** Menggunakan `MaterialPropertyBlock` pada renderer overlay noda agar tidak merusak SRP Batcher.

---

### 1.5. Kilap Refleksi Lantai Bersih (`VFX_CleanFloor_Sheen`) `[P1 - CLARITY]`
* **Trigger (Pemicu):** Aktif pasif setelah lantai ruangan dinyatakan tuntas bersih 100%.
* **Deskripsi Visual:** Lantai yang tadinya kusam (Smoothness 0.05, kotor) bertransisi menjadi mengkilap basah (Smoothness 0.40) yang memantulkan pantulan hangat lampu ruangan (*Cozy Clean Atmosphere*).

---

## 2. KATEGORI KARAKTER & LOCOMOTION (MOVEMENT & ACTIONS)

Memberikan bobot fisik dan kelucuan gerak pada Suami (P1) dan Istri (P2).

### 2.1. Awan Debu Terjang & Rem (`VFX_DashSkid_Dust`) `[P0 - CORE MVP]`
* **Trigger (Pemicu):**
  1. Saat tombol Dash ditekan (Event `DashWhoosh` frame 6 clip `ANIM_Dash_Base`).
  2. Di akhir durasi dash saat karakter melakukan pengereman komik (*skid*).
* **Deskripsi Visual:** Kepulan 8–12 awan debu bulat puff warna krem `#C9BBAE` dengan inti Paper Cream `#FFF9F2` yang memancar horizontal ke belakang kaki karakter.
* **Spesifikasi Partikel:**
  * Durasi: `0.4 – 0.5 detik`
  * Size over Lifetime: Membesar dari `0.2m` ke `0.45m` lalu memudar alpha.

---

### 2.2. Debu Langkah Lari (`VFX_Footstep_Dust`) `[P1 - CLARITY]`
* **Trigger (Pemicu):** Animation Event `Footstep_L` dan `Footstep_R` saat karakter berlari di ruangan yang masih kotor/berdebu.
* **Deskripsi Visual:** Puff debu mini (2–3 partikel kecil) meletup di bawah tumit sepatu karakter. Otomatis berhenti muncul jika lantai sudah bersih.

---

### 2.3. Tabrakan Adu Kepala Antar Pemain (`VFX_PlayfulClash_Stars`) `[P2 - FEEL & JUICE]`
* **Trigger (Pemicu):** Terjadi saat P1 dan P2 saling bertubrukan kencang ketika sama-sama melakukan Sprint/Dash.
* **Deskripsi Visual:** Bintang komik mini (*dizzy stars*) warna kuning berputar melingkar di atas kepala kedua karakter selama 0.8 detik saat keduanya membal mundur 0.5 meter.

---

### 2.4. Cipratan Terpeleset Air (`VFX_SlipSplash`) `[P2 - FEEL & JUICE]`
* **Trigger (Pemicu):** Saat karakter berlari kencang menerobos genangan air kotor L3 tanpa membawa pel (Event `Slip` clip `ANIM_Stumble`).
* **Deskripsi Visual:** Cipratan air kotor memercik ke samping kiri-kanan setinggi 0.5m disertai karakter yang terhuyung lucu.

---

## 3. KATEGORI FISIKA PERABOT & PENATAAN (PHYSICS & DROP-ZONE)

Menjamin kejelasan penataan perabot dan menghilangkan kebingungan posisi.

### 3.1. Denyut Marker Penataan (`VFX_DropZone_Pulse`) `[P0 - CORE MVP]`
* **Trigger (Pemicu):** Aktif pasif di lantai/dinding menandakan lokasi ideal tempat menaruh furnitur.
* **Deskripsi Visual:** Mesh plane horizontal transparan (opasitas 30%) dengan garis tepi warna Mint (`#9FE3C2`) yang berdenyut lembut (*breathing alpha 0.25 ➔ 0.45 ➔ 0.25* per 1.5 detik).

---

### 3.2. Sorotan Drop-Zone Sah (`VFX_DropZone_ValidHighlight`) `[P0 - CORE MVP]`
* **Trigger (Pemicu):** Saat pemain membawa furnitur masuk ke dalam radius toleransi drop-zone (jarak < 0.5m, sudut ±35°).
* **Deskripsi Visual:** Warna outline drop-zone seketika berganti dari Mint menjadi **Paper Cream terang (`#FFF9F2`)** berdenyut cepat, memberi sinyal visual jelas: *"Lepas sekarang untuk snap!"*.

---

### 3.3. Letupan Snap Selesai Tertata (`VFX_SnapSuccess_Burst`) `[P0 - CORE MVP]`
* **Trigger (Pemicu):** Saat perabot dilepas di atas drop-zone yang sah dan posisinya otomatis terkunci magnetik (*magnetic snap*).
* **Deskripsi Visual:**
  1. Furnitur membal kenyal 1.15x via DOTween.
  2. Meletupkan cincin gelombang kejut tipis (*shockwave ring*) melebar di lantai.
  3. Bintang sparkle kuning emas meletup di sekeliling sudut furnitur.
* **Audio Sinkron:** `SFX_Snap_Success`.

---

### 3.4. Dentuman Debu Perabot Jatuh (`VFX_HeavyThud_Dust`) `[P1 - CLARITY]`
* **Trigger (Pemicu):** Saat perabot kelas Medium atau Heavy (kulkas, sofa, kardus besar) dijatuhkan atau mendarat di lantai setelah dilempar.
* **Deskripsi Visual:** Cincin kepulan debu tebal memancar mendatar di sekeliling dasar furnitur (durasi 0.35s) sebanding dengan bobot benda.

---

### 3.5. Garis Tabrakan Komik (`VFX_ComicImpact_Lines`) `[P2 - FEEL & JUICE]`
* **Trigger (Pemicu):** Saat barang yang dilempar membentur dinding keras dengan kecepatan tinggi (> 8 m/s).
* **Deskripsi Visual:** Sprite garis kejut radial komik warna Deep Plum (`#251F44`) muncul seketika di titik benturan selama 0.15 detik disertai hentakan getar kamera mikro (*micro screen-shake*).

---

### 3.6. Kepulan Asap Respawn Barang Nyasar (`VFX_OOB_RespawnPuff`) `[P1 - CLARITY & ANTI-BUG]`
* **Trigger (Pemicu):** Saat barang terlempar ke luar batas map dan diselamatkan oleh trigger `ItemOutOfBoundsRecoverer`.
* **Deskripsi Visual:** Kepulan awan asap tebal kartun berbentuk donat meletup di titik tengah ruangan saat barang muncul kembali, menyamarkan perpindahan instan agar terasa seperti efek sulap kartun.

---

## 4. KATEGORI CHEMISTRY PASANGAN & MOOD (CO-OP EMOTES)

Visualisasi ikatan emosional dan dinamika gotong royong suami-istri.

### 4.1. Efek Cinta Pelukan Pasangan (`VFX_Heart_Hug`) `[P0 - CORE MVP]`
* **Trigger (Pemicu):** Dipicu dari Animation Event `EmoteHug` saat P1 dan P2 saling berpelukan selama 1.5 detik via menu emote.
* **Deskripsi Visual:** 6–10 partikel hati kartun warna Coral Blush (`#E38382`) dan Rose Salmon (`#F2AAAA`) memancar membubung naik dari tengah pelukan kedua karakter sambil berputar pelan dan membesar.
* **Gameplay Feedback:** Menandai pemulihan instan bar Mood sebesar +50%.

---

### 4.2. Percikan Bintang Tos Tangan (`VFX_HighFive_Spark`) `[P1 - CLARITY]`
* **Trigger (Pemicu):** Dipicu dari Animation Event `EmoteClap` saat kedua telapak tangan karakter bertemu di udara melakukan toss ceria.
* **Deskripsi Visual:** Kilatan percikan bintang terang meletup tajam di antara kedua telapak tangan selama 0.25 detik.
* **Gameplay Feedback:** Menandai pemulihan bar Mood sebesar +25%.

---

### 4.3. Tetes Keringat Bete / Lelah (`VFX_SweatDrop_Bete`) `[P1 - CLARITY]`
* **Trigger (Pemicu):** Aktif looping saat nilai Mood karakter berada di bawah 25% (State *Bete*) atau stamina lari habis total (*Exhausted*).
* **Deskripsi Visual:** Tetesan air keringat kartun warna biru pastel aqua menetes miring di samping dahi karakter setiap 1.5 detik sekali.

---

### 4.4. Meletupnya Balon Obrolan Kepala (`VFX_EmoteBubble_Pop`) `[P2 - FEEL & JUICE]`
* **Trigger (Pemicu):** Saat pemain memilih salah satu menu emote atau saat karakter memicu dialog reaksi otomatis.
* **Deskripsi Visual:** Balon kata komik 2D di atas kepala membal meletup (*scale 0 ➔ 1.15 ➔ 1.0*) via DOTween, mengapung naik 20 cm selama 1.2 detik lalu mengecil menghilang.

---

## 5. KATEGORI ATMOSFER RUANGAN & WAKTU KRITIS (CLIMAX & WIN/LOSE)

Membangun intensitas dan kepanikan menyenangkan menjelang tamu tiba.

### 5.1. Denyut Merah Waktu Kritis (`VFX_TimeCritical_Pulse`) `[P1 - CLARITY]`
* **Trigger (Pemicu):** Aktif looping saat waktu level tersisa **≤ 30 detik**.
* **Deskripsi Visual:** Tepi layar (vignette lembut) berdenyut merah transparan (*Critical Red `#C94F4F`*, opasitas 15%) mengikuti irama detak jam dinding komik.

---

### 5.2. Gelombang Kejut Bel Pintu Tamu (`VFX_DoorbellRing_Shockwave`) `[P1 - CLARITY]`
* **Trigger (Pemicu):** Tepat saat timer mencapai 00:00 dan suara bel rumah *"Ding-Dong!"* berdentang.
* **Deskripsi Visual:** Gelombang kejut garis komik bergetar memancar 3 kali berturut-turut dari arah pintu utama rumah, memberi tanda waktu telah habis!

---

### 5.3. Hujan Konfeti Kemenangan (`VFX_Celebration_Confetti`) `[P2 - FEEL & JUICE]`
* **Trigger (Pemicu):** Saat layar kemenangan (*ScoreResultsUI*) terbuka dan pemain berhasil meraih minimal Bintang 1 (kebersihan ≥ 70%).
* **Deskripsi Visual:** Hujan potongan pita kertas warna-warni palet brand (Coral, Aqua, Gold, Cream) berjatuhan bergoyang santai dari atas layar.

---

## 📊 TABEL MATRIKS PRODUKSI VFX

| ID Prefab VFX | Nama Efek | Kategori | Pemicu Utama (Trigger) | Material / Shader | Prioritas | PIC |
| :--- | :--- | :--- | :--- | :--- | :---: | :---: |
| `VFX_SoapBubble` | Busa Sabun Pel | Sanitasi | Event `MopSplash` (Clip Mop) | URP Unlit (Alpha Blend) | **[P0]** | Fito |
| `VFX_CleanSparkle` | Kilau Bintang Kinclong | Sanitasi | Noda 0% / Ruangan 95% Clean | URP Unlit (Additive) | **[P0]** | Fito |
| `VFX_DustWipe` | Kibasan Debu Kemoceng | Sanitasi | Ayunan kemoceng di rak/meja | URP Unlit (Alpha Blend) | **[P0]** | Fito |
| `VFX_GrimeOverlay_Fade` | Pudar Alpha Noda | Sanitasi | Tahan tombol pel di noda | URP Lit / MaterialBlock | **[P0]** | Fito / Rasyid |
| `VFX_DashSkid_Dust` | Awan Debu Dash & Rem | Locomotion | Event `DashWhoosh` / Rem kaki | URP Unlit (Alpha Blend) | **[P0]** | Fito |
| `VFX_DropZone_Pulse` | Denyut Marker Sasaran | Penataan | Pasif di lantai drop-zone | URP Lit / Unlit Alpha | **[P0]** | Fito / Faris |
| `VFX_DropZone_ValidHighlight` | Sorotan Target Sah | Penataan | Bawa perabot dekat drop-zone | Linework Lite / Material | **[P0]** | Rasyid |
| `VFX_SnapSuccess_Burst` | Ledakan Snap Rapi | Penataan | Lepas perabot pas di sasaran | URP Unlit (Additive) | **[P0]** | Fito |
| `VFX_Heart_Hug` | Hati Cinta Pelukan | Chemistry | Event `EmoteHug` (Clip Peluk) | URP Unlit (Alpha Blend) | **[P0]** | Fito |
| `VFX_CleanFloor_Sheen` | Kilap Lantai Bersih | Sanitasi | Ruangan tuntas 100% bersih | Material Smoothness Lerp | **[P1]** | Rasyid |
| `VFX_Footstep_Dust` | Debu Langkah Kaki | Locomotion | Event `Footstep` di lantai kotor | URP Unlit (Alpha Blend) | **[P1]** | Fito |
| `VFX_HeavyThud_Dust` | Dentuman Debu Perabot | Penataan | Barang berat mendarat di lantai | URP Unlit (Alpha Blend) | **[P1]** | Fito |
| `VFX_OOB_RespawnPuff` | Awan Respawn OOB | Anti-Bug | Trigger Out-of-Bounds aktif | URP Unlit (Alpha Blend) | **[P1]** | Fito |
| `VFX_HighFive_Spark` | Percikan Tos Tangan | Chemistry | Event `EmoteClap` (Clip Tos) | URP Unlit (Additive) | **[P1]** | Fito |
| `VFX_SweatDrop_Bete` | Keringat Bete / Lelah | Chemistry | Mood < 25% / Stamina habis | URP Unlit (Alpha Blend) | **[P1]** | Fito |
| `VFX_TimeCritical_Pulse` | Denyut Merah Waktu Kritis | Atmosfer | Timer level sisa ≤ 30 detik | Post-Processing / Canvas | **[P1]** | Rasyid |
| `VFX_DoorbellRing_Shockwave`| Gelombang Bel Tamu | Atmosfer | Timer mencapai 00:00 | URP Unlit Sprite | **[P1]** | Fito |
| `VFX_PlayfulClash_Stars` | Bintang Tabrakan Dash | Locomotion | P1 dan P2 tabrakan saat dash | URP Unlit (Additive) | **[P2]** | Fito |
| `VFX_SlipSplash` | Cipratan Terpeleset | Locomotion | Event `Slip` di genangan air | URP Unlit (Alpha Blend) | **[P2]** | Fito |
| `VFX_ComicImpact_Lines` | Garis Tabrakan Komik | Penataan | Lemparan keras bentur tembok | URP Unlit (Alpha Blend) | **[P2]** | Fito |
| `VFX_EmoteBubble_Pop` | Balon Obrolan Kepala | Chemistry | Menu radial emote dieksekusi | Canvas / Sprite DOTween | **[P2]** | Fito / Nikkow |
| `VFX_Celebration_Confetti` | Hujan Konfeti Menang | Kemenangan | Skor hasil Bintang ≥ 1 | Canvas / URP Unlit | **[P2]** | Fito |

---

## 💻 INTEGRASI KODE C# (PANDUAN PROGRAMMER)

Seluruh prefab partikel di atas diinstansiasi atau dipicu melalui script terpusat:
`Assets/Resources/Scripts/Juice&Feel/ParticleEffectTrigger.cs`

Contoh implementasi pemanggilan terpusat:

```csharp
using UnityEngine;

namespace Gimersiah.Juice
{
    public class ParticleEffectTrigger : MonoBehaviour
    {
        [Header("Sanitasi VFX")]
        [SerializeField] private ParticleSystem _soapBubblePrefab;
        [SerializeField] private ParticleSystem _cleanSparklePrefab;

        [Header("Locomotion VFX")]
        [SerializeField] private ParticleSystem _dashDustPrefab;

        [Header("Chemistry VFX")]
        [SerializeField] private ParticleSystem _heartHugPrefab;
        [SerializeField] private ParticleSystem _highFiveSparkPrefab;

        // Dipanggil via C# Event saat membersihkan noda
        public void PlaySoapBubble(Vector3 position)
        {
            Instantiate(_soapBubblePrefab, position, Quaternion.identity);
        }

        // Dipanggil saat noda 0% atau drop-zone snap
        public void PlaySparkle(Vector3 position)
        {
            Instantiate(_cleanSparklePrefab, position, Quaternion.identity);
        }

        // Dipanggil saat P1 dan P2 berpelukan
        public void PlayHeartHug(Vector3 position)
        {
            Instantiate(_heartHugPrefab, position, Quaternion.identity);
        }
    }
}
```

---
*Dokumen ini disusun sebagai acuan resmi produksi efek visual untuk tim art dan programmer gim "No, Please Clean the Room!" pada kompetisi Gimersia 2026.*
