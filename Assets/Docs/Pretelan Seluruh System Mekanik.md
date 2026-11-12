# 🛠️ PRETELAN LENGKAP SISTEM & MEKANIK GAMEPLAY
## *No, Please Clean the Room!* (#GIMERSIA2026)
### *Master Production Specification: Playable, Fun, Zero-Stuck & Bug-Free*

> Dokumen ini adalah spesifikasi teknis operasional untuk tim programmer (**Abdi, Raffa, Rasyid**) dan designer/juice (**Fito, Irfan, Nikkow**). Membedah seluruh mekanik yang diadaptasi dari **Moving Out 1**, dimodifikasi sesuai GDD (*Asymmetric Co-op*, *Cleaning & Tidy-Up*, *Gotong Royong*, dan *Mood Chemistry*), lengkap dengan **label prioritas produksi**, **protokol pencegahan bug/stuck**, **tuning parameter**, dan **pretelan arsitektur UI Programming**.
> 
> **Daftar Isi:**
> 1. [BAGIAN 1: CCC (Camera, Character, Control)](#bagian-1-ccc-camera-character-control)
> 2. [BAGIAN 2: SYSTEM & GAMEPLAY](#bagian-2-system--gameplay)
> 3. [BAGIAN 3: FEEL & JUICE](#bagian-3-feel--juice)
> 4. [BAGIAN 4: UI & HUD PROGRAMMING](#bagian-4-ui--hud-programming)
> 5. [MASTER CHECKLIST AUDIT ZERO-BUG](#-master-checklist-kesiapan-gameplay-zero-bug-audit)

---

## 🏷️ SISTEM LABELING PRIORITAS SPRINT (7 HARI)

Setiap fitur dan sub-sistem di bawah ini diberi label prioritas tegas:
* **`[P0 - CORE MVP]`**: Wajib selesai di **Hari 1–3**. Syarat mutlak game bisa dimainkan dari awal sampai akhir (*walkable, grabbable, cleanable, basic HUD, win/lose*).
* **`[P1 - ANTI-BUG & ANTI-STUCK]`**: Wajib selesai di **Hari 4–5**. Menjamin game **stabil, no-stuck, no-fall-through, no-softlock, auto-recover, dan UI gamepad navigation bebas deadlock**.
* **`[P2 - FEEL, JUICE & FUN]`**: Diselesaikan di **Hari 6**. Memoles rasa permainan (*squash/stretch, auto-aim pass, comic stumble, emote radial wheel, visual audio juice*).
* **`[P3 - POLISH / NICE-TO-HAVE]`**: Tambahan **Hari 7** jika seluruh P0–P2 sudah lulus QA.

---

## 🎮 TABEL KONTROL RESMI (NEW INPUT SYSTEM)

Konfigurasi multi-device lokal (*Shared Screen / Split-Screen*):

| Aksi Gameplay | Gamepad (P1 & P2) | Keyboard P1 (Suami - WASD) | Keyboard P2 (Istri - Numpad) |
| :--- | :--- | :--- | :--- |
| **Movement (8-Arah)** | Left Analog Stick (360°) | `W`, `A`, `S`, `D` | `Arrow Keys` (↑, ←, ↓, →) |
| **Grab / PickUp / Drop** | South Button (`A` / `✕`) | `E` | `Numpad 1` / `Keypad Enter` |
| **Throw / Toss (Lempar)** | West Button (`X` / `□`) | `Space` | `Numpad 2` |
| **Sprint / Dash** | East Button (`B` / `○`) | `Left Shift` | `Right Shift` |
| **Clean Action (Pel/Sapu)** | North Button (`Y` / `△`) / `RT` | `F` | `Numpad 0` / `Right Ctrl` |
| **Emote Radial Wheel** | Left Bumper (`LB` / `L1`) (Hold) | `Q` / `Tab` (Hold) | `Numpad .` (Hold) |
| **Pause / Menu** | Start / Menu Button | `Escape` | `Escape` |

---

# BAGIAN 1: CCC (CAMERA, CHARACTER, CONTROL)

Lokasi Script: `Assets/Resources/Scripts/CCC/`

```
Assets/Resources/Scripts/CCC/
├── PlayerMovementController.cs
├── PlayerInteractionController.cs
├── PlayerCarryController.cs
├── PlayerThrowController.cs
├── PlayerStaminaController.cs
├── CoOpLiftController.cs
└── DynamicCameraController.cs
```

---

### 1.1. Player Locomotion & Asymmetric Physics `[P0 - CORE MVP]`
* **Deskripsi & Modifikasi Moving Out 1:**
  Moving Out menggunakan karakter ragdoll lentur yang sering tergelincir. Di game kami, karakter menggunakan **Kinematic-driven Rigidbody dengan Capsule Collider kaku** untuk memastikan pergerakan responsif (*tight controls*) tanpa sliding liar, namun tetap memiliki inersia komikal.
  * **P1 - Suami (Spesialis Beban):**
    * Base Move Speed: `3.5 m/s`
    * Acceleration: `18 m/s²` (mantap dan berbobot)
    * Karakter berat: tidak mudah terdorong oleh tabrakan perabot.
  * **P2 - Istri (Spesialis Kelincahan):**
    * Base Move Speed: `4.2 m/s` (20% lebih cepat)
    * Acceleration: `30 m/s²` (instan dan sangat lincah)
    * Karakter ringan: lincah di lorong sempit, namun recoil lebih besar saat menabrak perabot berat.
* **Kontrol:**
  * Gamepad: `Left Stick` (Analog)
  * Keyboard: `WASD` (P1) / `Arrow Keys` (P2)
* **Protokol No-Bug & Anti-Stuck `[P1]`: `Zero-Friction Capsule`**
  * Player Capsule Collider dipasangi **PhysicMaterial "PM_Player"** dengan `Dynamic Friction = 0`, `Static Friction = 0`, dan `Friction Combine = Minimum`.
  * *Hasil:* Pemain **TIDAK AKAN PERNAH menempel atau tersangkut** di kusen pintu, tepi meja, atau sudut dinding saat berjalan menyerong.
* **Target Script:** `PlayerMovementController.cs`

---

### 1.2. Sprint Dash & Stamina Management `[P0 - CORE MVP]`
* **Deskripsi & Modifikasi Moving Out 1:**
  Menggantikan tombol lompat/dive Moving Out dengan Dash akselerasi horizontal:
  * Tombol ditekan ➔ Burst kecepatan maju instan selama `0.22 detik`.
  * **P1 (Suami):** Dash stabil, biaya stamina 20%, regenerasi stamina cepat (3 detik penuh).
  * **P2 (Istri):** Dash sangat kencang (kecepatan 8 m/s), biaya stamina 35%, ada animasi rem komik (*skid*).
  * **Kondisi Terkunci:** Dash otomatis dinonaktifkan saat memegang barang kelas *Heavy* atau saat angkat berdua.
* **Kontrol:**
  * Gamepad: `East Button` (`B` / `○`)
  * Keyboard: `Left Shift` (P1) / `Right Shift` (P2)
* **Protokol Fun & Responsif `[P2]`: `Input Buffering & Coyote Time`**
  * **Buffer Window (0.15 detik):** Jika pemain menekan Dash 0.15 detik sebelum animasi dash sebelumnya selesai, karakter langsung dash kembali tanpa delay input.
  * **Playful Bump:** Jika P1 dan P2 saling bertabrakan saat dash, keduanya terpental mundur sejauh 0.5m dengan partikel bintang (*playful clash*).
* **Target Script:** `PlayerStaminaController.cs`

---

### 1.3. Smart Grab, Socket Carry & Weight System `[P0 - CORE MVP]`
* **Deskripsi & Modifikasi Moving Out 1:**
  Moving Out menggunakan physics joint yang sering membuat barang melayang atau terpelintir ke dalam dinding (*physics clipping*). Game ini menggunakan **Socket Attachment System** yang kokoh:
  * Saat di-grab, objek dinonaktifkan Rigidbodynya dan di-parent ke Socket karakter (`HandSocket` / `CarryPivot`).
  * Collider objek dialihkan ke layer `CarriedItem` yang menonaktifkan tabrakan dengan badan pemain pembawa, tetapi **tetap bertabrakan dengan dinding**.
* **Klasifikasi Berat Benda:**
  1. **Light (Buku, Piring, Bantal, Sampah):** Socket `HandSocket_R`, kecepatan jalan 100%.
  2. **Medium (Kardus Parsel, Kursi, Ember Pel, Meja Kopi):** Socket `CarryPivot`, P1 jalan normal, P2 jalan 70%.
  3. **Heavy (Kulkas, Sofa Coral, Kasur, Lemari):** P1 Solo geser lambat (45%), P2 Solo **gagal angkat** (muncul prompt butuh bantuan), P1+P2 Co-op jalan normal (100%).
* **Kontrol:**
  * Gamepad: `South Button` (`A` / `✕`)
  * Keyboard: `E` (P1) / `Numpad 1` (P2)
* **Protokol No-Bug & Anti-Stuck `[P1]`: `Safe Drop SphereCast`**
  * Script melakukan `Physics.SphereCast` dari socket ke depan-bawah: jika terhalang dinding/objek lain, barang otomatis diletakkan di titik bebas terdekat (*fallback safe radius* 0.6m). Objek **dijamin tidak pernah tembus/terjepit di dalam dinding**.
* **Target Script:** `PlayerCarryController.cs`, `PlayerInteractionController.cs`

---

### 1.4. Throw, Pass & Catch System `[P0 - CORE MVP]`
* **Deskripsi & Modifikasi Moving Out 1:**
  Mekanisme melempar barang dimodifikasi untuk mendukung **estafet antar ruangan**:
  * Tahan tombol throw ➔ trajektori busur titik-titik (*trajectory arc*) ➔ Lepas untuk melempar.
  * **P1 (Suami - Cannon Arm):** Gaya lempar maksimal `14 m/s` (mampu melempar kardus melintasi 2 sekat ruangan).
  * **P2 (Istri - Soft Toss):** Gaya lempar maksimal `6 m/s` (lemparan pelan jarak dekat 1-2 meter).
* **Kontrol:**
  * Tahan `West Button` (`X` / `□`) atau `Space` / `Numpad 2` ➔ Arahkan ➔ Lepas.
* **Protokol Fun & Playable `[P2]`: `Magnetic Auto-Aim / Pass Assist`**
  * Trajektori otomatis melengkung halus mengarah ke dada pasangan jika berada dalam kerucut `±20°`. Pasangan yang berdiri menghadap arah datangnya barang otomatis menangkapnya (*instant catch*) dengan bunyi *"Plop!"*.
* **Target Script:** `PlayerThrowController.cs`

---

### 1.5. Two-Player Co-op Lift (Gotong Royong Perabot Berat) `[P0 - CORE MVP]`
* **Deskripsi Gameplay:**
  Dua pemain memegang sisi kiri dan kanan perabot berat (Sofa Coral, Kulkas Aqua). Keduanya mengunci socket ➔ status objek menjadi `IsCoOpCarried = true` ➔ kecepatan gerak disamakan menjadi `3.8 m/s`.
* **Kontrol:** Berdiri di sisi soket furnitur ➔ Tahan `South Button` (`A` / `E` / `Numpad 1`).
* **Protokol No-Stuck `[P1]`: `Doorway Alignment Assist`**
  * Ketika sofa yang diangkat berdua mendekati ambang pintu (radius 1.5m), sistem secara cerdas memberikan torsi rotasi halus (*soft auto-align*) agar posisi sofa memanjang searah pintu, sehingga **lolos melewati pintu tanpa macet melintang**.
* **Target Script:** `CoOpLiftController.cs`

---

### 1.6. Dynamic 3-Mode Camera & Screen Leash `[P0 - CORE MVP]`
* **Deskripsi Gameplay (Art Direction §3.1):**
  1. **Overview Mode:** Jarak 22m, FOV 30° selama 3 detik di awal level untuk menghafal denah.
  2. **Shared Co-op Mode:** Fokus ke midpoint kedua pemain (jarak 10–14m) saat pemain di ruangan yang sama.
  3. **Vertical Split-Screen (960×1080 per viewport):** Layar terbelah vertikal mulus saat pemain terpisah jarak > 9m atau berada di ruangan berbeda.
* **Protokol No-Stuck `[P1]`: `Anti-Straggle Leash & Teleport Safety`**
  * Jika salah satu pemain tertinggal/AFK > 15 detik, muncul panah kompas penunjuk rekan. Pemain tertinggal dapat menahan tombol interact 3 detik untuk teleport santai ke samping pasangannya.
* **Target Script:** `DynamicCameraController.cs`

---

# BAGIAN 2: SYSTEM & GAMEPLAY

Lokasi Script: `Assets/Resources/Scripts/System/`

```
Assets/Resources/Scripts/System/
├── CleaningSystemManager.cs
├── DropZoneManager.cs
├── LevelTimeManager.cs
├── LevelProgressionManager.cs
├── MoodChemistrySystem.cs
├── DoorInteractionSystem.cs
├── ItemOutOfBoundsRecoverer.cs
└── SaveLoadSystem.cs
```

---

### 2.1. Sanitation & Grime Removal System `[P0 - CORE MVP]`
* **Deskripsi Gameplay:**
  3 Level Grime: L1 Debu (Sapu/Kemoceng), L2 Tumpahan (Kain Pel), L3 Genangan Air (Pel 2 tahap).
  * **P2 (Istri - Master Sanitasi):** Radius ayunan pel `1.5m`, durasi per petak **0.8 detik** (kinclong instan).
  * **P1 (Suami):** Radius ayunan pel `0.9m`, durasi per petak **3.0 detik** (sangat lambat dan canggung).
* **Kontrol:** Tahan `North Button` (`Y` / `△`) atau `F` / `Numpad 0`.
* **Protokol No-Bug `[P1]`: `95% Clean Threshold (Anti-Pixel Hunting)`**
  * Begitu luas noda yang dibersihkan mencapai **≥ 95%**, sistem **otomatis menuntaskan ruangan menjadi 100% CLEAN**! Menghilangkan rasa kesal mencari 1 pixel sisa noda.
* **Target Script:** `CleaningSystemManager.cs`

---

### 2.2. Drop-Zone & Snapping Validation `[P0 - CORE MVP]`
* **Deskripsi Gameplay:**
  Setiap furnitur memiliki drop-zone transparan (*Mint Outline `#9FE3C2`*). Saat barang diletakkan di dalam radius `0.5m` dan sudut `±35°`, barang otomatis tertarik secara magnetik (*DOTween snap*) ke titik ideal dan terkunci kinematis.
* **Protokol No-Bug `[P1]`: `Static Kinematic Lock`**
  * Furnitur yang sudah rapi terkunci agar tidak bisa tertendang berantakan lagi secara sengaja oleh pemain.
* **Target Script:** `DropZoneManager.cs`

---

### 2.3. Emergency Out-of-Bounds (OOB) Item Recovery `[P1 - ANTI-BUG & ANTI-STUCK]`
* **Mekanisme Penanganan:**
  Trigger Box raksasa mengelilingi batas peta. Jika perabot terlempar ke luar peta (Y < -1.0):
  1. Kecepatan dinolkan.
  2. Barang otomatis respawn ke titik tengah ruangan asalnya dengan efek kepulan asap kartun.
  3. **Zero softlock:** Game tidak akan pernah macet karena barang objektif lenyap.
* **Target Script:** `ItemOutOfBoundsRecoverer.cs`

---

### 2.4. Smart Target Selection (Prioritas Jangkauan) `[P1 - ANTI-BUG]`
* Raycast kerucut interaksi memilih objek dengan bobot prioritas: 1) Alat pembersih, 2) Barang ringan, 3) Furnitur berat. Mencegah salah ambil objek saat barang bertumpuk.
* **Target Script:** `PlayerInteractionController.cs`

---

### 2.5. Door & Anti-Pinch Rebound System `[P1 - ANTI-BUG & ANTI-STUCK]`
* Pintu ayun dan pintu geser Level 2 memiliki sensor overlap fisik. Jika ada pemain atau perabot di celah kusen saat pintu menutup, pintu **otomatis membal terbuka kembali selebar 30 cm** dan tidak akan menjepit/menghancurkan karakter.
* **Target Script:** `DoorInteractionSystem.cs`

---

### 2.6. Mood & Chemistry System (Pasangan Suami-Istri) `[P2 - FEEL & FUN]`
* Bar Mood (0–100%). Mood turun jika menabrak dinding, kelelahan, atau di ruangan kotor. State "Bete" (<25%) membuat jalan melambat 15%.
* **Restorasi Gotong Royong:**
  * **Peluk (Hug):** Keduanya berdiri berdekatan ➔ Tahan menu emote ➔ Pilih Peluk. Keduanya berpelukan 1.5s ➔ Mood pulih +50% + hati merah coral.
  * **Tos (High-Five):** Berhadapan setelah menyelesaikan 1 ruangan ➔ Tekan Tos ➔ Mood pulih +25% + sparkle.
* **Target Script:** `MoodChemistrySystem.cs`

---

# BAGIAN 3: FEEL & JUICE

Lokasi Script: `Assets/Resources/Scripts/Juice&Feel/`

```
Assets/Resources/Scripts/Juice&Feel/
├── CartoonJuiceController.cs
├── AudioFeedbackManager.cs
├── ParticleEffectTrigger.cs
├── ScreenShakeJuice.cs
└── OutlineAffordanceController.cs
```

---

### 3.1. Procedural Squash & Stretch (DOTween) `[P2 - FEEL & FUN]`
* Deformasi kenyal volume kartun pada transform `VisualRoot`:
  * Ambil: `DOPunchScale(0.12f, 0.2s)`
  * Lempar: Stretch vertikal `(0.9, 1.15, 0.9)`
  * Mendarat: Squash horizontal `(1.25, 0.75, 1.25)` lalu kembali via `Ease.OutBack`.
  * Snap Drop-Zone: Membal kenyal 1.15x.
* **Target Script:** `CartoonJuiceController.cs`

---

### 3.2. Semantic Multi-Layer Outlines (Linework Lite) `[P0 - CORE MVP]`
* 🩵 **Aqua (`#A8D8DF` - 1.5px):** Objek interaktif umum.
* 🌺 **Coral (`#E38382` - 2.5px):** Furnitur berat (butuh P1 / Co-op).
* 🟢 **Mint (`#9FE3C2` - 1.5px):** Drop-zone sasaran penataan.
* 🤍 **Cream (`#FFF9F2` - 2.5px):** Objek yang sedang dipegang di tangan.
* **Target Script:** `OutlineAffordanceController.cs`

---

### 3.3. Particle Juice Feedback & Audio Feedback `[P2 - FEEL & FUN]`
* Partikel: `VFX_DustPuff` (debu), `VFX_CleanSparkle` (bintang emas), `VFX_SoapBubble` (busa pel), `VFX_Heart_Hug` (hati cinta).
* Audio: Langkah keramik vs kayu, ceburan pel basah, suara renyah *"Ting!"* drop-zone, dan transisi BGM adaptif (tenang ➔ panik heboh saat <30s).
* **Target Script:** `ParticleEffectTrigger.cs`, `AudioFeedbackManager.cs`, `ScreenShakeJuice.cs`

---

# BAGIAN 4: UI & HUD PROGRAMMING

Lokasi Script: `Assets/Resources/Scripts/UI/`

```
Assets/Resources/Scripts/UI/
├── Core/
│   ├── UIManager.cs
│   └── HUDManager.cs
├── HUD/
│   ├── WallClockTimerUI.cs
│   ├── CleanlinessMeterUI.cs
│   ├── TaskChecklistUI.cs
│   ├── PlayerStatusUI.cs
│   ├── MoodGaugeUI.cs
│   └── EmoteWheelUI.cs
├── World/
│   ├── WorldInteractPromptUI.cs
│   ├── CoOpWarningUI.cs
│   ├── WorldEmoteBubbleUI.cs
│   └── PartnerCompassUI.cs
├── SplitScreen/
│   └── SplitScreenUIDivider.cs
└── Screens/
    ├── MainMenuUI.cs
    ├── StageSelectUI.cs
    ├── PauseMenuUI.cs
    ├── ScoreResultsUI.cs
    ├── TimeoutDefeatUI.cs
    └── SettingsMenuUI.cs
```

---

### 4.1. In-Game Gameplay HUD (Dashboard Utama)

#### 4.1.1. Jam Dinding Hitung Mundur (`WallClockTimerUI`) `[P0 - CORE MVP]`
* **Visual & Layout Anchor:**
  * Anchor: `Top-Center` (atau `Top-Right` saat split-screen).
  * Komponen: Jarum jam analog berputar + TextMeshPro Digital Timer (Format `MM:SS`, Font Fredoka Bold 64pt, Warna Deep Plum `#251F44`).
* **Logika Pemrograman:**
  * Subscribe ke event `LevelTimeManager.OnTimeUpdated(float remainingSeconds)`.
  * Setiap detik, jarum jam berputar `6°` dengan punch rotasi halus DOTween: `transform.DOPunchRotation(Vector3.forward * 8f, 0.15f)`.
* **State Kritis (< 30 Detik):**
  * Teks berubah warna menjadi **Critical Red (`#C94F4F`)**.
  * Jam berdenyut membal tanpa henti: `transform.DOScale(1.15f, 0.4s).SetLoops(-1, LoopType.Yoyo)`.
  * Memutar SFX detak jam komik yang semakin cepat.
* **Protokol No-Bug `[P1]`:** Saat level pause, kill tween berulang dan set waktu beku (*unscaled time handling*).

---

#### 4.1.2. Meteran Kebersihan & Milestone 3 Bintang (`CleanlinessMeterUI`) `[P0 - CORE MVP]`
* **Visual & Layout Anchor:**
  * Anchor: `Top-Center` tepat di bawah timer.
  * Komponen: Slider horizontal melengkung (*Cozy Rounded Bar*) dengan 3 badge bintang di persentase 70%, 85%, dan 100%.
* **Logika Pemrograman:**
  * Subscribe ke event `CleaningSystemManager.OnCleanlinessRatioChanged(float ratio01)`.
  * Value slider bergerak halus via tween: `slider.DOValue(ratio01, 0.3f).SetEase(Ease.OutQuad)`.
  * **Dinamika Warna Bar (Gradasi Otomatis):**
    * 0% – 69%: Grime Brown (`#8E7A5F`)
    * 70% – 84%: Pastel Aqua (`#A8D8DF`) ➔ Bintang 1 meletup (*Pop Scale 1.4x*).
    * 85% – 99%: Coral Blush (`#E38382`) ➔ Bintang 2 meletup.
    * 100%: Golden Cream (`#FFF9F2`) ➔ Bintang 3 meletup + efek partikel sparkle UI!

---

#### 4.1.3. Checklist Misi Ruangan (`TaskChecklistUI`) `[P0 - CORE MVP]`
* **Visual & Layout Anchor:**
  * Anchor: `Top-Left` dengan margin aman 64px.
  * Komponen: Panel vertikal mini menampilkan 3 baris tugas:
    * Contoh: `[ ] Pel Dapur (100%)`
    * Contoh: `[ ] Nata Kulkas & Meja (2/2)`
    * Contoh: `[ ] Buang Sampah ke Garden (0/3)`
* **Logika Pemrograman:**
  * Subscribe ke event `RoomTaskManager.OnTaskProgressUpdated(string taskName, bool isDone)`.
  * Saat suatu tugas tuntas:
    1. Kotak `[ ]` berubah menjadi checklist hijau toska `[✔]`.
    2. Baris teks dicoret dan berkedip emas sesaat: `text.DOFade(0.5f, 0.3f)`.
    3. Muncul suara *ting!* renyah.

---

#### 4.1.4. Status Pemain & Stamina Gauge (`PlayerStatusUI`) `[P0 - CORE MVP]`
* **Visual & Layout Anchor:**
  * Terdapat dua mode tampilan:
    1. **HUD Anchor:** Di sudut `Bottom-Left` (P1 - Suami, Aksen Coral) dan `Bottom-Right` (P2 - Istri, Aksen Aqua).
    2. **World-Space Ring:** Lingkaran radial tipis semi-transparan di bawah kaki karakter yang hanya muncul saat berlari/dash (*On-Demand Visibility*).
* **Logika Pemrograman:**
  * Subscribe ke `PlayerStaminaController.OnStaminaChanged(float currentStamina, float maxStamina)`.
  * Fill amount stamina di-update secara instan.
  * **State Habis (Exhausted):** Lingkaran stamina berkedip merah dan karakter tidak bisa dash selama 1.5 detik (terengah-engah).

---

#### 4.1.5. Meteran Suasana Hati / Mood Pasangan (`MoodGaugeUI`) `[P2 - FEEL & FUN]`
* **Visual & Layout Anchor:**
  * Bersanding di sebelah potret karakter pada `PlayerStatusUI`.
  * Komponen: Ikon ekspresi wajah chibi 2D + Mini Bar Vertikal (0–100%).
* **Logika Pemrograman:**
  * Subscribe ke event `MoodChemistrySystem.OnMoodChanged(PlayerID id, float moodRatio, MoodState state)`.
  * Sprite wajah berganti otomatis:
    * `MoodState.Happy` (>75%): Wajah tersenyum ceria `^__^`.
    * `MoodState.Normal` (40–74%): Wajah santai.
    * `MoodState.Annoyed` (25–39%): Wajah cemberut datar.
    * `MoodState.Exhausted` (<25% - State Bete): Wajah pusing berkeringat `X__X`.
  * **Prompter Peluk:** Jika mood salah satu pemain < 30%, muncul balon chat mini berkedip: *"Peluk aku dong!"* di atas avatar pasangannya.

---

#### 4.1.6. Menu Radial Emote Wheel (`EmoteWheelUI`) `[P2 - FEEL & FUN]`
* **Visual & Layout Anchor:**
  * Layar penuh transparan (*Screen-Center*) yang hanya aktif saat tombol Emote ditahan.
  * Lingkaran radial terbagi menjadi 4 irisan (*slices*):
    * **Atas:** Peluk (Hug) — Icon Hati
    * **Kanan:** Tos (High-Five) — Icon Telapak Tangan
    * **Kiri:** Semangat (Cheer) — Icon Otot
    * **Bawah:** Iseng (Tease) — Icon Wajah Jahil
* **Logika Pemrograman:**
  * Pemain menahan tombol `LB` / `Q` / `Numpad .` ➔ Menu radial membal pop-up (`scale 0 -> 1.0` dalam 0.15s).
  * Arah Analog Stick dihitung menggunakan `Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg`.
  * Irisan yang ditunjuk analog stick membesar 1.2x dengan highlight warna Paper Cream.
  * Tombol dilepas ➔ Irisan yang terpilih dieksekusi ➔ Menu radial menutup sekejap.
* **Protokol No-Bug `[P1]`: `Deadzone Guard`**
  * Jika analog stick berada di deadzone tengah (< 0.3 radius), tidak ada irisan yang terpilih saat tombol dilepas (batal emote).

---

### 4.2. In-World & Floating World-Space UI (Affordance)

#### 4.2.1. Dynamic Billboard Interaction Prompts (`WorldInteractPromptUI`) `[P1 - ANTI-CONFUSION]`
* **Visual & Deskripsi:**
  Ikon tombol melayang di atas objek interaktif (misal tombol `A` / `E` di atas kardus, atau `Y` / `F` di atas alat pel).
* **Logika Pemrograman:**
  * Menggunakan Canvas World-Space. Selalu menghadap kamera (*Billboard rotation*):
    `transform.rotation = Camera.main.transform.rotation;`
  * **Auto-Detect Device Input:**
    * Jika input terdeteksi dari Gamepad: Menampilkan ikon tombol Gamepad (A, B, X, Y).
    * Jika input terdeteksi dari Keyboard: Menampilkan ikon tombol Keyboard (E, Space, F, Shift).
  * Muncul halus (*Fade In*) saat jarak pemain < 2.0 meter, dan menghilang (*Fade Out*) saat menjauh.

---

#### 4.2.2. Heavy Co-op Warning Bubble (`CoOpWarningUI`) `[P1 - ANTI-STUCK]`
* **Visual & Deskripsi:**
  Balon peringatan melayang saat P2 (Istri) mencoba mengangkat furnitur berat sendirian.
* **Logika Pemrograman:**
  * Menampilkan ikon animasi **"2 Pemain Dibutuhkan!"** bergoyang kiri-kanan (*wobble tween*).
  * Menghilang otomatis setelah 1.5 detik agar tidak menutupi pandangan layar.

---

#### 4.2.3. Overhead Emote Speech Bubble (`WorldEmoteBubbleUI`) `[P2 - FEEL & FUN]`
* **Visual & Deskripsi:**
  Balon obrolan komik 128x128 melayang di socket `EmoteAnchor` tepat di atas kepala karakter.
* **Logika Pemrograman:**
  * Spawn saat emote dipicu ➔ Pop scale dari 0 ke 1.15x lalu kembali ke 1.0x via DOTween:
    `transform.DOScale(1.0f, 0.25f).SetEase(Ease.OutBack);`
  * Mengapung naik perlahan 20 cm selama 1.2 detik, lalu mengecil dan hancur/kembali ke object pool.

---

#### 4.2.4. Off-Screen Partner Compass Indicator (`PartnerCompassUI`) `[P1 - ANTI-STRAGGLE]`
* **Visual & Deskripsi:**
  Panah penunjuk arah rekan setim yang menempel di tepi layar viewport saat kedua pemain terpisah jarak jauh.
* **Logika Pemrograman:**
  * Menghitung posisi viewport partner: `Vector3 screenPos = camera.WorldToViewportPoint(partnerTransform.position);`
  * Jika posisi berada di luar batas `(0,0)` hingga `(1,1)`:
    1. Panah muncul di batas tepi layar mengarah ke posisi rekan.
    2. Menampilkan angka meter jarak ke rekan (contoh: `"14m"`).
    3. Membantu pemain saling menemukan satu sama lain di denah rumah yang luas.

---

### 4.3. Split-Screen UI Layout Architecture

#### 4.3.1. Dynamic Dual Viewport Canvas & Separator Line (`SplitScreenUIDivider`) `[P1 - ANTI-BUG]`
* **Deskripsi Arsitektur:**
  Saat kamera bertransisi dari *Shared Mode* ke *Split-Screen Mode*:
  1. **Divider Line:** Garis vertikal 3px warna Deep Plum (`#251F44`) muncul di tengah layar (`x = 960px`).
  2. **Shared Overlay Canvas:** Elemen bersama (Timer jam dinding, Bintang milestone) tetap berada di **Top-Center layar utama** melintasi kedua viewport.
  3. **Local Viewport Canvas:**
     * Viewport Kiri (P1): Di-clamp di koordinat X: 0 – 960.
     * Viewport Kanan (P2): Di-clamp di koordinat X: 960 – 1920.
* **Protokol No-Bug `[P1]`: `Anchor Clamping`**
  * Elemen HUD lokal P1 tidak boleh menyeberang ke layar P2, dan sebaliknya, terbebas dari bug visual tumpang tindih.

---

### 4.4. Screen Flow, Menus & Navigation State Management

#### 4.4.1. Layar Menu Utama (`MainMenuUI`) `[P0 - CORE MVP]`
* **Komponen Tombol:** `Main Play`, `Pilih Stage`, `Pengaturan Audio/Kontrol`, `Kredit`, `Keluar`.
* **Protokol Gamepad Navigation `[P1]`: `Zero EventSystem Deadlock`**
  * Menggunakan `EventSystem.current.SetSelectedGameObject(firstButton)` saat scene dibuka.
  * Navigasi D-Pad dan Analog Stick memiliki loop navigasi tertutup (*Navigation Explicit / Vertical Wrap*).
  * **Tidak ada tombol hantu:** Pemain selalu bisa menggerakkan kursor menu tanpa mouse.

---

#### 4.4.2. Layar Pemilihan Level (`StageSelectUI`) `[P0 - CORE MVP]`
* **Komponen:** Kartu Level 0 (Dojo), Level 1 (Mudik Lebaran), Level 2 (Relokasi), Level 3 (Grand Chaos).
* **Data Display:** Menampilkan perolehan bintang emas tertinggi yang diraih (1–3 bintang) dan rekor waktu terbaik dari `PlayerPrefs` / Save JSON.

---

#### 4.4.3. Layar Jeda / Pause Menu (`PauseMenuUI`) `[P0 - CORE MVP]`
* **Komponen:** `Lanjutkan`, `Ulangi Level`, `Pengaturan`, `Kembali ke Menu Utama`.
* **Protokol Unscaled Time `[P1]`: `DOTween Freeze Prevention`**
  * Saat jeda: `Time.timeScale = 0f;`.
  * Seluruh animasi pop-up panel jeda **wajib** menggunakan `.SetUpdate(true)` agar animasi tween tidak macet saat `timeScale = 0`.
  * Saat pause, input gameplay dinonaktifkan total sehingga karakter tidak bergerak di latar belakang.

---

#### 4.4.4. Layar Hasil Kemenangan (`ScoreResultsUI`) `[P0 - CORE MVP]`
* **Alur Sekuensial (Dibuat Memuaskan & Dramatis via DOTween):**
  1. Panel rekap turun dari atas layar dengan efek guncangan empuk (*Ease.OutBounce*).
  2. Angka meter kebersihan dan sisa waktu berputar naik (*Tally Up Counter* dari 0 ke nilai akhir).
  3. **Bintang Dihitung Bertahap (Jeda 0.5s per bintang):**
     * Bintang 1 jatuh dari atas ➔ Suara *Ding!* ➔ Partikel bintang meletup.
     * Bintang 2 jatuh (jika lolos) ➔ Suara *Chime!*.
     * Bintang 3 jatuh (jika tuntas 100%) ➔ Suara *Fanfare Kemenangan!*.
  4. Tombol "Lanjut ke Level Berikutnya" otomatis terpilih sebagai fokus kursor gamepad.

---

#### 4.4.5. Layar Kegagalan Waktu Habis (`TimeoutDefeatUI`) `[P0 - CORE MVP]`
* **Komponen:** Ilustrasi komikal tamu sudah mengetuk pintu sementara rumah masih berantakan.
* Suara jingle kegagalan jenaka (*BGM_Jingle_Defeat*).
* Tombol: `Coba Lagi (Restart)` dan `Kembali ke Menu`.

---

#### 4.4.6. Layar Pengaturan Audio & Kontrol (`SettingsMenuUI`) `[P1 - QUALITY]`
* **Audio Sliders:** Master Volume, BGM Volume, SFX Volume (Terkoneksi langsung ke parameter AudioMixer: `MasterVol`, `MusicVol`, `SfxVol` via formula logaritmik `Mathf.Log10(val) * 20`).
* **Control Sheet Preview:** Menampilkan panduan gambar kontrol Gamepad & Keyboard P1/P2 yang bisa dilihat kapan saja.

---

### 4.5. Standar Arsitektur UI Programming (Panduan Implementasi Rasyid)

1. **Pola Desain: Model-View-Presenter (MVP) / Event-Driven:**
   * Script UI murni bertindak sebagai **View**.
   * Dilarang membaca data gameplay menggunakan `Update()` setiap frame (contoh: jangan menulis `cleanSlider.value = cleanManager.currentCleanRatio;` di `Update()`).
   * UI hanya memperbarui tampilan saat menerima **Event Action C#** dari Manager (contoh: `OnCleanlinessChanged += UpdateSlider;`).
2. **Penggunaan Font & Styling Standar:**
   * Seluruh teks angka dan heading wajib menggunakan TextMeshPro font asset **Fredoka Bold** (tersedia di `Assets/Resources/Fonts/`).
   * Seluruh label penjelasan menggunakan **Nunito SemiBold**.
3. **Canvas Scaler Standar:**
   * UI Scale Mode: `Scale With Screen Size`
   * Reference Resolution: `1920 x 1080`
   * Screen Match Mode: `Match Width Or Height`, Slider Match: `0.5` (seimbang untuk multi-resolusi).
4. **Alokasi Memori & Object Pooling:**
   * Balon interaksi dan ikon emote world-space menggunakan `ObjectPool` sederhana untuk mencegah alokasi garbage collection (`GC.Alloc`) yang memicu stutter saat gameplay.

---

## 📋 TABEL RANGKUMAN TUGAS PROGRAMMER UI (RASYID & TIM UI)

| Nama Script C# | Tanggung Jawab Teknis | Prioritas | PIC |
| :--- | :--- | :---: | :---: |
| `UIManager.cs` | Mengatur navigasi transisi antar scene & layar menu. | **[P0]** | Rasyid |
| `HUDManager.cs` | Mengorkestrasi seluruh widget HUD in-game saat level berjalan. | **[P0]** | Rasyid |
| `WallClockTimerUI.cs` | Timer jam dinding analog & digital + efek denyut panik <30s. | **[P0]** | Rasyid |
| `CleanlinessMeterUI.cs` | Progress bar kebersihan + peletupan 3 bintang milestone. | **[P0]** | Rasyid |
| `TaskChecklistUI.cs` | Daftar checklist tugas peruangan dinamis. | **[P0]** | Rasyid |
| `PlayerStatusUI.cs` | Potret P1/P2 + lingkaran stamina recharge di bawah kaki. | **[P0]** | Rasyid |
| `ScoreResultsUI.cs` | Alur sekuensial perhitungan skor, animasi bintang, dan stinger audio. | **[P0]** | Rasyid |
| `PauseMenuUI.cs` | Menu jeda unscaled time (`timeScale = 0`) tanpa freeze DOTween. | **[P0]** | Rasyid |
| `MainMenuUI.cs` | Navigasi menu utama gamepad-friendly bebas deadlock. | **[P0]** | Rasyid |
| `StageSelectUI.cs` | Kartu pemilihan stage 0–3 + pembacaan save rekor bintang. | **[P0]** | Rasyid |
| `TimeoutDefeatUI.cs` | Layar kekalahan waktu habis komikal. | **[P0]** | Rasyid |
| `WorldInteractPromptUI.cs` | Billboard prompt tombol interaksi dengan auto-detect gamepad/keyboard. | **[P1]** | Rasyid |
| `CoOpWarningUI.cs` | Balon peringatan butuh 2 pemain saat P2 angkat perabot berat. | **[P1]** | Rasyid |
| `PartnerCompassUI.cs` | Panah penunjuk arah rekan off-screen di batas tepi layar. | **[P1]** | Rasyid |
| `SplitScreenUIDivider.cs` | Garis pembatas vertikal 3px + clamping posisi HUD lokal. | **[P1]** | Rasyid |
| `SettingsMenuUI.cs` | Slider audio terhubung AudioMixer + panduan visual tombol kontrol. | **[P1]** | Rasyid |
| `MoodGaugeUI.cs` | Meteran mood pasangan + perubahan sprite wajah ekspresi. | **[P2]** | Rasyid |
| `EmoteWheelUI.cs` | Menu radial 4 irisan (Peluk, Tos, Semangat, Iseng) via analog stick. | **[P2]** | Rasyid |
| `WorldEmoteBubbleUI.cs` | Balon ekspresi komik melayang di atas kepala karakter. | **[P2]** | Rasyid |

---

## 📊 MASTER CHECKLIST KESIAPAN GAMEPLAY & UI (ZERO-BUG AUDIT)

Gunakan tabel ini sebelum melakukan build atau playtest:

| Skenario Pengujian Ekstrem (Edge Cases) | Mekanisme Pencegahan di Kode | Status Validasi |
| :--- | :--- | :---: |
| Pemain mencoba melewati pintu membawa sofa miring | **Auto-Align Torsi:** Sofa otomatis memanjang searah pintu saat dekat kusen. | **NO STUCK** |
| Pemain melempar kulkas ke luar batas pagar level | **OOB Recovery Trigger:** Kulkas otomatis respawn ke tengah ruangan asal. | **NO LOST ITEM** |
| Pemain menaruh barang persis di dalam dinding | **Safe Drop SphereCast:** Barang digeser ke lantai bebas terdekat. | **NO CLIPPING** |
| Pemain berjalan menyerong menempel di sudut dinding | **Zero-Friction Capsule:** Pemain meluncur mulus tanpa tersendat di dinding. | **NO SNAGGING** |
| Tersisa 1% noda kecil tak terlihat di bawah lemari | **95% Clean Threshold:** Ruangan otomatis tuntas 100% tanpa pixel hunting. | **NO SOFTLOCK** |
| Pemain 2 AFK di ruangan seberang saat split screen | **Screen Leash & Recall:** Tombol teleport santai ke samping rekan. | **NO STRAGGLING** |
| Pintu geser menutup saat pemain berada di tengah | **Anti-Pinch Rebound:** Pintu membal terbuka, pemain tidak terjepit. | **NO CRUSHING** |
| Game di-pause lalu animasi UI macet total | **Unscaled Time Tween:** Menggunakan `.SetUpdate(true)` pada DOTween UI. | **NO UI FREEZE** |
| Navigasi Gamepad di Menu Utama kehilangan fokus kursor | **Auto-Focus EventSystem:** Focus trapping eksplisit, kursor tidak pernah hilang. | **NO DEADLOCK** |
| Split-screen aktif dan HUD lokal tumpang tindih | **Clamped Viewport Anchors:** Canvas P1 di-clamp di X < 960, P2 di X > 960. | **NO OVERLAP** |

---
*Dokumen ini adalah spesifikasi resmi arsitektur mekanik gim "No, Please Clean the Room!". Seluruh implementasi script C# pada sprint wajib mengacu pada parameter angka dan protokol no-stuck di atas.*
